﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : Singleton<PlayerManager>
{
    
    [Header("Spawn character when the player connects with a controller")]
    [SerializeField] private bool isSpawnPlayerOnConnect_DebugMode = false;
    [SerializeField] private bool isSpawnAllPlayers_DebugMode = false;
    [SerializeField] private bool isSpawnArPlayers = false;

    // Always defined dictionaries 
    Dictionary<EPlayerID, Player> playerPrefabs                         = new Dictionary<EPlayerID, Player>();
    Dictionary<EPlayerID, PlayerSpawnPosition> playersSpawnPositions    = new Dictionary<EPlayerID, PlayerSpawnPosition>();
    Dictionary<EPlayerID, PlayerInput> playersInput                     = new Dictionary<EPlayerID, PlayerInput>();

    // Only defined for active members dictionaries
    Dictionary<EPlayerID, bool> connectedPlayers                        = new Dictionary<EPlayerID, bool>(); 
    Dictionary<EPlayerID, Player> activePlayers                         = new Dictionary<EPlayerID, Player>();

   



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

        if (isSpawnAllPlayers_DebugMode == true)
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


    

    private void SpawnPlayer(EPlayerID toSpawnPlayerID)
    {
        if ((connectedPlayers[toSpawnPlayerID] == true) || (isSpawnAllPlayers_DebugMode == true))
        {
            if (activePlayers.ContainsKey(toSpawnPlayerID) == false)
            {
                Player playerPrefab = playerPrefabs[toSpawnPlayerID];
                PlayerSpawnPosition playerSpawnPosition = playersSpawnPositions[toSpawnPlayerID];
                Vector3 playerPosition = playerSpawnPosition.Position;
                Quaternion playerRotation = playerSpawnPosition.Rotation;

                Player spawnedPlayer = Instantiate(playerPrefab, playerPosition, playerRotation);
                spawnedPlayer.PlayerID = toSpawnPlayerID;

                activePlayers.Add(toSpawnPlayerID, spawnedPlayer);

                if (isSpawnArPlayers == true)
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

            if (isSpawnPlayerOnConnect_DebugMode == true)
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

    public bool IsPlayerConnected(EPlayerID playerID)
    {
        return connectedPlayers[playerID];
    }

    public bool IsPlayerActive(EPlayerID playerID)
    {
        return activePlayers.ContainsKey(playerID);
    }

    public Dictionary<EPlayerID, Player> GetActivePlayers()
    {
        return activePlayers;
    }

}
