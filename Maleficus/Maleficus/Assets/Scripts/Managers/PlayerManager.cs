using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : Singleton<PlayerManager>
{

    public Dictionary<EPlayerID, Player>                PlayerPrefabs               { get { return playerPrefabs; } }
    public Dictionary<EPlayerID, PlayerSpawnPosition>   PlayersSpawnPositions       { get { return playersSpawnPositions; } }
    public Dictionary<EPlayerID, PlayerInput>           PlayersInput                { get { return playersInput; } }
    public Dictionary<EPlayerID, bool>                  ConnectedPlayers            { get { return connectedPlayers; } }
    public Dictionary<EPlayerID, Player>                ActivePlayers               { get { return activePlayers; } }
    public Dictionary<ETeamID, List<EPlayerID>>         PlayerTeams                 { get { return playerTeams; } }



    /* Dictionaries that are initialized with all 4 players (weither they are connected or not) */
    private Dictionary<EPlayerID, Player>               playerPrefabs               = new Dictionary<EPlayerID, Player>();
    private Dictionary<EPlayerID, PlayerSpawnPosition>  playersSpawnPositions       = new Dictionary<EPlayerID, PlayerSpawnPosition>();
    private Dictionary<EPlayerID, PlayerInput>          playersInput                = new Dictionary<EPlayerID, PlayerInput>();
    /// Initially initialized with false, then true whenever the respective player connects
    private Dictionary<EPlayerID, bool>                 connectedPlayers            = new Dictionary<EPlayerID, bool>();

    /* Dictionaries that are defined only for active players  */
    /// Added whenever a player has spawned. Removed when he dies.
    private Dictionary<EPlayerID, Player>               activePlayers               = new Dictionary<EPlayerID, Player>();
    private Dictionary<ETeamID, List<EPlayerID>>        playerTeams                 = new Dictionary<ETeamID, List<EPlayerID>>();

   

    protected override void Awake()
    {
        base.Awake();

        InitializeDictionaries();
        LoadPlayerResources();
        FindPlayerSpawnPositions();
    }


    private void Start()
    {
        // Input events
        EventManager.Instance.INPUT_ButtonPressed += On_INPUT_ButtonPressed;
        EventManager.Instance.INPUT_JoystickMoved += On_INPUT_JoystickMoved;
       
        StartCoroutine(LateStartCoroutine());
    }
   


    private IEnumerator LateStartCoroutine()
    {
        yield return new WaitForEndOfFrame();

        if (MotherOfManagers.Instance.IsSpawnAllPlayers == true)
        {
            SpawnPlayer(EPlayerID.PLAYER_1);
            SpawnPlayer(EPlayerID.PLAYER_2);
            SpawnPlayer(EPlayerID.PLAYER_3);
            SpawnPlayer(EPlayerID.PLAYER_4);
        }
    }

    
    private void Update()
    {
        MoveAndRotatePlayers();
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
        if ((connectedPlayers[toSpawnPlayerID] == true) || (MotherOfManagers.Instance.IsSpawnAllPlayers == true))
        {
            if (activePlayers.ContainsKey(toSpawnPlayerID) == false)
            {
                Player playerPrefab = playerPrefabs[toSpawnPlayerID];
                PlayerSpawnPosition playerSpawnPosition = playersSpawnPositions[toSpawnPlayerID];
                Vector3 playerPosition = playerSpawnPosition.Position;
                Quaternion playerRotation = playerSpawnPosition.Rotation;

                Player spawnedPlayer = Instantiate(playerPrefab, playerPosition, playerRotation);
                spawnedPlayer.PlayerID = toSpawnPlayerID;
                spawnedPlayer.TeamID = GetPlayerTeamID(toSpawnPlayerID);

                activePlayers.Add(toSpawnPlayerID, spawnedPlayer);

                if (MotherOfManagers.Instance.IsSpawnARPlayers == true)
                {
                    spawnedPlayer.IsARPlayer = true;
                    spawnedPlayer.transform.parent = playerSpawnPosition.transform.parent;
                    spawnedPlayer.transform.localScale = playerSpawnPosition.transform.localScale;
                }

                EventManager.Instance.Invoke_PLAYERS_PlayerSpawned(toSpawnPlayerID);
            }
            else
            {
                Debug.LogError("Trying to spawn a player that is still active");
            }
            
        }
    }
    

    #region Input
    private void On_INPUT_ButtonPressed(EInputButton inputButton, EPlayerID playerID)
    {
        if (playerID == EPlayerID.TEST) return;

        switch (inputButton)
        {
            case EInputButton.CAST_SPELL_1:
                activePlayers[playerID].CastSpell_1();
                break;

            case EInputButton.CAST_SPELL_2:
                activePlayers[playerID].CastSpell_2();
                break;

            case EInputButton.CAST_SPELL_3:
                activePlayers[playerID].CastSpell_3();
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

            if (MotherOfManagers.Instance.IsSpawnPlayerOnConnect == true)
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


    private void InitializeDictionaries()
    {
        playersInput[EPlayerID.PLAYER_1] = new PlayerInput();
        playersInput[EPlayerID.PLAYER_2] = new PlayerInput();
        playersInput[EPlayerID.PLAYER_3] = new PlayerInput();
        playersInput[EPlayerID.PLAYER_4] = new PlayerInput();

        connectedPlayers[EPlayerID.PLAYER_1] = false;
        connectedPlayers[EPlayerID.PLAYER_2] = false;
        connectedPlayers[EPlayerID.PLAYER_3] = false;
        connectedPlayers[EPlayerID.PLAYER_4] = false;

        playerTeams[ETeamID.TEAM_1] = new List<EPlayerID>() { EPlayerID.PLAYER_1 };
        playerTeams[ETeamID.TEAM_2] = new List<EPlayerID>() { EPlayerID.PLAYER_2 };
        playerTeams[ETeamID.TEAM_3] = new List<EPlayerID>() { EPlayerID.PLAYER_3 };
        playerTeams[ETeamID.TEAM_4] = new List<EPlayerID>() { EPlayerID.PLAYER_4 };
    }

    

    private void LoadPlayerResources()
    {
        playerPrefabs.Add(EPlayerID.PLAYER_1, Resources.Load<Player>(MaleficusTypes.PATH_PLAYER_RED));
        playerPrefabs.Add(EPlayerID.PLAYER_2, Resources.Load<Player>(MaleficusTypes.PATH_PLAYER_BLUE));
        playerPrefabs.Add(EPlayerID.PLAYER_3, Resources.Load<Player>(MaleficusTypes.PATH_PLAYER_YELLOW));
        playerPrefabs.Add(EPlayerID.PLAYER_4, Resources.Load<Player>(MaleficusTypes.PATH_PLAYER_GREEN));
    }

    private void FindPlayerSpawnPositions()
    {
        PlayerSpawnPosition[] spawnPositions = FindObjectsOfType<PlayerSpawnPosition>();
        foreach (PlayerSpawnPosition spawnPosition in spawnPositions)
        {
            playersSpawnPositions.Add(spawnPosition.ToSpawnPlayerID, spawnPosition);
        }

        // Determine spawn positions relative to this transform if no PlayerSpawnPosition found in scene
        int angle;
        for (int i = 1; i < 5; i++)
        {
            angle = 90 * i;
            EPlayerID playerID = MaleficusTypes.IntToPlayerID(i);
            if (playersSpawnPositions.ContainsKey(playerID) == false)
            {
                PlayerSpawnPosition spawnPosition = Instantiate(Resources.Load<PlayerSpawnPosition>(MaleficusTypes.PATH_PLAYER_SPAWN_POSITION));
                spawnPosition.ToSpawnPlayerID = playerID;
                spawnPosition.Position = transform.position + Vector3.forward * 3.0f + Vector3.left * 3.0f;
                spawnPosition.transform.RotateAround(transform.position, Vector3.up, angle);
                spawnPosition.Rotation = transform.rotation;

                playersSpawnPositions.Add(playerID, spawnPosition);
            }
        }
    }


    /* public player information getter functions */
    public bool IsPlayerConnected(EPlayerID playerID)
    {
        return connectedPlayers[playerID];
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
}
