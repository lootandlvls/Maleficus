using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.Utils;
using static Maleficus.Consts;

public class PlayerManager : AbstractSingletonManager<PlayerManager>
{
    /// <summary> Positions in the scene (or around PlayerManager if not found) where the players will be spawned. </summary>
    public Dictionary<EPlayerID, PlayerSpawnPosition> PlayersSpawnPositions { get; } = new Dictionary<EPlayerID, PlayerSpawnPosition>();

    /// <summary> Added whenever a player has spawned. Removed when he dies. </summary>
    public Dictionary<EPlayerID, Player> ActivePlayers { get; } = new Dictionary<EPlayerID, Player>();

    /// <summary> All assigned players for every team. </summary>
    public Dictionary<ETeamID, List<EPlayerID>> Teams { get; } = new Dictionary<ETeamID, List<EPlayerID>>();

    /// <summary> Assigned team of every player </summary>
    public Dictionary<EPlayerID, ETeamID> PlayersTeam { get; } = new Dictionary<EPlayerID, ETeamID>();

    public EPlayerID OwnPlayerID { get { return GetPlayerIDFrom(NetworkManager.Instance.OwnerClientID); } }

    /// <summary> The join status of all the players in the current party </summary>
    private Dictionary<EPlayerID, PlayerJoinStatus> partyStatus { get; } = new Dictionary<EPlayerID, PlayerJoinStatus>();
    private Dictionary<EControllerID, EPlayerID> controllersMap { get; } = new Dictionary<EControllerID, EPlayerID>();
    private Dictionary<EPlayerID, Vector3> PlayersDeathPositions = new Dictionary<EPlayerID, Vector3>();
    private Dictionary<EPlayerID, Quaternion> PlayersDeathRotations = new Dictionary<EPlayerID, Quaternion>();
    private Dictionary<EPlayerID, Player> playersPrefabs { get; } = new Dictionary<EPlayerID, Player>();
    private PlayerSpawnPosition playerSpawnPositionPrefab;
    private PlayerRespawnGhost playerRespawnGhostPrefab;
    private IEnumerator ChargingDelayIEnumerator;

    #region BNJMO Behaviour
    protected override void Awake()
    {
        base.Awake();

        LoadPlayerResources();
        ReinitializeDictionaries();
    }

    protected override void Start()
    {
        base.Start();

        On_APP_SceneChanged_Event(new Event_GenericHandle<EScene>(AppStateManager.Instance.CurrentScene));
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        // Input events
        EventManager.Instance.INPUT_ControllerConnected.Event += On_INPUT_ControllerConnected;
        EventManager.Instance.INPUT_ControllerDisconnected.Event += On_INPUT_ControllerDisconnected;

        EventManager.Instance.INPUT_ButtonPressed.Event += On_INPUT_ButtonPressed;
        EventManager.Instance.INPUT_ButtonReleased.Event += On_INPUT_ButtonReleased;

        // Scene changed event
        EventManager.Instance.APP_SceneChanged.Event += On_APP_SceneChanged_Event;

        //Network
        //EventManager.Instance.NETWORK_ReceivedGameSessionInfo.AddListener       (On_NETWORK_ReceivedGameSessionInfo);
        EventManager.Instance.NETWORK_GameStateReplication.Event += On_NETWORK_GameStateReplicate;
        EventManager.Instance.NETWORK_GameStarted.Event += On_NETWORK_GameStarted;
    }

    protected override void Update()
    {
        base.Update();

        UpdatePartyDebugText();
    }

    #endregion

    #region Initialization
    private void ReinitializeDictionaries()
    {
        Teams.Clear();
        Teams[ETeamID.TEAM_1] = new List<EPlayerID>();
        Teams[ETeamID.TEAM_2] = new List<EPlayerID>();
        Teams[ETeamID.TEAM_3] = new List<EPlayerID>();
        Teams[ETeamID.TEAM_4] = new List<EPlayerID>();

        partyStatus.Clear();
        PlayersTeam.Clear();
        PlayersDeathPositions.Clear();
        PlayersDeathRotations.Clear();
        foreach (EPlayerID playerID in Enum.GetValues(typeof(EPlayerID)))
        {
            if ((playerID != EPlayerID.NONE)
                && (playerID != EPlayerID.SPECTATOR))
            {
                PlayersTeam.Add(playerID, ETeamID.NONE);
                PlayersDeathPositions.Add(playerID, Vector3.zero);
                PlayersDeathRotations.Add(playerID, Quaternion.identity);
                partyStatus.Add(playerID, new PlayerJoinStatus(EControllerID.NONE));
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

    private void FindPlayerSpellSelectionContexts()
    {
        foreach (PlayerSpellSelectionContext playerSpellSelectionContext in FindObjectsOfType<PlayerSpellSelectionContext>())
        {
            playerSpellSelectionContext.LeaveRequest += On_PlayerSpellSelectionContext_LeaveRequest;
            playerSpellSelectionContext.ReadyRequest += On_PlayerSpellSelectionContext_ReadyRequest;
            playerSpellSelectionContext.CancelReadyRequest += On_PlayerSpellSelectionContext_CancelReadyRequest;
        }
    }

    private void ReinitializeControllersMap()
    {
        List<EControllerID> keys = new List<EControllerID>();
        foreach (EControllerID key in controllersMap.Keys)
        {
            keys.Add(key);

        }
        foreach (EControllerID controllerID in keys)
        {
            controllersMap[controllerID] = EPlayerID.SPECTATOR;
        }
    }
    #endregion

    #region Players Management
    private void SpawnPlayer(EPlayerID playerID)
    {
        if (IS_TRUE(HasPlayerJoined(playerID)))
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
        LogConsole("Spawning remaining AI players");
        // Fill empty slots with AI
        int emptySlots = 4 - GetJoinedPlayers().Count;
        int remainingAI = MotherOfManagers.Instance.MaximumNumberOfAIToSpawn;
        while ((emptySlots > 0) && (remainingAI > 0))
        {
            EControllerID aIControllerID = GetAIControllerIDFrom(MotherOfManagers.Instance.MaximumNumberOfAIToSpawn - remainingAI + 1);
            JoinNextAIPlayer(aIControllerID);
            emptySlots--;
            remainingAI--;
        }

        foreach (EPlayerID playerID in GetJoinedPlayers())
        {
            if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID))
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
    #endregion

    #region Party Join
    private EPlayerID GetNextFreePlayerSpot()
    {
        foreach (var pair in partyStatus)
        {
            EPlayerID playerID = pair.Key;
            PlayerJoinStatus playerJoinStatus = pair.Value;

            if (playerJoinStatus.HasJoined == false)
            {
                return playerID;
            }
        }
        return EPlayerID.NONE;
    }

    private void JoinPlayer(EPlayerID playerID, EControllerID controllerID)
    {
        if ((IS_KEY_CONTAINED(partyStatus, playerID))
            && (IS_NOT_TRUE(partyStatus[playerID].HasJoined)))
        {
            partyStatus[playerID].ControllerID = controllerID;
            partyStatus[playerID].HasJoined = true;

            // Update assigned PlayerID in the controller map (instead of Spectator)
            if (IS_KEY_CONTAINED(controllersMap, controllerID))
            {
                controllersMap[controllerID] = playerID;
            }

            EventManager.Instance.Invoke_PLAYERS_PlayerJoined(playerID, controllerID);
        }
    }

    private EPlayerID JoinNextAIPlayer(EControllerID controllerID)
    {
        foreach(var pair in partyStatus)
        {
            EPlayerID playerID = pair.Key;
            PlayerJoinStatus playerJoinStatus = pair.Value;

            if (playerJoinStatus.HasJoined == false)
            {
                JoinPlayer(playerID, controllerID);
                return playerID;
            }
        }
        return EPlayerID.NONE;
    }

    private void On_PlayerSpellSelectionContext_LeaveRequest(EPlayerID playerID)
    {
        if ((IS_KEY_CONTAINED(partyStatus, playerID))
        && (IS_TRUE(partyStatus[playerID].HasJoined)))
        {
            // Reset controller to Spectator
            EControllerID controllerID = partyStatus[playerID].ControllerID;
            if (IS_KEY_CONTAINED(controllersMap, controllerID))
            {
                controllersMap[controllerID] = EPlayerID.SPECTATOR;
            }

            // Reset entry of playerID in party status 
            partyStatus[playerID] = new PlayerJoinStatus(EControllerID.NONE);

            // Trigger global event
            EventManager.Instance.Invoke_PLAYERS_PlayerLeft(playerID);

            // Are the rest of the joined players ready?
            CheckIfAllPlayersAreReady();
        }
    }

    private void On_PlayerSpellSelectionContext_ReadyRequest(EPlayerID playerID)
    {
        LogConsole(playerID + " ready");
        if ((IS_KEY_CONTAINED(partyStatus, playerID))
        && (IS_TRUE(partyStatus[playerID].HasJoined))
        && (IS_NOT_TRUE(partyStatus[playerID].IsReady)))
        {
            partyStatus[playerID].IsReady = true;

            EventManager.Instance.Invoke_PLAYERS_PlayerReady(playerID);

            CheckIfAllPlayersAreReady();
        }
    }

    private void On_PlayerSpellSelectionContext_CancelReadyRequest(EPlayerID playerID)
    {
        if ((IS_KEY_CONTAINED(partyStatus, playerID))
        && (IS_TRUE(partyStatus[playerID].HasJoined))
        && (IS_TRUE(partyStatus[playerID].IsReady)))
        {
            partyStatus[playerID].IsReady = false;

            EventManager.Instance.Invoke_PLAYERS_PLAYERS_PlayerCanceledReady(playerID);
        }
    }
    
    private void CheckIfAllPlayersAreReady()
    {
        bool areAllReady = false;
        foreach (PlayerJoinStatus playerJoinStatus in partyStatus.Values)
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
    #endregion

    #region Input
    private void On_INPUT_ButtonPressed(NetEvent_ButtonPressed eventHandle)
    {
        
        EInputButton inputButton = eventHandle.InputButton;
        EControllerID controllerID = eventHandle.ControllerID;
        EPlayerID playerID = GetPlayerIDFrom(eventHandle.SenderID);

        switch (AppStateManager.Instance.CurrentState)
        {
           
            case EAppState.IN_GAME_IN_RUNNING:
         
                ESpellSlot spellSlot = GetSpellSlotFrom(inputButton);

                if ((spellSlot != ESpellSlot.NONE)
                    && (ActivePlayers.ContainsKey(playerID) == true))
                {
              
                    AbstractSpell spell = SpellManager.Instance.GetChosenSpell(playerID, spellSlot);
                    if (IS_NOT_NULL(spell))
                    {
                       
                        if (ActivePlayers[playerID].IsReadyToShoot)
                        {
                         

                            switch (spell.MovementType)
                            {
                                case ESpellMovementType.LINEAR_HIT:
                             
                                    if (ActivePlayers[playerID].ReadyToUseSpell[spellSlot] && !ActivePlayers[playerID].IsPlayerCharging && ActivePlayers[playerID].SpellButtonPressed[spellSlot] == false  )
                                    {

                                        ActivePlayers[playerID].SpellButtonPressed[spellSlot] = true;
                                        ActivePlayers[playerID].ResetSpellChargingPower();
                                        ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);
                                        ActivePlayers[playerID].StartChargingSpell(spell, spellSlot);
                                        
                                    }
                                  
                                    break;
                                case ESpellMovementType.LINEAR_WAVE:

                                    if (ActivePlayers[playerID].ReadyToUseSpell[spellSlot] && !ActivePlayers[playerID].IsPlayerCharging && ActivePlayers[playerID].SpellButtonPressed[spellSlot] == false)
                                    {

                                        ActivePlayers[playerID].SpellButtonPressed[spellSlot] = true;
                                        ActivePlayers[playerID].ResetSpellChargingPower();
                                        ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);
                                        if (spell.IsChargeable)
                                        {
                                            ActivePlayers[playerID].StartChargingSpell(spell, spellSlot);

                                        }

                                    }

                                    break;
                                case ESpellMovementType.LINEAR_LASER:
                                    if (ActivePlayers[playerID].ReadyToUseSpell[spellSlot] && !ActivePlayers[playerID].IsPlayerCharging && ActivePlayers[playerID].SpellButtonPressed[spellSlot] == false)
                                    {

                                        ActivePlayers[playerID].SpellButtonPressed[spellSlot] = true;
                                        /*   ActivePlayers[playerID].IsReadyToShoot = false;
                                           ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
                                           SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingPower);
                                           StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));*/
                                        ActivePlayers[playerID].ResetSpellChargingPower();
                                        ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);
                                        ActivePlayers[playerID].StartChargingSpell(spell, spellSlot);
                                    }
                                        break;
                                case ESpellMovementType.TELEPORT:
                                     if (   ActivePlayers[playerID].ReadyToUseSpell[spellSlot])
                                     {
                                        ActivePlayers[playerID].IsReadyToShoot = false;
                                        ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
                                        SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingPower);
                                        StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));

                                     }
                                 
                                    break;
                                case ESpellMovementType.STATIC:
                                    if (ActivePlayers[playerID].ReadyToUseSpell[spellSlot])
                                    {
                                        ActivePlayers[playerID].IsReadyToShoot = false;
                                        ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
                                        SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingPower);
                                        StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));
                                    }
                                    break;
                                case ESpellMovementType.LINEAR_EXPLOSIVE:
                                    if (ActivePlayers[playerID].ReadyToUseSpell[spellSlot] && !ActivePlayers[playerID].IsPlayerCharging && ActivePlayers[playerID].SpellButtonPressed[spellSlot] == false)
                                    {

                                        ActivePlayers[playerID].SpellButtonPressed[spellSlot] = true;                                    
                                        ActivePlayers[playerID].ResetSpellChargingPower();
                                        ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);
                                        ActivePlayers[playerID].StartChargingSpell(spell, spellSlot);
                                    }
                                    break;



                            }

                        }
                      /*  ActivePlayers[playerID].ResetSpellChargingPower();

                        // Instantiate spell now ?
                        if (spell.MovementType == ESpellMovementType.LINEAR_LASER)
                        {
                            if (ActivePlayers[playerID].IsReadyToShoot && ActivePlayers[playerID].ReadyToUseSpell[spellSlot] && ActivePlayers[playerID].IsPlayerCharging == false)
                            {

                                ActivePlayers[playerID].IsReadyToShoot = false;
                                ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
                               // ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);

                                SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingPower);

                                StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));
                            }
                        }
                      
                        else if (spell.MovementType == ESpellMovementType.UNIQUE)
                        {
                            if (ActivePlayers[playerID].IsReadyToShoot && ActivePlayers[playerID].ReadyToUseSpell[spellSlot] && ActivePlayers[playerID].IsPlayerCharging == false)
                            {
                                if (!ActivePlayers[playerID].HasCastedSpell)
                                {
                                    StartCoroutine(FirstTimeSpellCastedCoroutine(playerID, spellSlot, spell.CastDuration));
                                    SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingPower);
                                    StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));
                                }
                            }
                            else
                            {
                                EventManager.Instance.Invoke_SPELLS_UniqueEffectActivated(spell.SpellID, playerID);
                            }
                        }

                        else if (spell.IsChargeable && ActivePlayers[playerID].IsReadyToShoot && ActivePlayers[playerID].IsPlayerCharging == false && ActivePlayers[playerID].SpellButtonPressed[spellSlot] == false)
                        {
                            ActivePlayers[playerID].SpellButtonPressed[spellSlot] = true;

                            ActivePlayers[playerID].StartChargingSpell(spell, spellSlot);

                            //Player can t use other spells while charging 

                            StartNewCoroutine(ref ChargingDelayIEnumerator, ChargingDelayCoroutine(playerID, spell,spellSlot));
                            Debug.Log("Start charging");

                        }*/
                    }
                }
                break;

            case EAppState.IN_MENU_IN_SPELL_SELECTION:
                // Join player if Confirm button is presssed
                if (inputButton == EInputButton.CONFIRM)
                {
                    // Is Controller not alreay joined?
                    if ((IS_KEY_CONTAINED(controllersMap, controllerID)
                        && (controllersMap[controllerID] == EPlayerID.SPECTATOR)))
                    {
                        EPlayerID nextPlayerID = GetNextFreePlayerSpot();
                        // Is party not full already?
                        if (nextPlayerID != EPlayerID.NONE)
                        {
                            JoinPlayer(nextPlayerID, controllerID);
                        }
                    }
                }
                break;
        }
 
    }

    private void On_INPUT_ButtonReleased(NetEvent_ButtonReleased eventHandle)
    {
        Debug.Log("Button RELEASED");
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
                

                switch (spell.MovementType)
                {
                    case ESpellMovementType.LINEAR_HIT:

                        if (ActivePlayers[playerID].SpellButtonPressed[spellSlot] == true && ActivePlayers[playerID].ReadyToUseSpell[spellSlot] )
                        {
                            ActivePlayers[playerID].IsPlayerCharging = false;
                            
                            ActivePlayers[playerID].SpellButtonPressed[spellSlot] = false;
                            ActivePlayers[playerID].IsReadyToShoot = false;


                             ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);
                           

                            SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingPower);
                            ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
                            StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));
                           
                        }
                        break;
                    case ESpellMovementType.LINEAR_WAVE:

                        if (ActivePlayers[playerID].SpellButtonPressed[spellSlot] == true && ActivePlayers[playerID].ReadyToUseSpell[spellSlot])
                        {
                            ActivePlayers[playerID].IsPlayerCharging = false;

                            ActivePlayers[playerID].SpellButtonPressed[spellSlot] = false;
                            ActivePlayers[playerID].IsReadyToShoot = false;


                            ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);


                            SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingPower);
                            ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
                            StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));

                        }
                        break;
                    case ESpellMovementType.LINEAR_LASER:

                        if (ActivePlayers[playerID].SpellButtonPressed[spellSlot] == true && ActivePlayers[playerID].ReadyToUseSpell[spellSlot])
                        {
                            ActivePlayers[playerID].IsPlayerCharging = false;

                            ActivePlayers[playerID].SpellButtonPressed[spellSlot] = false;
                            ActivePlayers[playerID].IsReadyToShoot = false;


                            ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);


                            SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingPower);
                            ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
                            StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));

                        }
                        break;
                    case ESpellMovementType.LINEAR_EXPLOSIVE:

                        if (ActivePlayers[playerID].SpellButtonPressed[spellSlot] == true && ActivePlayers[playerID].ReadyToUseSpell[spellSlot])
                        {
                            ActivePlayers[playerID].IsPlayerCharging = false;

                            ActivePlayers[playerID].SpellButtonPressed[spellSlot] = false;
                            ActivePlayers[playerID].IsReadyToShoot = false;


                            ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);


                            SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingPower);
                            ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;
                            StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));

                        }
                        break;
                   
                }





                /*  if (spell.MovementType != ESpellMovementType.LINEAR_LASER && spell.MovementType != ESpellMovementType.UNIQUE)
                  {
                      if (ActivePlayers[playerID].IsReadyToShoot && ActivePlayers[playerID].ReadyToUseSpell[spellSlot] && ActivePlayers[playerID].SpellButtonPressed[spellSlot] == true)
                      {
                          StopCoroutineIfRunning(ChargingDelayIEnumerator);
                          ActivePlayers[playerID].SpellButtonPressed[spellSlot] = false;
                          if (ActivePlayers[playerID].IsPlayerCharging)
                          {
                              ActivePlayers[playerID].StopChargingSpell(spell, spellSlot);
                          }
                          ActivePlayers[playerID].RotateToClosestPlayer();

                          SpellManager.Instance.CastSpell(playerID, spellSlot, ActivePlayers[playerID].SpellChargingPower);
                          ActivePlayers[playerID].IsReadyToShoot = false;
                          ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = false;                       


                          StartCoroutine(SetReadyToUseSpellCoroutine(playerID, spellSlot));
                      }
                  }
                  */
            }
        }
    }
    #endregion

    #region Events Callbacks
    private void On_APP_SceneChanged_Event(Event_GenericHandle<EScene> eventHandle)
    {
        ActivePlayers.Clear();

        EScene scene = eventHandle.Arg1;
        switch (scene)
        {
            case EScene.GAME:
                FindPlayerSpawnGhost();
                StartCoroutine(DelayedSpawnAllCoroutine());
                break;

            case EScene.MENU:
                ReinitializeDictionaries();
                FindPlayerSpellSelectionContexts();
                ReinitializeControllersMap();
                break;
        }

    }

    private IEnumerator DelayedSpawnAllCoroutine()
    {
        yield return new WaitForEndOfFrame();

        SpawnAllJoinedPlayers();
    }

    private void On_NETWORK_GameStarted(NetEvent_GameStarted eventHandle)
    {
        // Connect and spawn reamining players as AI
        //if (MotherOfManagers.Instance.IsSpawnRemainingAIPlayersOnGameStart == true)
        //{
        //    //SpawnRemaningAIPlayers();
        //}
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

    private void On_INPUT_ControllerConnected(Event_GenericHandle<EControllerID> eventHandle)
    {
        EControllerID controllerID = eventHandle.Arg1;

        // Add connected controller as a spectator
        if (IS_KEY_NOT_CONTAINED(controllersMap, controllerID))
        {
            controllersMap.Add(controllerID, EPlayerID.SPECTATOR);
        }
    }

    private void On_INPUT_ControllerDisconnected(Event_GenericHandle<EControllerID> eventHandle)
    {
        EControllerID controllerID = eventHandle.Arg1;

        if (IS_KEY_CONTAINED(controllersMap, controllerID))
        {
            controllersMap.Remove(controllerID);
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
    #endregion

    #region public methods
    public List<EPlayerID> GetJoinedPlayers()
    {
        List<EPlayerID> result = new List<EPlayerID>();
        foreach (KeyValuePair<EPlayerID, PlayerJoinStatus> pair in partyStatus)
        {
            PlayerJoinStatus playerJoinStatus = pair.Value;
            if (playerJoinStatus.HasJoined == true)
            {
                result.Add(pair.Key);
            }
        }
        return result;
    }

    public bool HasPlayerJoined(EPlayerID playerID)
    {
        foreach (KeyValuePair<EPlayerID, PlayerJoinStatus> pair in partyStatus)
        {
            PlayerJoinStatus playerJoinStatus = pair.Value;
            if ((playerID == pair.Key)
                && (playerJoinStatus.HasJoined == true))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsPlayerAlive(EPlayerID playerID)
    {
        return ActivePlayers.ContainsKey(playerID);
    }

    public EPlayerID[] GetPlayersInTeam(ETeamID inTeamID)
    {
        return Teams[inTeamID].ToArray();
    }

    public EPlayerID GetAssignedPlayerID(EControllerID controllerID)
    {
        if (IS_KEY_CONTAINED(controllersMap, controllerID))
        {
            return controllersMap[controllerID];
        }
        return EPlayerID.NONE;
    }

    public EControllerID GetAssignedControllerID(EPlayerID playerID)
    {
        if (IS_KEY_CONTAINED(partyStatus, playerID))
        {
            return partyStatus[playerID].ControllerID;
        }
        return EControllerID.NONE;
    }
    #endregion

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

    private IEnumerator ChargingDelayCoroutine(EPlayerID playerID , ISpell spell , ESpellSlot spellSlot)
    {
        yield return new WaitForSeconds(1f);
        ActivePlayers[playerID].IsPlayerCharging = true;
        ActivePlayers[playerID].StartChargingSpell(spell, spellSlot);

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
   

    private void UpdatePartyDebugText()
    {
        string playerStatusLog = "Party join status : \n";
        foreach (var pair in partyStatus)
        {
            playerStatusLog += pair.Key + " : " + pair.Value.ControllerID + " - joined : " + pair.Value.HasJoined + " | is ready : " + pair.Value.IsReady + "\n";
        }
        LogCanvas(12, playerStatusLog);
    }
}
