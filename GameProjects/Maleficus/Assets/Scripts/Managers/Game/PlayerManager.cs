using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Maleficus.MaleficusUtilities;
using static Maleficus.MaleficusConsts;

public class PlayerManager : AbstractSingletonManager<PlayerManager>
{
    /* Dictionaries that are initialized with all 4 players (weither they are connected or not) */
    /// <summary> Reference to all player prefabs to be spawned. </summary>
    public Dictionary<EPlayerID, Player> PlayerPrefabs { get; } = new Dictionary<EPlayerID, Player>();

    /// <summary> Positions in the scene (or around PlayerManager if not found) where the players will be spawned. </summary>
    public Dictionary<EPlayerID, PlayerSpawnPosition> PlayersSpawnPositions { get; } = new Dictionary<EPlayerID, PlayerSpawnPosition>();

    /* Dictionaries that are defined only for connected players  */
    /// <summary> Added whenever a player has spawned. Removed when he dies. </summary>
    public Dictionary<EPlayerID, Player> ActivePlayers { get; } = new Dictionary<EPlayerID, Player>();

    /// <summary> All assigned players for every team. </summary>
    public Dictionary<ETeamID, List<EPlayerID>> Teams { get; } = new Dictionary<ETeamID, List<EPlayerID>>();

    /// <summary> Assigned team of every player </summary>
    public Dictionary<EPlayerID, ETeamID> PlayersTeam { get; } = new Dictionary<EPlayerID, ETeamID>();
    public EPlayerID OwnPlayerID { get { return GetPlayerIDFrom(NetworkManager.Instance.OwnerClientID); } }

    /// <summary> Joysticks inputs for every player (movement, rotation). </summary>
    public Dictionary<EPlayerID, JoystickInput> PlayersMovement { get; } = new Dictionary<EPlayerID, JoystickInput>();

    /// <summary> The join status of player that have connected with a controllers </summary>
    public Dictionary<EPlayerID, PlayerJoinStatus> PlayersJoinStatus { get; } = new Dictionary<EPlayerID, PlayerJoinStatus>();

    /// <summary>
    /// Get all players that are connected with a controller.
    /// Warning! A connected player is not necessarily a player that has joined the game session
    /// </summary>
    public EPlayerID[] GetConnectedPlayers()
    {
        return InputManager.Instance.ConnectedControllers.Values.ToArray<EPlayerID>();
    }

    /// <summary>
    /// Get the player that is connected with the given controller.
    /// Warning! A connected player is not necessarily a player that has joined the game session
    /// </summary>
    public bool IsPlayerConnected(EPlayerID playerID)
    {
        return InputManager.Instance.ConnectedControllers.ContainsValue(playerID);
    }

    // TODO: Add a list of active Coroutines for every player to stop when he dies
    protected override void Awake()
    {
        base.Awake();

        InitializeDictionaries();
        LoadPlayerResources();
    }


    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        // Input events
        EventManager.Instance.INPUT_ControllerConnected.AddListener(On_INPUT_ControllerConnected);
        EventManager.Instance.INPUT_ControllerDisconnected.AddListener(On_INPUT_ControllerDisconnected);

        EventManager.Instance.INPUT_ButtonPressed.AddListener(On_INPUT_ButtonPressed);
        EventManager.Instance.INPUT_ButtonReleased.AddListener(On_INPUT_ButtonReleased);
        //EventManager.Instance.INPUT_JoystickMoved.AddListener                   (On_INPUT_JoystickMoved);
        // Listen to broadcasted inputs 
        EventManager.Instance.INPUT_JoystickMoved.AddListener(On_SERVER_INPUT_JoystickMoved);

        // Scene changed event
        EventManager.Instance.APP_SceneChanged.AddListener(On_APP_SceneChanged);
        EventManager.Instance.APP_AppStateUpdated.AddListener(On_APP_AppStateUpdated);

        //Network
        //EventManager.Instance.NETWORK_ReceivedGameSessionInfo.AddListener       (On_NETWORK_ReceivedGameSessionInfo);
        EventManager.Instance.NETWORK_GameStateReplication.AddListener(On_NETWORK_GameStateReplicate);
        EventManager.Instance.NETWORK_GameStarted.AddListener(On_NETWORK_GameStarted);

        EventManager.Instance.PLAYERS_PlayerJoined += On_PLAYERS_PlayerJoined;
        EventManager.Instance.PLAYERS_PlayerLeft += On_PLAYERS_PlayerLeft;
        EventManager.Instance.PLAYERS_PlayerReady += On_PLAYERS_PlayerReady;
        EventManager.Instance.PLAYERS_PlayerCanceledReady += On_PLAYERS_PlayerCanceledReady;
    }


    protected override void Update()
    {
        base.Update();

        string playerStatusLog = "";
        foreach (var pair in PlayersJoinStatus)
        {
            playerStatusLog += pair.Key + " - joined : " + pair.Value.HasJoined + " | is ready : " + pair.Value.IsReady;
        }
        LogCanvas(69, playerStatusLog);
    }




    public override void OnSceneStartReinitialize()
    {
        FindPlayerSpawnGhost();

        ActivePlayers.Clear();
    }




    #region Players Management
    public void SpawnPlayer(EPlayerID toSpawnPlayerID)
    {
        if ((IsPlayerConnected(toSpawnPlayerID) == true)
            || ((MotherOfManagers.Instance.IsSpawnRemainingPlayersOnGameStart == true)
                && (AppStateManager.Instance.CurrentScene.ContainedIn(GAME_SCENES))))
        {

            if (ActivePlayers.ContainsKey(toSpawnPlayerID) == false)
            {
                Player playerPrefab = PlayerPrefabs[toSpawnPlayerID];
                PlayerSpawnPosition playerSpawnPosition = PlayersSpawnPositions[toSpawnPlayerID];
                Vector3 playerPosition = playerSpawnPosition.Position;
                Quaternion playerRotation = playerSpawnPosition.Rotation;

                Player spawnedPlayer = Instantiate(playerPrefab, playerPosition, playerRotation);
                spawnedPlayer.PlayerID = toSpawnPlayerID;
                spawnedPlayer.TeamID = PlayersTeam[toSpawnPlayerID];

                ActivePlayers.Add(toSpawnPlayerID, spawnedPlayer);

                // Place player under parent of SpawnPosition
                if (playerSpawnPosition.transform.parent != null)
                {
                    spawnedPlayer.transform.parent = playerSpawnPosition.transform.parent;
                }

                EventManager.Instance.Invoke_PLAYERS_PlayerSpawned(toSpawnPlayerID);
            }
            else
            {
                Debug.LogWarning("Trying to spawn a player that is still active");
            }
        }
    }

    private void AssignPlayerToTeam(EPlayerID playerID, ETeamID teamID)
    {
        // First remove from old team
        ETeamID oldTeamID = PlayersTeam[playerID];
        if (oldTeamID != ETeamID.NONE)
        {
            Teams[oldTeamID].Remove(playerID);
        }

        Teams[teamID].Add(playerID);
        PlayersTeam[playerID] = teamID;
    }

    private void RemovePlayerFromTeam(EPlayerID playerID, ETeamID teamID)
    {
        // TODO: Implement removing player from team
    }

    /// <summary>
    /// Looks for the next possibe free slot of a PlayerID
    /// </summary>
    /// <returns> The ID of the PlayerID free spot (NONE if none found)</returns>
    public EPlayerID GetNextFreePlayerID()
    {
        EPlayerID result = EPlayerID.NONE;
        for (int i = 1; i < 5; i++)
        {
            EPlayerID currentPlayerID = IntToPlayerID(i);
            if (IsPlayerConnected(currentPlayerID) == false)
            {
                result = currentPlayerID;
                break;
            }
        }
        return result;
    }


    #endregion

    #region Input


    private void On_INPUT_ButtonPressed(NetEvent_ButtonPressed eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EPlayerID playerID = GetPlayerIDFrom(eventHandle.SenderID);

        ESpellSlot spellSlot = GetSpellSlotFrom(inputButton);
        if ((spellSlot == ESpellSlot.NONE) || (playerID == EPlayerID.TEST) || (ActivePlayers.ContainsKey(playerID) == false))
        {
            return;
        }

        ISpell spell = SpellManager.Instance.Player_Spells[playerID][spellSlot];

        // Instantiate spell now ?
        if (spell.MovementType == ESpellMovementType.LINEAR_LASER)
        {
            if (ActivePlayers[playerID].IsReadyToShoot && ActivePlayers[playerID].ReadyToUseSpell[spellSlot])
            {

                ActivePlayers[playerID].IsReadyToShoot = false;
                ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
                ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);

                SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingLVL);

                StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));
            }
        }
        else if (spell.MovementType == ESpellMovementType.RAPID_FIRE)
        {
            if (ActivePlayers[playerID].IsReadyToShoot && ActivePlayers[playerID].ReadyToUseSpell[spellSlot])
            {

                StartCoroutine(FirstTimeSpellCastedCoroutine(playerID, spellSlot, spell.CastingDuration));
                SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingLVL);

                StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));
            }
        }
        else if (spell.MovementType == ESpellMovementType.UNIQUE)
        {
            if (ActivePlayers[playerID].IsReadyToShoot && ActivePlayers[playerID].ReadyToUseSpell[spellSlot])
            {
                if (!ActivePlayers[playerID].hasCastedSpell)
                {
                    StartCoroutine(FirstTimeSpellCastedCoroutine(playerID, spellSlot, spell.CastingDuration));
                    SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingLVL);
                    StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));
                }




            }
            else
            {
                EventManager.Instance.Invoke_SPELLS_UniqueEffectActivated(spell.SpellID, playerID);
            }
        }

        else if (spell.IsChargeable)
        {

            ActivePlayers[playerID].StartChargingSpell(spell, spellSlot);
            Debug.Log("Start charging");

        }


    }

    private void On_INPUT_ButtonReleased(NetEvent_ButtonReleased eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EPlayerID playerID = GetPlayerIDFrom(eventHandle.SenderID);

        ESpellSlot spellSlot = GetSpellSlotFrom(inputButton);
        if ((spellSlot == ESpellSlot.NONE) || (ActivePlayers.ContainsKey(playerID) == false))
        {
            return;
        }

        ISpell spell = SpellManager.Instance.Player_Spells[playerID][spellSlot];

        if (spell.MovementType != ESpellMovementType.LINEAR_LASER && spell.MovementType != ESpellMovementType.RAPID_FIRE && spell.MovementType != ESpellMovementType.UNIQUE)
        {
            if (ActivePlayers[playerID].IsReadyToShoot && ActivePlayers[playerID].ReadyToUseSpell[spellSlot])
            {

                ActivePlayers[playerID].IsReadyToShoot = false;
                ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
                ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);

                SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingLVL);

                StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));
            }
        }

    }


    private void On_SERVER_INPUT_JoystickMoved(NetEvent_JoystickMoved eventHandle)
    {
        EJoystickType joystickType = eventHandle.JoystickType;
        float joystick_X = eventHandle.Joystick_X;
        float joystick_Y = eventHandle.Joystick_Y;
        EPlayerID playerID = GetPlayerIDFrom(eventHandle.SenderID);


        if (PlayersMovement.ContainsKey(playerID))
        {
            if (joystickType == EJoystickType.MOVEMENT)
            {
                PlayersMovement[playerID].JoystickValues[EInputAxis.MOVE_X] = joystick_X;
                PlayersMovement[playerID].JoystickValues[EInputAxis.MOVE_Y] = joystick_Y;
            }
            else if (joystickType == EJoystickType.ROTATION)
            {
                PlayersMovement[playerID].JoystickValues[EInputAxis.ROTATE_X] = joystick_X;
                PlayersMovement[playerID].JoystickValues[EInputAxis.ROTATE_Y] = joystick_Y;
            }
        }
    }

    #endregion

    #region Events Callbacks
    private void On_APP_SceneChanged(Event_GenericHandle<EScene> eventHandle)
    {
        EScene newScene = eventHandle.Arg1;
        if (newScene.ContainedIn(GAME_SCENES))
        {
            FindPlayerSpawnGhost();
        }
    }

    private void On_APP_AppStateUpdated(Event_StateUpdated<EAppState> eventHandle)
    {
        if (eventHandle.NewState == EAppState.IN_GAME_IN_NOT_STARTED)
        {
            SpawnAllJoinedPlayers();
        }
    }
    #endregion

    #region Initialization
    private void InitializeDictionaries()
    {
        Teams.Clear();
        Teams[ETeamID.TEAM_1] = new List<EPlayerID>();
        Teams[ETeamID.TEAM_2] = new List<EPlayerID>();
        Teams[ETeamID.TEAM_3] = new List<EPlayerID>();
        Teams[ETeamID.TEAM_4] = new List<EPlayerID>();

        PlayersTeam.Clear();
        PlayersTeam[EPlayerID.PLAYER_1] = ETeamID.NONE;
        PlayersTeam[EPlayerID.PLAYER_2] = ETeamID.NONE;
        PlayersTeam[EPlayerID.PLAYER_3] = ETeamID.NONE;
        PlayersTeam[EPlayerID.PLAYER_4] = ETeamID.NONE;
    }


    private void LoadPlayerResources()
    {
        PlayerPrefabs.Clear();
        PlayerPrefabs.Add(EPlayerID.PLAYER_1, Resources.Load<Player>(PATH_PLAYER_RED));
        PlayerPrefabs.Add(EPlayerID.PLAYER_2, Resources.Load<Player>(PATH_PLAYER_BLUE));
        PlayerPrefabs.Add(EPlayerID.PLAYER_3, Resources.Load<Player>(PATH_PLAYER_YELLOW));
        PlayerPrefabs.Add(EPlayerID.PLAYER_4, Resources.Load<Player>(PATH_PLAYER_GREEN));
    }

    private void FindPlayerSpawnGhost()
    {
        PlayersSpawnPositions.Clear();

        // Try to find already placed player spawn positions in the scene
        PlayerSpawnPosition[] spawnPositions = FindObjectsOfType<PlayerSpawnPosition>();
        foreach (PlayerSpawnPosition spawnPosition in spawnPositions)
        {
            PlayersSpawnPositions.Add(spawnPosition.ToSpawnPlayerID, spawnPosition);
        }

        // Determine spawn positions relative to this transform if no PlayerSpawnPosition found in scene
        if (MotherOfManagers.Instance.IsSpawnGhostPlayerPositionsIfNotFound == true)
        {
            int angle;
            for (int i = 1; i < 5; i++)
            {
                angle = 90 * i;
                EPlayerID playerID = IntToPlayerID(i);
                if (PlayersSpawnPositions.ContainsKey(playerID) == false)
                {
                    PlayerSpawnPosition spawnGhost = Instantiate(Resources.Load<PlayerSpawnPosition>(PATH_PLAYER_SPAWN_POSITION));
                    spawnGhost.ToSpawnPlayerID = playerID;
                    spawnGhost.Position = transform.position + Vector3.forward * 3.0f + Vector3.left * 3.0f;
                    spawnGhost.transform.RotateAround(transform.position, Vector3.up, angle);
                    spawnGhost.Rotation = transform.rotation;
                    PlayersSpawnPositions.Add(playerID, spawnGhost);
                }
            }
        }
    }
    #endregion


    private void SpawnAllJoinedPlayers()
    {
        foreach (EPlayerID playerID in PlayersJoinStatus.Keys)
        {
            if (PlayersJoinStatus[playerID].HasJoined == true)
            {
                SpawnPlayer(playerID);
            }
        }
    }

    public JoystickInput GetPlayerInput(EPlayerID playerID)
    {
        if (PlayersMovement.ContainsKey(playerID) == false)
        {
            if (MotherOfManagers.Instance.IsSpawnRemainingPlayersOnGameStart == false)
            {
                Debug.LogError("No player movement found for : " + playerID);
            }
            return new JoystickInput();
        }
        return PlayersMovement[playerID];
    }

    public bool IsPlayerActive(EPlayerID playerID)
    {
        return ActivePlayers.ContainsKey(playerID);
    }

    public EPlayerID[] GetPlayersInTeam(ETeamID inTeamID)
    {
        return Teams[inTeamID].ToArray();
    }


    private IEnumerator FirstTimeSpellCastedCoroutine(EPlayerID playerID, ESpellSlot spellSlot, float duration)
    {

        if (!ActivePlayers[playerID].hasCastedSpell)
        {
            ActivePlayers[playerID].hasCastedSpell = true;
            yield return new WaitForSeconds(duration);
            ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
            ActivePlayers[playerID].IsReadyToShoot = false;
        }
        else { yield return new WaitForSeconds(0f); }
    }
    private IEnumerator SetReadyToUseSpellCoroutine(EPlayerID playerID, ESpellSlot spellSlot)
    {
        if (ActivePlayers.ContainsKey(playerID))
        {
            yield return new WaitForSeconds(ActivePlayers[playerID].SpellDuration[spellSlot]);
            ActivePlayers[playerID].IsReadyToShoot = true;

            yield return new WaitForSeconds(ActivePlayers[playerID].SpellCooldown[spellSlot]);
            ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = true;
            ActivePlayers[playerID].hasCastedSpell = false;
        }

    }

    public void OnPlayerOutOfBound(EPlayerID playerID)
    {
        if (ActivePlayers.ContainsKey(playerID))
        {
            StartCoroutine(DestroyPlayerCoroutine(playerID));
        }
    }

    private IEnumerator DestroyPlayerCoroutine(EPlayerID playerID)
    {
        yield return new WaitForSeconds(PLAYER_FALLING_TIME);
        if (ActivePlayers.ContainsKey(playerID))
        {
            Player playerToDestroy = ActivePlayers[playerID];
            ActivePlayers.Remove(playerID);
            playerToDestroy.DestroyPlayer();
            EventManager.Instance.Invoke_PLAYERS_PlayerDied(playerID);
        }
    }

    private void On_INPUT_ControllerConnected(Event_GenericHandle<EControllerID, EPlayerID> eventHandle)
    {
        EControllerID controllerID = eventHandle.Arg1;
        EPlayerID playerID = eventHandle.Arg2;
        AssignPlayerToTeam(playerID, GetIdenticPlayerTeam(playerID));

        // Initialize dictionaries for the new player
        if (IS_KEY_NOT_CONTAINED(PlayersMovement, playerID))
        {
            PlayersMovement.Add(playerID, new JoystickInput());
        }
        if (IS_KEY_NOT_CONTAINED(PlayersJoinStatus, playerID))
        {
            PlayersJoinStatus.Add(playerID, new PlayerJoinStatus());
        }

        // Spawn player On Connect?
        if ((MotherOfManagers.Instance.IsSpawnPlayerOnControllerConnect == true)
            && (ActivePlayers.ContainsKey(playerID) == false)
            && (AppStateManager.Instance.CurrentScene.ContainedIn(GAME_SCENES)))
        {
            SpawnPlayer(playerID);
        }
    }

    private void On_INPUT_ControllerDisconnected(Event_GenericHandle<EControllerID, EPlayerID> eventHandle)
    {
        EPlayerID playerID = eventHandle.Arg2;
        if (PlayersMovement.ContainsKey(playerID) == true)
        {
            PlayersMovement.Remove(playerID);
        }
        else
        {
            Debug.LogError("Trying to disconnect a player that is not connected");
        }
    }


    private void On_PLAYERS_PlayerJoined(EPlayerID playerID)
    {
        if (IS_KEY_CONTAINED(PlayersJoinStatus, playerID))
        {
            PlayersJoinStatus[playerID].HasJoined = true;
        }
    }

    private void On_PLAYERS_PlayerLeft(EPlayerID playerID)
    {
        if (IS_KEY_CONTAINED(PlayersJoinStatus, playerID))
        {
            PlayersJoinStatus[playerID].HasJoined = false;
        }
    }

    private void On_PLAYERS_PlayerReady(EPlayerID playerID)
    {
        if (IS_KEY_CONTAINED(PlayersJoinStatus, playerID))
        {
            PlayersJoinStatus[playerID].IsReady = true;

            // Check if all players are now ready
            bool areAllReady = true;
            foreach (PlayerJoinStatus playerJoinStatus in PlayersJoinStatus.Values)
            {
                if (playerJoinStatus.IsReady == false)
                {
                    areAllReady = false;
                    break;
                }
            }
            if (areAllReady == true)
            {
                EventManager.Instance.Invoke_PLAYERS_AllPlayersReady();
            }
        }
    }

    private void On_PLAYERS_PlayerCanceledReady(EPlayerID playerID)
    {
        if (IS_KEY_CONTAINED(PlayersJoinStatus, playerID))
        {
            PlayersJoinStatus[playerID].IsReady = false;
        }
    }

    private void On_NETWORK_GameStateReplicate(NetEvent_GameStateReplication eventHandle)
    {
        EPlayerID playerID = eventHandle.UpdatedPlayerID;
        float[] playerPosition = eventHandle.playerPosition;

        if (ActivePlayers.ContainsKey(playerID))
        {
            ActivePlayers[playerID].transform.localPosition = new Vector3(playerPosition[0], playerPosition[1], playerPosition[2]);
        }
    }

    private void On_NETWORK_GameStarted(NetEvent_GameStarted eventHandle)
    {
        SpawnAllJoinedPlayers();
    }
}
