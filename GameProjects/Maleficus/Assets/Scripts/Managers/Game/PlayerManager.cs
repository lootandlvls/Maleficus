﻿using System;
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

    /* Dictionaries that are defined only for connected players  */
    /// <summary> Initially initialized with false, then true whenever the respective player connects. </summary>
    public Dictionary<EPlayerID, bool>                  ConnectedPlayers            { get { return connectedPlayers; } }

    /// <summary> Added whenever a player has spawned. Removed when he dies. </summary>
    public Dictionary<EPlayerID, Player>                ActivePlayers               { get { return activePlayers; } }

    /// <summary> All assigned players for every team. </summary>
    public Dictionary<ETeamID, List<EPlayerID>>         Teams                       { get { return teams; } }

    /// <summary> Assigned team of every player </summary>
    public Dictionary<EPlayerID, ETeamID>               PlayersTeam                 { get { return playersTeam; } }



    /// <summary> Mapping from controllerID to playerID </summary> 
    private Dictionary<EPlayerID, EControllerID>        playerControllerMapping     = new Dictionary<EPlayerID, EControllerID>();

    private Dictionary<EPlayerID, Player>               playerPrefabs               = new Dictionary<EPlayerID, Player>();
    private Dictionary<EPlayerID, PlayerSpawnPosition>  playersSpawnPositions       = new Dictionary<EPlayerID, PlayerSpawnPosition>();
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


        // Input events
        EventManager.Instance.INPUT_ButtonPressed.AddListener                   (On_INPUT_ButtonPressed);
        EventManager.Instance.INPUT_ButtonReleased.AddListener                  (On_INPUT_ButtonReleased);
        //EventManager.Instance.INPUT_JoystickMoved.AddListener                   (On_INPUT_JoystickMoved);

        // Scene changed event
        EventManager.Instance.APP_SceneChanged.AddListener                      (On_APP_SceneChanged);
        EventManager.Instance.APP_AppStateUpdated.AddListener                   (On_APP_AppStateUpdated);

        //Network
        EventManager.Instance.NETWORK_ReceivedGameSessionInfo.AddListener       (On_NETWORK_ReceivedGameSessionInfo);
        EventManager.Instance.NETWORK_GameStateReplicate.AddListener            (On_NETWORK_GameStateReplicate);
        EventManager.Instance.NETWORK_GameStarted.AddListener                   (On_NETWORK_GameStarted);

        StartCoroutine(LateStartCoroutine());
    }

    

    

    private IEnumerator LateStartCoroutine()
    {
        yield return new WaitForEndOfFrame();

        // Connect Touch player as first player
        if ((MotherOfManagers.Instance.InputMode == EInputMode.TOUCH) && (MotherOfManagers.Instance.IsSpawnTouchAsPlayer1 == true) && (playerControllerMapping.ContainsValue(EControllerID.TOUCH) == false))
        {
            Debug.Log("QWDQDQWDQ : Connecting Touhc");
            ConnectPlayer(EPlayerID.PLAYER_1, EControllerID.TOUCH);
            Debug.Log("QWDQDQWDQ : Connected Touhc");

        }

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
        //UpdateControllersInput();
    }


    private void On_NETWORK_ReceivedGameSessionInfo(BasicEventHandle<List<EPlayerID>, EPlayerID> eventHandle)
    {
        List<EPlayerID> connectedPlayers = eventHandle.Arg1;
        EPlayerID ownPlayer = eventHandle.Arg2;

        // Connect and spawn player
        foreach (EPlayerID playerID in connectedPlayers)
        {
            if (playerID == ownPlayer)
            {
                ConnectPlayer(playerID, EControllerID.TOUCH);
            }
            else
            {
                ConnectPlayer(playerID, MaleficusUtilities.GetPlayerNeteworkID(playerID));
            }
        }
    }

    private void On_NETWORK_GameStateReplicate(GameStateReplicateEventhandle gameState)
    {
        EPlayerID playerID = gameState.playerID;
        float[] playerPosition = gameState.playerPosition;
        float[] playerRotation = gameState.playerRotation;

        if (activePlayers.ContainsKey(playerID))
        {
            activePlayers[playerID].transform.localPosition = new Vector3(playerPosition[0], playerPosition[1], playerPosition[2]);
            activePlayers[playerID].transform.localRotation = Quaternion.Euler(new Vector3(playerRotation[0], playerRotation[1], playerRotation[2]));
        }
    }

    private void On_NETWORK_GameStarted(GameStartedEventHandle eventHandle)
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
        if ((ConnectedPlayers[toSpawnPlayerID] == true) 
            || ((MotherOfManagers.Instance.IsSpawnAllPlayers == true) 
                && (AppStateManager.Instance.CurrentScene == EScene.GAME)))
        {
            
            if (ActivePlayers.ContainsKey(toSpawnPlayerID) == false)
            {
                Debug.Log("Respawning Player...");
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

                // Set AR Player
                if (MotherOfManagers.Instance.IsARGame == true)
                {
                    spawnedPlayer.IsARPlayer = true;
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

    private void RemovePlayerFromTeam(EPlayerID playerID, ETeamID teamID)
    {
        // TODO: Implement removing player from team
    }

    /// <summary>
    /// Connect new player to a free spot
    /// </summary>
    /// <returns> The ID of the connected player </returns>
    public EPlayerID ConnectNextPlayerToController(EControllerID controllerID)
    {
        EPlayerID playerIDToConnect = EPlayerID.NONE;

        // Look for a free slot to connect player
        foreach (EPlayerID playerID in connectedPlayers.Keys)
        {
            if (ConnectedPlayers[playerID] == false)
            {
                ConnectedPlayers[playerID] = true;
                AssignPlayerToTeam(playerID, MaleficusUtilities.GetIdenticPlayerTeam(playerID));
                playerIDToConnect = playerID;
                playerControllerMapping[playerID] = controllerID;
                continue;
            }
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

    private void ConnectPlayer(EPlayerID playerID, EControllerID controllerID)
    {
        if (ConnectedPlayers[playerID] == false)
        {
            ConnectedPlayers[playerID] = true;
            AssignPlayerToTeam(EPlayerID.PLAYER_1, ETeamID.TEAM_1);
            playerControllerMapping[playerID] = controllerID;

            InputManager.Instance.ConnectController(controllerID);

            // Debug Spawn player
            if ((MotherOfManagers.Instance.IsSpawnPlayerOnConnect == true) 
                && (ActivePlayers.ContainsKey(playerID) == false)
                && (AppStateManager.Instance.CurrentScene == EScene.GAME))
            {
                SpawnPlayer(EPlayerID.PLAYER_1);
            }

        }
    }

    public void DisconnectPlayer(EControllerID controllerID)
    {
        EPlayerID playerID = GetPlayerIDFrom(controllerID);

        if (IsPlayerConnected(playerID) == true)
        {
            EventManager.Instance.Invoke_PLAYERS_PlayerDisconnected(playerID);
            ConnectedPlayers[playerID] = false;
            InputManager.Instance.DisconnectController(controllerID);
        }
        else
        {
            Debug.LogError("Trying to disconnect a player that is not connected");
        }
    }

    
    #endregion

    #region Input


    private void On_INPUT_ButtonPressed(ButtonPressedEventHandle eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EPlayerID playerID = eventHandle.PlayerID;

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

    private void On_INPUT_JoystickMoved(JoystickMovedEventHandle eventHandle)
    {
        EJoystickType joystickType = eventHandle.JoystickType;
        float joystick_X = eventHandle.Joystick_X;
        float joystick_Y = eventHandle.Joystick_Y;
        EPlayerID playerID = eventHandle.PlayerID;

        if (playerID == EPlayerID.TEST) return;

        //switch (axisType)
        //{
        //    case EInputAxis.MOVE_X:
        //        PlayersInput[playerID].Move_X = axisValue;
        //        break;

        //    case EInputAxis.MOVE_Y:
        //        PlayersInput[playerID].Move_Y = axisValue;
        //        break;

        //    case EInputAxis.ROTATE_X:
        //        PlayersInput[playerID].Rotate_X = axisValue;
        //        break;

        //    case EInputAxis.ROTATE_Y:
        //        PlayersInput[playerID].Rotate_Y = axisValue;
        //        break;
        //}
    }

    private void On_INPUT_ButtonReleased(ButtonReleasedEventHandle eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EPlayerID playerID = eventHandle.PlayerID;

        ESpellSlot spellSlot = MaleficusUtilities.GetSpellSlotFrom(inputButton);
        if ((spellSlot == ESpellSlot.NONE) || (activePlayers.ContainsKey(playerID) == false))
        {
            return;
        }

        ISpell spell = SpellManager.Instance.Player_Spells[playerID][spellSlot];


        Debug.Log(spellSlot + " by " + playerID + " > IsReadyToShoot : " + ActivePlayers[playerID].IsReadyToShoot + " > ReadyToUseSpell : " + ActivePlayers[playerID].ReadyToUseSpell[spellSlot]);

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
                    if ((MotherOfManagers.Instance.IsARGame == true) && (ARManager.IsInstanceSet == true))
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

    public ControllerInput GetPlayerInput(EPlayerID playerID)
    {
        if (playerControllerMapping.ContainsKey(playerID) == false)
        {
            return new ControllerInput();
        }
        EControllerID controllerID = playerControllerMapping[playerID];

        return InputManager.Instance.ControllersInput[controllerID];
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
        yield return new WaitForSeconds(ActivePlayers[playerID].SpellDuration[spellSlot]);
        ActivePlayers[playerID].IsReadyToShoot = true;

        yield return new WaitForSeconds(ActivePlayers[playerID].SpellCooldown[spellSlot]);
        ActivePlayers[playerID].ReadyToUseSpell[spellSlot] = true;

        
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

        Player playerToDestroy = ActivePlayers[playerID];
        ActivePlayers.Remove(playerID);
        playerToDestroy.DestroyPlayer();
        EventManager.Instance.Invoke_PLAYERS_PlayerDied(playerID);
    }

    public EPlayerID GetPlayerIDFrom(EControllerID controllerID)
    {
        EPlayerID result = EPlayerID.NONE;
        foreach (EPlayerID playerID in playerControllerMapping.Keys)
        {
            if (controllerID == playerControllerMapping[playerID])
            {
                result = playerID;
            }
        }
        return result;
    }

    public EControllerID GetControllerFrom(EPlayerID playerID)
    {
        if (playerControllerMapping.ContainsKey(playerID))
        {
            return playerControllerMapping[playerID];
        }
        return EControllerID.NONE;
    }


    private void UpdateControllersInput()
    {
        //var controllersInput = InputManager.Instance.ControllersInput;
        //foreach (EControllerID controllerID in controllersInput.Keys)
        //{
        //    ControllerInput controllerInput = controllersInput[controllerID];
        //    EPlayerID playerID = GetPlayerIDFrom(controllerID);

        //    // Movement
        //    if (controllerInput.HasMoved())
        //    {
        //        float x = controllerInput.JoystickValues[EInputAxis.MOVE_X];
        //        float y = controllerInput.JoystickValues[EInputAxis.MOVE_Y];
        //        JoystickMovedEventHandle eventHandle = new JoystickMovedEventHandle(EJoystickType.MOVEMENT, x, y, playerID);

        //        On_INPUT_JoystickMoved(eventHandle);
        //    }

        //    // Rotation
        //    if (controllerInput.HasRotated())
        //    {
        //        float x = controllerInput.JoystickValues[EInputAxis.ROTATE_X];
        //        float y = controllerInput.JoystickValues[EInputAxis.ROTATE_Y];
        //        JoystickMovedEventHandle eventHandle = new JoystickMovedEventHandle(EJoystickType.ROTATION, x, y, playerID);

        //        On_INPUT_JoystickMoved(eventHandle);
        //    }

        //    // Button pressesd
        //    var isButtonPressed = controllerInput.IsButtonPressed;
        //    foreach (EInputButton inputButton in isButtonPressed.Keys)
        //    {
        //        if (isButtonPressed[inputButton] == true)
        //        {
        //            ButtonPressedEventHandle eventHandle = new ButtonPressedEventHandle(playerID, inputButton);
        //            On_INPUT_ButtonPressed(eventHandle);
        //        }
        //    }

        //    // Button Released
        //    var isButtonReleased = controllerInput.IsButtonReleased;
        //    foreach (EInputButton inputButton in isButtonReleased.Keys)
        //    {
        //        if (isButtonReleased[inputButton] == true)
        //        {
        //            Debug.Log("inputButton : " + inputButton + " | controller : " + controllerID);
        //            ButtonReleasedEventHandle eventHandle = new ButtonReleasedEventHandle(playerID, inputButton);
        //            //On_INPUT_ButtonReleased(eventHandle);
        //        }
        //    }
        //}
    }
}
