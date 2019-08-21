using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : AbstractSingletonManager<PlayerManager>
{
    /* Dictionaries that are initialized with all 4 players (weither they are connected or not) */
    /// <summary> Reference to all player prefabs to be spawned. </summary>
    public Dictionary<EPlayerID, Player>                PlayerPrefabs               { get { return playerPrefabs; } }

    /// <summary> Positions in the scene (or around PlayerManager if not found) where the players will be spawned. </summary>
    public Dictionary<EPlayerID, PlayerSpawnPosition>   PlayersSpawnPositions       { get { return playersSpawnPositions; } }

    /// <summary> Controller inputs for every player (movement, rotation and button press/spell casts). </summary>
    public Dictionary<EPlayerID, PlayerInput>           PlayersInput                { get { return playersInput; } }

    /* Dictionaries that are defined only for connected players  */
    /// <summary> Initially initialized with false, then true whenever the respective player connects. </summary>
    public Dictionary<EPlayerID, bool>                  ConnectedPlayers            { get { return connectedPlayers; } }

    /// <summary> Added whenever a player has spawned. Removed when he dies. </summary>
    public Dictionary<EPlayerID, Player>                ActivePlayers               { get { return activePlayers; } }

    /// <summary> All assigned players for every team. </summary>
    public Dictionary<ETeamID, List<EPlayerID>>         Teams                       { get { return teams; } }

    /// <summary> Assigned team of every player </summary>
    public Dictionary<EPlayerID, ETeamID>               PlayersTeam                 { get { return playersTeam; } }


    private Dictionary<EPlayerID, Player>               playerPrefabs               = new Dictionary<EPlayerID, Player>();
    private Dictionary<EPlayerID, PlayerSpawnPosition>  playersSpawnPositions       = new Dictionary<EPlayerID, PlayerSpawnPosition>();
    private Dictionary<EPlayerID, PlayerInput>          playersInput                = new Dictionary<EPlayerID, PlayerInput>();
    private Dictionary<EPlayerID, bool>                 connectedPlayers            = new Dictionary<EPlayerID, bool>();
    private Dictionary<EPlayerID, Player>               activePlayers               = new Dictionary<EPlayerID, Player>();
    private Dictionary<ETeamID, List<EPlayerID>>        teams                       = new Dictionary<ETeamID, List<EPlayerID>>();
    private Dictionary<EPlayerID, ETeamID>              playersTeam                 = new Dictionary<EPlayerID, ETeamID>();


    protected override void Awake()
    {
        base.Awake();

        InitializeDictionaries();
        LoadPlayerResources();
    }


    private void Start()
    {
        // Connect Touch player as player 1
        if ((MotherOfManagers.Instance.InputMode == EInputMode.TOUCH) && (MotherOfManagers.Instance.IsSpawnTouchAsPlayer1 == true))
        {
            InputManager.Instance.TouchPlayerID = Instance.ConnectNextPlayerToController();
        }

        // Input events
        EventManager.Instance.INPUT_ButtonPressed   += On_INPUT_ButtonPressed;
        EventManager.Instance.INPUT_ButtonReleased  += On_INPUT_ButtonReleased;
        EventManager.Instance.INPUT_JoystickMoved   += On_INPUT_JoystickMoved;
       
        // Scene changed event
        EventManager.Instance.APP_SceneChanged.AddListener(On_APP_SceneChanged);
        EventManager.Instance.APP_AppStateUpdated.AddListener(On_APP_AppStateUpdated);

        //Network
        EventManager.Instance.NETWORK_ReceivedGameSessionInfo.AddListener(On_NETWORK_ReceivedGameSessionInfo);
       
        StartCoroutine(LateStartCoroutine());
    }


    private void On_NETWORK_ReceivedGameSessionInfo(BasicEventHandle<List<EPlayerID>, EPlayerID> eventHandle)
    {
        List<EPlayerID> connectedPlayers = eventHandle.Arg1;
        EPlayerID ownPlayer = eventHandle.Arg2;

        if (ownPlayer != EPlayerID.NONE)
        {
            InputManager.Instance.TouchPlayerID = ownPlayer;
        }

        // Connect and spawn player
        foreach (EPlayerID playerID in connectedPlayers)
        {
            ConnectPlayer(playerID);
            SpawnPlayer(playerID);
        }
    }

    public override void Initialize()
    {
        FindPlayerSpawnGhost();

        ActivePlayers.Clear();
    }


    private IEnumerator LateStartCoroutine()
    {
        yield return new WaitForEndOfFrame();

        if ((MotherOfManagers.Instance.IsSpawnAllPlayers == true) && (AppStateManager.Instance.CurrentScene == EScene.GAME))
        {
            SpawnPlayer(EPlayerID.PLAYER_1);
            SpawnPlayer(EPlayerID.PLAYER_2);
            SpawnPlayer(EPlayerID.PLAYER_3);
            SpawnPlayer(EPlayerID.PLAYER_4);
        }
    }


    private void LateUpdate()
    {
        foreach(PlayerInput playerInput in PlayersInput.Values)
        {
            playerInput.Flush();
        }
    }



    #region Players Management
    private void SpawnPlayer(EPlayerID toSpawnPlayerID)
    {
        if ((ConnectedPlayers[toSpawnPlayerID] == true) 
            || ((MotherOfManagers.Instance.IsSpawnAllPlayers == true) 
                && (AppStateManager.Instance.CurrentScene == EScene.GAME)))
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

                if (MotherOfManagers.Instance.IsARGame == true)
                {
                    spawnedPlayer.IsARPlayer = true;
                    spawnedPlayer.transform.parent = playerSpawnPosition.transform.parent;
                    spawnedPlayer.transform.localScale = playerSpawnPosition.transform.localScale;
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

    /// <summary>
    /// Connect new player to a free spot
    /// </summary>
    /// <returns> The ID of the connected player </returns>
    public EPlayerID ConnectNextPlayerToController()
    {
        EPlayerID playerIDToConnect = EPlayerID.NONE;

        if (ConnectedPlayers[EPlayerID.PLAYER_1] == false)
        {
            ConnectedPlayers[EPlayerID.PLAYER_1] = true;
            AssignPlayerToTeam(EPlayerID.PLAYER_1, ETeamID.TEAM_1);
            playerIDToConnect = EPlayerID.PLAYER_1;
        }
        else if (ConnectedPlayers[EPlayerID.PLAYER_2] == false)
        {
            ConnectedPlayers[EPlayerID.PLAYER_2] = true;
            AssignPlayerToTeam(EPlayerID.PLAYER_2, ETeamID.TEAM_2);
            playerIDToConnect = EPlayerID.PLAYER_2;
        }
        else if (ConnectedPlayers[EPlayerID.PLAYER_3] == false)
        {
            ConnectedPlayers[EPlayerID.PLAYER_3] = true;
            AssignPlayerToTeam(EPlayerID.PLAYER_3, ETeamID.TEAM_3);
            playerIDToConnect = EPlayerID.PLAYER_3;
        }
        else if (ConnectedPlayers[EPlayerID.PLAYER_4] == false)
        {
            ConnectedPlayers[EPlayerID.PLAYER_4] = true;
            AssignPlayerToTeam(EPlayerID.PLAYER_4, ETeamID.TEAM_4);
            playerIDToConnect = EPlayerID.PLAYER_4;
        }

        if (playerIDToConnect != EPlayerID.NONE)
        {
            EventManager.Instance.Invoke_PLAYERS_PlayerConnected(playerIDToConnect);

            if ((MotherOfManagers.Instance.IsSpawnPlayerOnConnect == true) && (AppStateManager.Instance.CurrentScene == EScene.GAME))
            {
                SpawnPlayer(playerIDToConnect);
            }

            return playerIDToConnect;
        }
        else
        {
            Debug.Log("Can't connect new player. All 4 players are already connected");
            return 0;
        }
    }

    private void ConnectPlayer(EPlayerID playerID)
    {
        if (ConnectedPlayers[playerID] == false)
        {
            ConnectedPlayers[playerID] = true;
            AssignPlayerToTeam(EPlayerID.PLAYER_1, ETeamID.TEAM_1);
        }
    }

    public void DisconnectPlayer(EPlayerID playerID)
    {
        if (IsPlayerConnected(playerID) == true)
        {
            EventManager.Instance.Invoke_PLAYERS_PlayerDisconnected(playerID);
            ConnectedPlayers[playerID] = false;
        }
        else
        {
            Debug.LogError("Trying to disconnect a player that is not connected");
        }
    }
    #endregion

    #region Input



    private void On_INPUT_ButtonPressed(EInputButton inputButton, EPlayerID playerID)
    {
        if (playerID == EPlayerID.TEST) return;

        ESpellID spellID = MaleficusUtilities.GetSpellIDFrom(inputButton);
        if (spellID == ESpellID.NONE)
        {
            return;
        }
        
        if (ActivePlayers[playerID].ReadyToUseSpell[spellID] && ActivePlayers[playerID].IsReadyToShoot)
        {
            EMovementType movementType = SpellManager.Instance.Player_Spells[playerID][spellID].MovementType;

            ActivePlayers[playerID].StartChargingSpell(spellID, movementType);
            ActivePlayers[playerID].IsPlayerCharging = true;
        }
    }

    private void On_INPUT_JoystickMoved(EInputAxis axisType, float axisValue, EPlayerID playerID)
    {
        if (playerID == EPlayerID.TEST) return;

        switch (axisType)
        {
            case EInputAxis.MOVE_X:
                PlayersInput[playerID].Move_X = axisValue;
                break;

            case EInputAxis.MOVE_Y:
                PlayersInput[playerID].Move_Y = axisValue;
                break;

            case EInputAxis.ROTATE_X:
                PlayersInput[playerID].Rotate_X = axisValue;
                break;

            case EInputAxis.ROTATE_Y:
                PlayersInput[playerID].Rotate_Y = axisValue;
                break;
        }
    }

    private void On_INPUT_ButtonReleased(EInputButton inputButton, EPlayerID playerID)
    {
        ESpellID spellID = MaleficusUtilities.GetSpellIDFrom(inputButton);

        Debug.Log(spellID + " by " + playerID + " > IsReadyToShoot : " + ActivePlayers[playerID].IsReadyToShoot + " > ReadyToUseSpell : " + ActivePlayers[playerID].ReadyToUseSpell[spellID]);

        if (ActivePlayers[playerID].IsReadyToShoot && ActivePlayers[playerID].ReadyToUseSpell[spellID])
        {
            ActivePlayers[playerID].IsReadyToShoot = false;
            ActivePlayers[playerID].ReadyToUseSpell[spellID] = false;
            ActivePlayers[playerID].StopChargingSpell(spellID);
          
            SpellManager.Instance.CastSpell(playerID, spellID, ActivePlayers[playerID].SpellChargingLVL);
            
            StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellID));
        }
    }

    

    
    #endregion


    #region Events Callbacks
    private void On_APP_SceneChanged(BasicEventHandle<EScene> eventHandle)
    {
        if (eventHandle.Arg1 == EScene.GAME)
        {
            FindPlayerSpawnGhost();
        }
    }

    private void On_APP_AppStateUpdated(StateUpdatedEventHandle<EAppState> eventHandle)
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
        PlayersInput.Clear();
        PlayersInput[EPlayerID.PLAYER_1] = new PlayerInput();
        PlayersInput[EPlayerID.PLAYER_2] = new PlayerInput();
        PlayersInput[EPlayerID.PLAYER_3] = new PlayerInput();
        PlayersInput[EPlayerID.PLAYER_4] = new PlayerInput();

        ConnectedPlayers.Clear();
        ConnectedPlayers[EPlayerID.PLAYER_1] = false;
        ConnectedPlayers[EPlayerID.PLAYER_2] = false;
        ConnectedPlayers[EPlayerID.PLAYER_3] = false;
        ConnectedPlayers[EPlayerID.PLAYER_4] = false;

        Teams.Clear();
        Teams[ETeamID.TEAM_1] = new List<EPlayerID>();
        Teams[ETeamID.TEAM_2] = new List<EPlayerID>();
        Teams[ETeamID.TEAM_3] = new List<EPlayerID>();
        Teams[ETeamID.TEAM_4] = new List<EPlayerID>();

        playersTeam.Clear();
        playersTeam[EPlayerID.PLAYER_1] = ETeamID.NONE;
        playersTeam[EPlayerID.PLAYER_2] = ETeamID.NONE;
        playersTeam[EPlayerID.PLAYER_3] = ETeamID.NONE;
        playersTeam[EPlayerID.PLAYER_4] = ETeamID.NONE;
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
                    if (MotherOfManagers.Instance.IsARGame == true)
                    {
                        spawnGhost.transform.localScale *= ARManager.Instance.SizeFactor;
                    }
                    PlayersSpawnPositions.Add(playerID, spawnGhost);
                }
            }
        }
    }
#endregion


    private void SpawnAllConnectedPlayers()
    {
        foreach (EPlayerID playerID in ConnectedPlayers.Keys)
        {
            SpawnPlayer(playerID);
        }
    }

    /* public player information getter functions */
    public bool IsPlayerConnected(EPlayerID playerID)
    {
        return ConnectedPlayers[playerID];
    }

    public PlayerInput GetPlayerInput (EPlayerID playerID)
    {
        return PlayersInput[playerID];
    }

    public bool IsPlayerActive(EPlayerID playerID)
    {
        return ActivePlayers.ContainsKey(playerID);
    }

    public EPlayerID[] GetPlayersInTeam(ETeamID inTeamID)
    {
        return Teams[inTeamID].ToArray();
    }

    private IEnumerator SetReadyToUseSpellCoroutine(EPlayerID playerID, ESpellID spellID)                 
    {
        yield return new WaitForSeconds(ActivePlayers[playerID].SpellDuration[spellID]);
        ActivePlayers[playerID].IsReadyToShoot = true;

        yield return new WaitForSeconds(ActivePlayers[playerID].SpellCooldown[spellID]);
        ActivePlayers[playerID].ReadyToUseSpell[spellID] = true;

        
    }
}
