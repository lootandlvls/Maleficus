using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : AbstractSingletonManager<PlayerManager>
{

    public Dictionary<EPlayerID, Player>                PlayerPrefabs               { get { return playerPrefabs; } }
    public Dictionary<EPlayerID, PlayerSpawnPosition>   PlayersSpawnPositions       { get { return playersSpawnGhosts; } }
    public Dictionary<EPlayerID, PlayerInput>           PlayersInput                { get { return playersInput; } }
    public Dictionary<EPlayerID, bool>                  ConnectedPlayers            { get { return connectedPlayers; } }
    public Dictionary<EPlayerID, Player>                ActivePlayers               { get { return activePlayers; } }
    public Dictionary<ETeamID, List<EPlayerID>>         PlayerTeams                 { get { return playerTeams; } }

    
    /* Dictionaries that are initialized with all 4 players (weither they are connected or not) */
    private Dictionary<EPlayerID, Player>               playerPrefabs               = new Dictionary<EPlayerID, Player>();
    private Dictionary<EPlayerID, PlayerSpawnPosition>  playersSpawnGhosts          = new Dictionary<EPlayerID, PlayerSpawnPosition>();
    private Dictionary<EPlayerID, PlayerInput>          playersInput                = new Dictionary<EPlayerID, PlayerInput>();
    /* Dictionaries that are defined only for active players  */
    /// Added whenever a player has spawned. Removed when he dies.
    /// Initially initialized with false, then true whenever the respective player connects
    private Dictionary<EPlayerID, bool>                 connectedPlayers            = new Dictionary<EPlayerID, bool>();

    private Dictionary<EPlayerID, Player>               activePlayers               = new Dictionary<EPlayerID, Player>();
    private Dictionary<ETeamID, List<EPlayerID>>        playerTeams                 = new Dictionary<ETeamID, List<EPlayerID>>();

 


    protected override void Awake()
    {
        base.Awake();

        InitializeDictionaries();
        LoadPlayerResources();
    }


    private void Start()
    {
        // Input events
        EventManager.Instance.INPUT_ButtonPressed   += On_INPUT_ButtonPressed;
        EventManager.Instance.INPUT_ButtonReleased  += On_INPUT_ButtonReleased;
        EventManager.Instance.INPUT_JoystickMoved   += On_INPUT_JoystickMoved;
       
        // Scene changed event
        EventManager.Instance.APP_SceneChanged += On_APP_SceneChanged;
        EventManager.Instance.APP_AppStateUpdated += On_ÁPP_AppStateUpdated;
       
        StartCoroutine(LateStartCoroutine());
    }


    public override void Initialize()
    {
        FindPlayerSpawnGhost();

        activePlayers.Clear();
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


    private void Update()
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)  
        {
            MoveAndRotatePlayers();                                                             // TODO: Remove this and let players move themselves from player input
        }
    }


    // TODO: Use this 
    public void AssignPlayerToTeam(EPlayerID playerID, ETeamID teamID)
    {
        if (playerTeams[teamID].Contains(playerID))
        {
            playerTeams[teamID].Remove(playerID);
        }
        playerTeams[teamID].Add(playerID);
          
    }
    

    private void SpawnPlayer(EPlayerID toSpawnPlayerID)
    {
        if ((connectedPlayers[toSpawnPlayerID] == true) || ((MotherOfManagers.Instance.IsSpawnAllPlayers == true) && (AppStateManager.Instance.CurrentScene == EScene.GAME)))
        {
            if (activePlayers.ContainsKey(toSpawnPlayerID) == false)
            {
                Player playerPrefab = playerPrefabs[toSpawnPlayerID];
                PlayerSpawnPosition playerSpawnPosition = playersSpawnGhosts[toSpawnPlayerID];
                Vector3 playerPosition = playerSpawnPosition.Position;
                Quaternion playerRotation = playerSpawnPosition.Rotation;

                Player spawnedPlayer = Instantiate(playerPrefab, playerPosition, playerRotation);
                spawnedPlayer.PlayerID = toSpawnPlayerID;
                spawnedPlayer.TeamID = GetPlayerTeamID(toSpawnPlayerID);

                activePlayers.Add(toSpawnPlayerID, spawnedPlayer);

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


    #region Input
    private void On_INPUT_ButtonReleased(EInputButton inputButton, EPlayerID playerID)
    {
      
        switch (inputButton)
        {

            case EInputButton.CAST_SPELL_1:
                if (activePlayers[playerID].readyToShoot && activePlayers[playerID].readyToUseSpell_1)
                {
                    activePlayers[playerID].readyToShoot = false;
                    activePlayers[playerID].readyToUseSpell_1 = false;
                    activePlayers[playerID].StopChargingSpell_1();
                    
                    SpellManager.Instance.CastSpell(playerID, 0);
                    StartCoroutine(ReadyToUseSpell(playerID,activePlayers[playerID].spellDuration_1, 0));
                    StartCoroutine(ReadyToUseSpell(playerID,activePlayers[playerID].spellCooldown_1 + activePlayers[playerID].spellDuration_1, 1));
                }
                break;

            case EInputButton.CAST_SPELL_2:
                if (activePlayers[playerID].readyToShoot && activePlayers[playerID].readyToUseSpell_2)
                {
                    activePlayers[playerID].readyToShoot = false;
                    activePlayers[playerID].readyToUseSpell_2 = false;
                   
                    activePlayers[playerID].StopChargingSpell_2();
                    SpellManager.Instance.CastSpell(playerID, 1);
                    StartCoroutine(ReadyToUseSpell(playerID, activePlayers[playerID].spellDuration_2, 0));
                    StartCoroutine(ReadyToUseSpell(playerID, activePlayers[playerID].spellCooldown_2 + activePlayers[playerID].spellDuration_2, 2));
                }
                break;

            case EInputButton.CAST_SPELL_3:
                if (activePlayers[playerID].readyToShoot && activePlayers[playerID].readyToUseSpell_3)
                {
                    activePlayers[playerID].readyToShoot = false;
                    activePlayers[playerID].readyToUseSpell_3 = false;
                  
                    activePlayers[playerID].StopChargingSpell_3();
                    SpellManager.Instance.CastSpell(playerID, 2);
                    StartCoroutine(ReadyToUseSpell(playerID, activePlayers[playerID].spellDuration_3, 0));
                    StartCoroutine(ReadyToUseSpell(playerID, activePlayers[playerID].spellCooldown_3 + activePlayers[playerID].spellDuration_3 , 3));
                }
                break;
        }
    }

    private void On_INPUT_ButtonPressed(EInputButton inputButton, EPlayerID playerID)
    {
        if (playerID == EPlayerID.TEST) return;

        switch (inputButton)
        {
            case EInputButton.CAST_SPELL_1:
                //change to  SpellManager.Instance.Player_Spells[playerID][1].MovementType; when dictionary are ready to use
                if ( activePlayers[playerID].readyToUseSpell_1 && activePlayers[playerID].readyToShoot)             
                {
                

                    MovementType movementType = SpellManager.Instance.Player_Spells[playerID][0].MovementType;
                   
                    activePlayers[playerID].StartChargingSpell_1(movementType);
                    activePlayers[playerID].playerCharging = true;
                }
               
                break;

            case EInputButton.CAST_SPELL_2:
               
                if ( activePlayers[playerID].readyToUseSpell_2 && activePlayers[playerID].readyToShoot)
                {
                    
                    MovementType movementType = SpellManager.Instance.Player_Spells[playerID][1].MovementType;
                  
                    activePlayers[playerID].StartChargingSpell_2(movementType);
                    activePlayers[playerID].playerCharging = true;
                }
               
                break;

            case EInputButton.CAST_SPELL_3:
              
                if ( activePlayers[playerID].readyToUseSpell_3 && activePlayers[playerID].readyToShoot)
                {
                   
                    MovementType movementType = SpellManager.Instance.Player_Spells[playerID][2].MovementType;
                  
                    activePlayers[playerID].StartChargingSpell_3(movementType);
                    activePlayers[playerID].playerCharging = true;
                }
               

                break;
        }
    }

    private void On_INPUT_JoystickMoved(EInputAxis axisType, float axisValue, EPlayerID playerID)
    {
        if (playerID == EPlayerID.TEST) return;
        switch (axisType)
        {
            case EInputAxis.MOVE_X:
                playersInput[playerID].Move_X = axisValue;
                break;

            case EInputAxis.MOVE_Y:
                playersInput[playerID].Move_Y = axisValue;
                break;

            case EInputAxis.ROTATE_X:
                playersInput[playerID].Rotate_X = axisValue;
                break;

            case EInputAxis.ROTATE_Y:
                playersInput[playerID].Rotate_Y = axisValue;
                break;
        }
    }


    /// <summary>
    /// Connect new player to a free spot
    /// </summary>
    /// <returns> The ID of the connected player </returns>
    public EPlayerID ConnectNextPlayerToController()
    {
        EPlayerID playerIDToConnect = EPlayerID.NONE;

        if (connectedPlayers[EPlayerID.PLAYER_1] == false)
        {
            connectedPlayers[EPlayerID.PLAYER_1] = true;
            playerIDToConnect = EPlayerID.PLAYER_1;
        }
        else if (connectedPlayers[EPlayerID.PLAYER_2] == false)
        {
            connectedPlayers[EPlayerID.PLAYER_2] = true;
            playerIDToConnect = EPlayerID.PLAYER_2;
        }
        else if (connectedPlayers[EPlayerID.PLAYER_3] == false)
        {
            connectedPlayers[EPlayerID.PLAYER_3] = true;
            playerIDToConnect = EPlayerID.PLAYER_3;
        }
        else if (connectedPlayers[EPlayerID.PLAYER_4] == false)
        {
            connectedPlayers[EPlayerID.PLAYER_4] = true;
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
    public void DisconnectPlayer(EPlayerID playerID)
    {
        if (IsPlayerConnected(playerID) == true)
        {
            EventManager.Instance.Invoke_PLAYERS_PlayerDisconnected(playerID);
            connectedPlayers[playerID] = false;
                                                                // TODO: Destroy only in connection menu
        }
        else
        {
            Debug.LogError("Trying to disconnect a player that is not connected");
        }
    }

    private void MoveAndRotatePlayers()
    {
        for (int i = 1; i < 5; i++)
        {
            EPlayerID playerID = MaleficusTypes.IntToPlayerID(i);
            if ((IsPlayerConnected(playerID) == true) && (IsPlayerActive(playerID) ==  true))
            {

                PlayerInput playerInput = playersInput[playerID];
                Player player = activePlayers[playerID];
                if (playerInput.HasMoved())
                {

                    player.Move(playerInput.Move_X, playerInput.Move_Y);
                }
                if (playerInput.HasRotated())
                {
                    player.Rotate(playerInput.Rotate_X, playerInput.Rotate_Y);
                }

                playerInput.Flush();
            }
        }
    }
    #endregion


    #region Events Callbacks
    private void On_APP_SceneChanged(EScene newScene)
    {
        if (newScene == EScene.GAME)
        {
            FindPlayerSpawnGhost();
        }
    }

    private void On_ÁPP_AppStateUpdated(EAppState newState, EAppState lastState)
    {
        if (newState == EAppState.IN_GAME_IN_NOT_STARTED)
        {
            SpawnAllConnectedPlayers();
        }
    }
    #endregion

    private void InitializeDictionaries()
    {
        playersInput.Clear();
        playersInput[EPlayerID.PLAYER_1] = new PlayerInput();
        playersInput[EPlayerID.PLAYER_2] = new PlayerInput();
        playersInput[EPlayerID.PLAYER_3] = new PlayerInput();
        playersInput[EPlayerID.PLAYER_4] = new PlayerInput();

        connectedPlayers.Clear();
        connectedPlayers[EPlayerID.PLAYER_1] = false;
        connectedPlayers[EPlayerID.PLAYER_2] = false;
        connectedPlayers[EPlayerID.PLAYER_3] = false;
        connectedPlayers[EPlayerID.PLAYER_4] = false;

        playerTeams.Clear();
        playerTeams[ETeamID.TEAM_1] = new List<EPlayerID>() { EPlayerID.PLAYER_1 };
        playerTeams[ETeamID.TEAM_2] = new List<EPlayerID>() { EPlayerID.PLAYER_2 };
        playerTeams[ETeamID.TEAM_3] = new List<EPlayerID>() { EPlayerID.PLAYER_3 };
        playerTeams[ETeamID.TEAM_4] = new List<EPlayerID>() { EPlayerID.PLAYER_4 };
    }

    

    private void LoadPlayerResources()
    {
        playerPrefabs.Clear();
        playerPrefabs.Add(EPlayerID.PLAYER_1, Resources.Load<Player>(MaleficusTypes.PATH_PLAYER_RED));
        playerPrefabs.Add(EPlayerID.PLAYER_2, Resources.Load<Player>(MaleficusTypes.PATH_PLAYER_BLUE));
        playerPrefabs.Add(EPlayerID.PLAYER_3, Resources.Load<Player>(MaleficusTypes.PATH_PLAYER_YELLOW));
        playerPrefabs.Add(EPlayerID.PLAYER_4, Resources.Load<Player>(MaleficusTypes.PATH_PLAYER_GREEN));
    }

    private void FindPlayerSpawnGhost()
    {
        playersSpawnGhosts.Clear();

        // Try to find already placed player spawn positions in the scene
        PlayerSpawnPosition[] spawnPositions = FindObjectsOfType<PlayerSpawnPosition>();
        foreach (PlayerSpawnPosition spawnPosition in spawnPositions)
        {
            playersSpawnGhosts.Add(spawnPosition.ToSpawnPlayerID, spawnPosition);
        }

        // Determine spawn positions relative to this transform if no PlayerSpawnPosition found in scene
        if (MotherOfManagers.Instance.IsSpawnGhostPlayerPositionsIfNotFound == true)
        {
            int angle;
            for (int i = 1; i < 5; i++)
            {
                angle = 90 * i;
                EPlayerID playerID = MaleficusTypes.IntToPlayerID(i);
                if (playersSpawnGhosts.ContainsKey(playerID) == false)
                {
                    PlayerSpawnPosition spawnGhost = Instantiate(Resources.Load<PlayerSpawnPosition>(MaleficusTypes.PATH_PLAYER_SPAWN_POSITION));
                    spawnGhost.ToSpawnPlayerID = playerID;
                    spawnGhost.Position = transform.position + Vector3.forward * 3.0f + Vector3.left * 3.0f;
                    spawnGhost.transform.RotateAround(transform.position, Vector3.up, angle);
                    spawnGhost.Rotation = transform.rotation;
                    if (MotherOfManagers.Instance.IsARGame == true)
                    {
                        spawnGhost.transform.localScale *= ARManager.Instance.SizeFactor;
                    }
                    playersSpawnGhosts.Add(playerID, spawnGhost);
                }
            }
        }
    }

    private void SpawnAllConnectedPlayers()
    {
        foreach (EPlayerID playerID in connectedPlayers.Keys)
        {
            SpawnPlayer(playerID);
        }
    }

    /* public player information getter functions */
    public bool IsPlayerConnected(EPlayerID playerID)
    {
        return connectedPlayers[playerID];
    }

    public PlayerInput GetPlayerInput (EPlayerID playerID)
    {
        return playersInput[playerID];
    }

    public bool IsPlayerActive(EPlayerID playerID)
    {
        return activePlayers.ContainsKey(playerID);
    }

    public ETeamID GetPlayerTeamID(EPlayerID playerID)
    {
        ETeamID result = ETeamID.NONE;
        foreach (ETeamID teamID in PlayerTeams.Keys)
        {
            if (PlayerTeams[teamID].Contains(playerID))
            {
                result = teamID;
            }
        }
        return result;
    }

    public EPlayerID[] GetPlayersInTeam(ETeamID inTeamID)
    {
        return PlayerTeams[inTeamID].ToArray();
    }


    IEnumerator ReadyToUseSpell(EPlayerID playerID, float time, int id)
    {
        switch (id)
        {
            case 0:
                yield return new WaitForSeconds(time);
                activePlayers[playerID].readyToShoot = true;

                break;
            case 1:
                yield return new WaitForSeconds(time);
                activePlayers[playerID].readyToUseSpell_1 = true;

                break;

            case 2:
                yield return new WaitForSeconds(time);
                activePlayers[playerID].readyToUseSpell_2 = true;

                break;

            case 3:
                yield return new WaitForSeconds(time);
                activePlayers[playerID].readyToUseSpell_3 = true;
                break;

        }


        Debug.Log("ready to use the spell again");
    }
}
