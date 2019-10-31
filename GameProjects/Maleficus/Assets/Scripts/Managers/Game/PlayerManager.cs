using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

                                                                                                                                        
public class PlayerManager : AbstractSingletonManager<PlayerManager>
{
    /* Dictionaries that are initialized with all 4 players (weither they are connected or not) */
    /// <summary> Reference to all player prefabs to be spawned. </summary>
    public Dictionary<EPlayerID, Player>                PlayerPrefabs               { get { return playerPrefabs; } }

    /// <summary> Positions in the scene (or around PlayerManager if not found) where the players will be spawned. </summary>
    public Dictionary<EPlayerID, PlayerSpawnPosition>   PlayersSpawnPositions       { get { return playersSpawnPositions; } }


    /* Dictionaries that are defined only for connected players  */
    /// <summary> Added whenever a player has spawned. Removed when he dies. </summary>
    public Dictionary<EPlayerID, Player>                ActivePlayers               { get { return activePlayers; } }

    /// <summary> All assigned players for every team. </summary>
    public Dictionary<ETeamID, List<EPlayerID>>         Teams                       { get { return teams; } }

    /// <summary> Assigned team of every player </summary>
    public Dictionary<EPlayerID, ETeamID>               PlayersTeam                 { get { return playersTeam; } }
    public EPlayerID OwnPlayerID { get { return MaleficusUtilities.GetPlayerIDFrom(NetworkManager.Instance.OwnClientID); } }

    /// <summary> Joysticks inputs for every player (movement, rotation). </summary>
    public Dictionary<EPlayerID, JoystickInput>         PlayersMovement             { get { return playersMovement; } }



    private Dictionary<EPlayerID, Player>               playerPrefabs               = new Dictionary<EPlayerID, Player>();
    private Dictionary<EPlayerID, PlayerSpawnPosition>  playersSpawnPositions       = new Dictionary<EPlayerID, PlayerSpawnPosition>();
    private Dictionary<EPlayerID, Player>               activePlayers               = new Dictionary<EPlayerID, Player>();
    private Dictionary<ETeamID, List<EPlayerID>>        teams                       = new Dictionary<ETeamID, List<EPlayerID>>();
    private Dictionary<EPlayerID, ETeamID>              playersTeam                 = new Dictionary<EPlayerID, ETeamID>();
    private Dictionary<EPlayerID, JoystickInput>        playersMovement             = new Dictionary<EPlayerID, JoystickInput>();


                                                                                                                                    // TODO: Add a list of active Coroutines for every player to stop when he dies
    protected override void Awake()
    {
        base.Awake();

        InitializeDictionaries();
        LoadPlayerResources();
    }


    private void Start()
    {
        // Input events
        EventManager.Instance.INPUT_ControllerConnected.AddListener             (On_INPUT_ControllerConnected);            
        EventManager.Instance.INPUT_ControllerDisconnected.AddListener          (On_INPUT_ControllerDisconnected); 
        
        EventManager.Instance.INPUT_ButtonPressed.AddListener                   (On_INPUT_ButtonPressed);
        EventManager.Instance.INPUT_ButtonReleased.AddListener                  (On_INPUT_ButtonReleased);
        //EventManager.Instance.INPUT_JoystickMoved.AddListener                   (On_INPUT_JoystickMoved);
        // Listen to broadcasted inputs 
        EventManager.Instance.INPUT_JoystickMoved.AddListener                   (On_SERVER_INPUT_JoystickMoved);

        // Scene changed event
        EventManager.Instance.APP_SceneChanged.AddListener                      (On_APP_SceneChanged);
        EventManager.Instance.APP_AppStateUpdated.AddListener                   (On_APP_AppStateUpdated);

        //Network
        //EventManager.Instance.NETWORK_ReceivedGameSessionInfo.AddListener       (On_NETWORK_ReceivedGameSessionInfo);
        EventManager.Instance.NETWORK_GameStateReplicate.AddListener            (On_NETWORK_GameStateReplicate);
        EventManager.Instance.NETWORK_GameStarted.AddListener                   (On_NETWORK_GameStarted);

    }
    


    private void Update()
    {
        //UpdateControllersInput();

    }

    private void On_NETWORK_GameStateReplicate(NetEvent_GameStateReplicate eventHandle)
    {
        EPlayerID playerID      = eventHandle.UpdatedPlayerID;
        float[] playerPosition  = eventHandle.playerPosition;

        if (activePlayers.ContainsKey(playerID))
        {
            activePlayers[playerID].transform.localPosition = new Vector3(playerPosition[0], playerPosition[1], playerPosition[2]);
        }
    }

    private void On_NETWORK_GameStarted(NetEvent_GameStarted eventHandle)
    {
        SpawnAllConnectedPlayers();
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
            || ((MotherOfManagers.Instance.IsSpawnAllPlayers == true) 
                && (AppStateManager.Instance.CurrentScene.ContainedIn(MaleficusConsts.GAME_SCENES))))
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
        ETeamID oldTeamID = playersTeam[playerID];
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
            EPlayerID currentPlayerID = MaleficusUtilities.IntToPlayerID(i);
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
        EPlayerID playerID = MaleficusUtilities.GetPlayerIDFrom(eventHandle.SenderID);

        ESpellSlot spellSlot = MaleficusUtilities.GetSpellSlotFrom(inputButton);
        if ((spellSlot == ESpellSlot.NONE) || (playerID == EPlayerID.TEST) || (activePlayers.ContainsKey(playerID) == false))
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
        else
        {
            ActivePlayers[playerID].StartChargingSpell(spell, spellSlot);
        }

    }

    private void On_INPUT_ButtonReleased(NetEvent_ButtonReleased eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EPlayerID playerID = MaleficusUtilities.GetPlayerIDFrom(eventHandle.SenderID);

        ESpellSlot spellSlot = MaleficusUtilities.GetSpellSlotFrom(inputButton);
        if ((spellSlot == ESpellSlot.NONE) || (activePlayers.ContainsKey(playerID) == false))
        {
            return;
        }

        ISpell spell = SpellManager.Instance.Player_Spells[playerID][spellSlot];

        if (spell.MovementType != ESpellMovementType.LINEAR_LASER)
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
        EPlayerID playerID = MaleficusUtilities.GetPlayerIDFrom(eventHandle.SenderID);

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
        if (newScene.ContainedIn(MaleficusConsts.GAME_SCENES))
        {
            FindPlayerSpawnGhost();
        }
    }

    private void On_APP_AppStateUpdated(Event_StateUpdated<EAppState> eventHandle)
    {
        if (eventHandle.NewState == EAppState.IN_GAME_IN_NOT_STARTED)
        {
            SpawnAllConnectedPlayers();
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

        playersTeam.Clear();
        playersTeam[EPlayerID.PLAYER_1] = ETeamID.NONE;
        playersTeam[EPlayerID.PLAYER_2] = ETeamID.NONE;
        playersTeam[EPlayerID.PLAYER_3] = ETeamID.NONE;
        PlayersTeam[EPlayerID.PLAYER_4] = ETeamID.NONE;
}


    private void LoadPlayerResources()
    {
        PlayerPrefabs.Clear();
        PlayerPrefabs.Add(EPlayerID.PLAYER_1, Resources.Load<Player>(MaleficusConsts.PATH_PLAYER_RED));
        PlayerPrefabs.Add(EPlayerID.PLAYER_2, Resources.Load<Player>(MaleficusConsts.PATH_PLAYER_BLUE));
        PlayerPrefabs.Add(EPlayerID.PLAYER_3, Resources.Load<Player>(MaleficusConsts.PATH_PLAYER_YELLOW));
        PlayerPrefabs.Add(EPlayerID.PLAYER_4, Resources.Load<Player>(MaleficusConsts.PATH_PLAYER_GREEN));
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
                EPlayerID playerID = MaleficusUtilities.IntToPlayerID(i);
                if (PlayersSpawnPositions.ContainsKey(playerID) == false)
                {
                    PlayerSpawnPosition spawnGhost = Instantiate(Resources.Load<PlayerSpawnPosition>(MaleficusConsts.PATH_PLAYER_SPAWN_POSITION));
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


    private void SpawnAllConnectedPlayers()
    {
        foreach (EPlayerID playerID in GetConnectedPlayers())
        {
            SpawnPlayer(playerID);
        }
    }

    public JoystickInput GetPlayerInput(EPlayerID playerID)
    {
        if (PlayersMovement.ContainsKey(playerID) == false)
        {
            if (MotherOfManagers.Instance.IsSpawnAllPlayers == false)
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

    private IEnumerator SetReadyToUseSpellCoroutine(EPlayerID playerID, ESpellSlot spellSlot)                 
    {
        if (ActivePlayers.ContainsKey(playerID))
        {
            yield return new WaitForSeconds(ActivePlayers[playerID].SpellDuration[spellSlot]);
            ActivePlayers[playerID].IsReadyToShoot = true;

            yield return new WaitForSeconds(ActivePlayers[playerID].SpellCooldown[spellSlot]);
            ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = true;
        }

    }

    public void OnPlayerOutOfBound(EPlayerID playerID)
    {
        if (activePlayers.ContainsKey(playerID))
        {
            StartCoroutine(DestroyPlayerCoroutine(playerID));
        }
    }

    private IEnumerator DestroyPlayerCoroutine(EPlayerID playerID)
    {
        yield return new WaitForSeconds(MaleficusConsts.PLAYER_FALLING_TIME);
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
        EPlayerID playerID = eventHandle.Arg2;
        AssignPlayerToTeam(playerID, MaleficusUtilities.GetIdenticPlayerTeam(playerID));

        if (PlayersMovement.ContainsKey(playerID) == false)
        {
            // Initialize player movement for the new player
            PlayersMovement.Add(playerID, new JoystickInput());
        }
        else
        {
            Debug.LogError("Connecting a player that is already connected");
        }

        // Spawn player On Connect?
        if ((MotherOfManagers.Instance.IsSpawnPlayerOnConnect == true)
            && (ActivePlayers.ContainsKey(playerID) == false)
            && (AppStateManager.Instance.CurrentScene.ContainedIn(MaleficusConsts.GAME_SCENES)))
        {
            SpawnPlayer(EPlayerID.PLAYER_1);
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

   
    public EPlayerID[] GetConnectedPlayers()
    {
        return InputManager.Instance.ConnectedControllers.Values.ToArray<EPlayerID>();
    }

    public bool IsPlayerConnected(EPlayerID playerID)
    {
        return InputManager.Instance.ConnectedControllers.ContainsValue(playerID);
    }
}
