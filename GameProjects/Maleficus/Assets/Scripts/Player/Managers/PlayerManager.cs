using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Maleficus.Utils;
using static Maleficus.Consts;

public class PlayerManager : AbstractSingletonManager<PlayerManager>
{
    ///* Dictionaries that are initialized with all 4 players (weither they are connected or not) */
    //public List<EPlayerID> JoinedPlayers { get; private set; } = new List<EPlayerID>();

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

    /// <summary> The join status of player that have connected with a controllers </summary>
    private Dictionary<EPlayerID, PlayerJoinStatus> playersJoinStatus { get; } = new Dictionary<EPlayerID, PlayerJoinStatus>();

    private Dictionary<EPlayerID, Vector3> PlayersDeathPositions = new Dictionary<EPlayerID, Vector3>();
    private Dictionary<EPlayerID, Quaternion> PlayersDeathRotations = new Dictionary<EPlayerID, Quaternion>();

    private Dictionary<EPlayerID, Player> playersPrefabs { get; } = new Dictionary<EPlayerID, Player>();
    private PlayerSpawnPosition playerSpawnPositionPrefab;
    private PlayerRespawnGhost playerRespawnGhostPrefab;


    public List<EPlayerID> GetJoinedPlayers()
    {
        List<EPlayerID> result = new List<EPlayerID>();
        foreach (KeyValuePair<EPlayerID, PlayerJoinStatus> pair in playersJoinStatus)
        {
            if (pair.Value.HasJoined == true)
            {
                result.Add(pair.Key);
            }
        }
        return result;
    }

    public bool HasPlayerJoined(EPlayerID playerID)
    {
        if ((playersJoinStatus.ContainsKey(playerID))
            && (playersJoinStatus[playerID].HasJoined))
        {
            return true;
        }
        return false;
    }

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

        // Scene changed event
        EventManager.Instance.APP_SceneChanged.Event += On_APP_SceneChanged_Event;

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
        foreach (var pair in playersJoinStatus)
        {
            playerStatusLog += pair.Key + " - joined : " + pair.Value.HasJoined + " | is ready : " + pair.Value.IsReady + "\n";
        }
        LogCanvas(69, playerStatusLog);
    }


    protected override void OnReinitializeManager()
    {
        base.OnReinitializeManager();

        FindPlayerSpawnGhost();

        ActivePlayers.Clear();
    }




    #region Players Management
    public void SpawnPlayer(EPlayerID playerID)
    {
        if (IsPlayerConnected(playerID) == true)
        {
            if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID))
            {
                Player playerPrefab = playersPrefabs[playerID];
                PlayerSpawnPosition playerSpawnPosition = PlayersSpawnPositions[playerID];
                Vector3 playerPosition = playerSpawnPosition.Position;
                Quaternion playerRotation = playerSpawnPosition.Rotation;

                Player spawnedPlayer = Instantiate(playerPrefab, playerPosition, playerRotation);
                spawnedPlayer.PlayerID = playerID;
                spawnedPlayer.TeamID = PlayersTeam[playerID];

                ActivePlayers.Add(playerID, spawnedPlayer);

                // Place player under parent of SpawnPosition
                if (playerSpawnPosition.transform.parent != null)
                {
                    spawnedPlayer.transform.parent = playerSpawnPosition.transform.parent;
                }

                EventManager.Instance.Invoke_PLAYERS_PlayerSpawned(playerID);
            }
            else
            {
                Debug.LogWarning("Trying to spawn a player that is still active");
            }
        }
    }

    public void RespawnPlayer(EPlayerID playerID)
    {
        if ((IS_KEY_NOT_CONTAINED(ActivePlayers, playerID))
            && (IS_KEY_CONTAINED(PlayersDeathPositions, playerID))
            && (IS_KEY_CONTAINED(PlayersDeathRotations, playerID))
            && (IS_KEY_CONTAINED(PlayersSpawnPositions, playerID)))
        {
            Vector3 startPostion = PlayersDeathPositions[playerID];
            Quaternion startRotation = PlayersDeathRotations[playerID];
            Vector3 endPosition = PlayersSpawnPositions[playerID].Position;
            Quaternion endRotation = PlayersSpawnPositions[playerID].Rotation;

            PlayerRespawnGhost playerRespawnGhost = Instantiate(playerRespawnGhostPrefab, startPostion, startRotation);
            playerRespawnGhost.PlayerID = playerID;
            playerRespawnGhost.RespawnAnimationDone += On_RespawnAnimationDone;
            playerRespawnGhost.StartRespawnAnimation(startPostion, startRotation, endPosition, endRotation);
        }
    }

    private void DestroyPlayer(EPlayerID playerID)
    {
        if (IS_KEY_CONTAINED(ActivePlayers, playerID))
        {
            Player player = ActivePlayers[playerID];
            if ((IS_KEY_CONTAINED(PlayersDeathPositions, playerID))
                && (IS_KEY_CONTAINED(PlayersDeathRotations, playerID)))
            {
                PlayersDeathPositions[playerID] = player.Position;
                PlayersDeathRotations[playerID] = player.Rotation;
            }

            ActivePlayers.Remove(playerID);
            player.DestroyPlayer();
            EventManager.Instance.Invoke_PLAYERS_PlayerDied(playerID);
        }
    }

    private void SpawnAllJoinedPlayers()
    {
        foreach (EPlayerID playerID in GetJoinedPlayers())
        {
            if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID))
            {
                SpawnPlayer(playerID);
            }
        }
    }

    private void SpawnRemaningAIPlayers()
    {
        // First connect AI Controllers on reamining slots if not already connected
         InputManager.Instance.ConnectAllRemainingAIPlayers();

        LogConsole("Spawning remaining AI playeres");

        foreach (KeyValuePair<EControllerID, EPlayerID> pair in InputManager.Instance.ConnectedControllers)
        {
            EControllerID controllerID = pair.Key;
            EPlayerID playerID = pair.Value;

            if ((controllerID.ContainedIn(AI_CONTROLLERS))
                && (ActivePlayers.ContainsKey(playerID) == false)
                && (InputManager.Instance.IsControllerConnected(controllerID)))
            {
                SpawnPlayer(playerID);
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
            EPlayerID currentPlayerID = GetPlayerIDFrom(i);
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
        if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {
            EInputButton inputButton = eventHandle.InputButton;
            EPlayerID playerID = GetPlayerIDFrom(eventHandle.SenderID);
            ESpellSlot spellSlot = GetSpellSlotFrom(inputButton);

            if ((spellSlot != ESpellSlot.NONE)
                && (ActivePlayers.ContainsKey(playerID) == true))
            {
                AbstractSpell spell = SpellManager.Instance.GetChosenSpell(playerID, spellSlot);
                if (IS_NOT_NULL(spell))
                {
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
                    // TODO: clean this (Removed because it felt wrong on android NetController)
                    //else if (spell.MovementType == ESpellMovementType.RAPID_FIRE)
                    //{
                    //    if (ActivePlayers[playerID].IsReadyToShoot && ActivePlayers[playerID].ReadyToUseSpell[spellSlot])
                    //    {

                    //        // StartCoroutine(FirstTimeSpellCastedCoroutine(playerID, spellSlot, spell.CastDuration));
                    //        ActivePlayers[playerID].IsReadyToShoot = false;
                    //        ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
                    //        SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingLVL);

                    //        StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));
                    //    }
                    //}
                    else if (spell.MovementType == ESpellMovementType.UNIQUE)
                    {
                        if (ActivePlayers[playerID].IsReadyToShoot && ActivePlayers[playerID].ReadyToUseSpell[spellSlot])
                        {
                            if (!ActivePlayers[playerID].HasCastedSpell)
                            {
                                StartCoroutine(FirstTimeSpellCastedCoroutine(playerID, spellSlot, spell.CastDuration));
                                SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingLVL);
                                StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));
                            }
                        }
                        else
                        {
                            EventManager.Instance.Invoke_SPELLS_UniqueEffectActivated(spell.SpellID, playerID);
                        }
                    }

                    else if (spell.IsChargeable && ActivePlayers[playerID].IsReadyToShoot)
                    {
                        ActivePlayers[playerID].StartChargingSpell(spell, spellSlot);
                        Debug.Log("Start charging");

                    }
                }
            }
        }
    }

    private void On_INPUT_ButtonReleased(NetEvent_ButtonReleased eventHandle)
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {
            EInputButton inputButton = eventHandle.InputButton;
            EPlayerID playerID = GetPlayerIDFrom(eventHandle.SenderID);

            ESpellSlot spellSlot = GetSpellSlotFrom(inputButton);
            if ((spellSlot == ESpellSlot.NONE) || (ActivePlayers.ContainsKey(playerID) == false))
            {
                return;
            }

            AbstractSpell spell = SpellManager.Instance.GetChosenSpell(playerID, spellSlot);
            if (IS_NOT_NULL(spell))
            {
                                                                            // TODO: clean this (Removed because it felt wrong on android NetController)
                if (spell.MovementType != ESpellMovementType.LINEAR_LASER /*&& spell.MovementType != ESpellMovementType.RAPID_FIRE*/ && spell.MovementType != ESpellMovementType.UNIQUE)
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
        }
    }

    #endregion

    #region Events Callbacks

    private void On_APP_SceneChanged_Event(Event_GenericHandle<EScene> eventHandle)
    {
        EScene scene = eventHandle.Arg1;
        if (scene == EScene.GAME)
        {
            StartCoroutine(DelayedSpawnAllOnSceneChangeCoroutine());
        }
    }

    private IEnumerator DelayedSpawnAllOnSceneChangeCoroutine()
    {
        yield return new WaitForEndOfFrame();

        SpawnAllJoinedPlayers();

        if (MotherOfManagers.Instance.IsSpawnRemainingAIPlayersOnGameStart == false)
        {
            SpawnRemaningAIPlayers();
        }
    }

    private void On_NETWORK_GameStarted(NetEvent_GameStarted eventHandle)
    {
        // Connect and spawn reamining players as AI
        if (MotherOfManagers.Instance.IsSpawnRemainingAIPlayersOnGameStart == true)
        {
            SpawnRemaningAIPlayers();
        }
    }

    public void OnPlayerDead(EPlayerID playerID)
    {
        DestroyPlayer(playerID);
    }

    private void On_RespawnAnimationDone(PlayerRespawnGhost playerRespawnGhost)
    {
        playerRespawnGhost.RespawnAnimationDone -= On_RespawnAnimationDone;

        LogConsole(playerRespawnGhost.PlayerID + " completed respawn animation");
        SpawnPlayer(playerRespawnGhost.PlayerID);
        playerRespawnGhost.DestroyGhost();
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
        PlayersDeathPositions.Clear();
        PlayersDeathRotations.Clear();
        foreach (EPlayerID playerID in Enum.GetValues(typeof(EPlayerID)))
        {
            if (playerID != EPlayerID.NONE)
            {
                PlayersTeam.Add(playerID, ETeamID.NONE);
                PlayersDeathPositions.Add(playerID, Vector3.zero);
                PlayersDeathRotations.Add(playerID, Quaternion.identity);
            }
        }
    }


    private void LoadPlayerResources()
    {
        playersPrefabs.Clear();
        playersPrefabs.Add(EPlayerID.PLAYER_1, Resources.Load<Player>(PATH_PLAYER_RED));
        playersPrefabs.Add(EPlayerID.PLAYER_2, Resources.Load<Player>(PATH_PLAYER_BLUE));
        playersPrefabs.Add(EPlayerID.PLAYER_3, Resources.Load<Player>(PATH_PLAYER_YELLOW));
        playersPrefabs.Add(EPlayerID.PLAYER_4, Resources.Load<Player>(PATH_PLAYER_GREEN));
        IS_NOT_NULL(playersPrefabs[EPlayerID.PLAYER_1]);
        IS_NOT_NULL(playersPrefabs[EPlayerID.PLAYER_2]);
        IS_NOT_NULL(playersPrefabs[EPlayerID.PLAYER_3]);
        IS_NOT_NULL(playersPrefabs[EPlayerID.PLAYER_4]);

        playerSpawnPositionPrefab = Resources.Load<PlayerSpawnPosition>(PATH_PLAYER_SPAWN_POSITION);
        IS_NOT_NULL(playerSpawnPositionPrefab);

        playerRespawnGhostPrefab = Resources.Load<PlayerRespawnGhost>(PATH_PLAYER_RESPAWN_GHOST);
        IS_NOT_NULL(playerRespawnGhostPrefab);
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
                EPlayerID playerID = GetPlayerIDFrom(i);
                if (PlayersSpawnPositions.ContainsKey(playerID) == false)
                {
                    PlayerSpawnPosition spawnGhost = Instantiate(playerSpawnPositionPrefab);
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

        if (!ActivePlayers[playerID].HasCastedSpell)
        {
            ActivePlayers[playerID].HasCastedSpell = true;
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
            yield return new WaitForSeconds(ActivePlayers[playerID].SpellCastDuration[spellSlot]);
            if (ActivePlayers.ContainsKey(playerID))
            {
                ActivePlayers[playerID].IsReadyToShoot = true;
            }

            yield return new WaitForSeconds(ActivePlayers[playerID].SpellCooldown[spellSlot]);

            if (ActivePlayers.ContainsKey(playerID))
            {
                ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = true;
                ActivePlayers[playerID].HasCastedSpell = false;
            }
            
        }

    }



    private void On_INPUT_ControllerConnected(Event_GenericHandle<EControllerID, EPlayerID> eventHandle)
    {
        EControllerID controllerID = eventHandle.Arg1;
        EPlayerID playerID = eventHandle.Arg2;
        AssignPlayerToTeam(playerID, GetIdenticPlayerTeam(playerID));

        // Initialize dictionaries for the new player
        if (IS_KEY_NOT_CONTAINED(playersJoinStatus, playerID))
        {
            playersJoinStatus.Add(playerID, new PlayerJoinStatus());
        }

        if ((MotherOfManagers.Instance.IsJoinAndSpawnPlayerOnControllerConnect == true)
            || (controllerID.ContainedIn(AI_CONTROLLERS)))
        {
            if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID))
            {
                playersJoinStatus[playerID].HasJoined = true;
                playersJoinStatus[playerID].IsReady = true;
                StartCoroutine(DelayedTriggerJoinEvent(playerID));
            }

            if (MotherOfManagers.Instance.IsJoinAndSpawnPlayerOnControllerConnect == true)
            {
                SpawnPlayer(playerID);
            }
        }
    }

    private IEnumerator DelayedTriggerJoinEvent(EPlayerID playerID)
    {
        yield return new WaitForEndOfFrame();

        EventManager.Instance.Invoke_PLAYERS_PlayerJoined(playerID);
    }

    private void On_INPUT_ControllerDisconnected(Event_GenericHandle<EControllerID, EPlayerID> eventHandle)
    {
        EPlayerID playerID = eventHandle.Arg2;
        // TODO
    }


    private void On_PLAYERS_PlayerJoined(EPlayerID playerID)
    {
        if (IS_KEY_CONTAINED(playersJoinStatus, playerID))
        {
            playersJoinStatus[playerID].HasJoined = true;
        }
    }

    private void On_PLAYERS_PlayerLeft(EPlayerID playerID)
    {
        if (IS_KEY_CONTAINED(playersJoinStatus, playerID))
        {
            playersJoinStatus[playerID].HasJoined = false;
            CheckIfAllPlayersAreReady();
        }
    }

    private void On_PLAYERS_PlayerReady(EPlayerID playerID)
    {
        if (IS_KEY_CONTAINED(playersJoinStatus, playerID))
        {
            playersJoinStatus[playerID].IsReady = true;

            CheckIfAllPlayersAreReady();
        }
    }

    private void On_PLAYERS_PlayerCanceledReady(EPlayerID playerID)
    {
        if (IS_KEY_CONTAINED(playersJoinStatus, playerID))
        {
            playersJoinStatus[playerID].IsReady = false;
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




    private void CheckIfAllPlayersAreReady()
    {
        bool areAllReady = false;
        foreach (PlayerJoinStatus playerJoinStatus in playersJoinStatus.Values)
        {
            if (playerJoinStatus.HasJoined == true)
            {
                areAllReady = true;
                if (playerJoinStatus.IsReady == false)
                {
                    areAllReady = false;
                    break;
                }
            }
        }
        if (areAllReady == true)
        {
            EventManager.Instance.Invoke_PLAYERS_AllPlayersReady();
        }
    }


    /// <summary>
    /// Get the player that is connected with the given controller.
    /// Warning! A connected player is not necessarily a player that has joined the game session
    /// </summary>
    private bool IsPlayerConnected(EPlayerID playerID)
    {
        return InputManager.Instance.ConnectedControllers.ContainsValue(playerID);
    }
}
