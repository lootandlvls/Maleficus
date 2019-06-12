using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : Singleton<PlayerManager>
{
    [Header("Spawn character when the player connects with a controller")]
    [SerializeField] private bool isSpawnPlayerOnConnect_DebugMode = false;
    [SerializeField] private bool isSpawnAllPlayers_DebugMode = false;

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
        //  Spell events
        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;

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


    #region Spell
    private void On_SPELLS_SpellHitPlayer(HitInfo hitInfo)
    {
       
        if (activePlayers[hitInfo.HitPlayerID].PlayerID == hitInfo.HitPlayerID)
        {

            //  StartCoroutine(PushPlayer(hitInfo));
            

        }
        if (hitInfo.HasEffect)
        {   foreach (Debuff debuffeffect in hitInfo.DebuffEffects)
                {
                ApplyDebuff(hitInfo.HasEffect, debuffeffect , hitInfo.HitPlayerID);
            }

            foreach (Buff buffeffect in hitInfo.DebuffEffects)
            {
                ApplyBuff(hitInfo.HasEffect, buffeffect, hitInfo.HitPlayerID);
            }
           
        }
    }
    private IEnumerator PushPlayer(HitInfo hitInfo)
    {
        Rigidbody rgb = activePlayers[hitInfo.HitPlayerID].GetComponent<Rigidbody>();
        rgb.isKinematic = false;
        transform.position = Vector3.MoveTowards(activePlayers[hitInfo.HitPlayerID].transform.position, hitInfo.HitVelocity, Time.deltaTime * 2);
        yield return new WaitForSeconds(0.1f);
        rgb.isKinematic = true;
    }

    private void ApplyDebuff(bool hasEffect ,Debuff debuff , EPlayerID playerID)
    {
        if (hasEffect)
        {
            switch (debuff)
            {
                case Debuff.FROZEN:
                    Debug.Log("Player Frozen");
                    
                    break;

                case Debuff.STUN:
                    Debug.Log("Player Stunned");
               
                    break;

                case Debuff.SLOWDOWN:
                    Debug.Log("Player SLOWED DOWN");
                    
                    break;

                case Debuff.CHARM:

                    break;
                   
            }

        }


    }
    private void ApplyBuff(bool hasEffect, Buff buff, EPlayerID playerID)
    {
        if (hasEffect)
        {
            switch (buff)
            {
                case Buff.INCREACE_SPEED:
                    Debug.Log("Spead increased");
                    break;

                case Buff.INCREASE_CASTING_SPEED:
                    Debug.Log("INCREASE_CASTING_SPEED");
                    break;

                case Buff.INCREASE_DAMAGE:
                    Debug.Log("IINCREASE_DAMAGE");
                    break;

                case Buff.INCREASE_OFFENSIVE_SPELL_SIZE:
                    Debug.Log("INCREASE_OFFENSIVE_SPELL_SIZE");
                    break;
                case Buff.PROTECT:
                    Debug.Log("PROTECT");
                    break;
                case Buff.REMOVE_DEBUFF:
                    Debug.Log("REMOVE_DEBUF");
                    break;


            }

        }
    }

        #region SpellEffects
        #endregion
        #endregion

        private void SpawnPlayer(EPlayerID toSpawnPlayerID)
    {
        if ((connectedPlayers[toSpawnPlayerID] == true) || (isSpawnAllPlayers_DebugMode == true))
        {
            if (activePlayers.ContainsKey(toSpawnPlayerID) == false)
            {
                Player playerPrefab = playerPrefabs[toSpawnPlayerID];
                Vector3 playerPosition = playersSpawnPositions[toSpawnPlayerID].Position;
                Quaternion playerRotation = playersSpawnPositions[toSpawnPlayerID].Rotation;

                Player spawnedPlayer = Instantiate(playerPrefab, playerPosition, playerRotation);
                spawnedPlayer.PlayerID = toSpawnPlayerID;

                activePlayers.Add(toSpawnPlayerID, spawnedPlayer);

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


   
}
