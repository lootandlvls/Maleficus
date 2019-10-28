using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class AppStateManager : AbstractSingletonManagerWithStateMachine<AppStateManager, EAppState>
{
    public EScene CurrentScene { get { return currentScene; } }  //return GetScene(CurrentState); } }


    private EScene currentScene = EScene.ENTRY;

    private List<AbstractUIAction> boundActions = new List<AbstractUIAction>();

    private EDungeonID dungeonIDtoLoad = EDungeonID.NONE;

    protected override void Awake()
    {
        base.Awake();

        // 1) Assign appropriate currentState from MaleficusTypes
        startStates = MaleficusConsts.START_APP_STATES;
        // 2) Define "debugStateID" in Awake() of child class
        debugStateID = 51;
    }

    protected override void Start()
    {
        base.Start();

        if (MotherOfManagers.Instance.IsServer == true)
        {
            currentScene = EScene.GAME;
        }

        // 3) Bind event in start method of child class!
        StateUpdateEvent += EventManager.Instance.APP_AppStateUpdated.Invoke;

        SceneManager.sceneLoaded += On_SceneLoaded;

        EventManager.Instance.GAME_GameStarted += On_GAME_GameStarted;
        EventManager.Instance.GAME_GamePaused += On_GAME_GamePaused;
        EventManager.Instance.GAME_GameUnPaused += On_GAME_GameUnPaused;
        EventManager.Instance.GAME_GameEnded += On_GAME_GameEnded;

        EventManager.Instance.NETWORK_ReceivedMessageUpdated += On_NETWORK_ReceivedMessageUpdated;
        EventManager.Instance.NETWORK_GameStarted.AddListener                (On_NETWORK_GameStarted);
    }

    private void On_NETWORK_GameStarted(NetEvent_GameStarted obj)
    {
        UpdateState(EAppState.IN_GAME_IN_RUNNING);
    }

    public override void OnSceneStartReinitialize()
    {
        FindAndBindButtonActions();
    }

    protected override void Update()
    {
        base.Update();


        DebugManager.Instance.Log(52, CurrentScene.ToString());
    }


    protected override void UpdateState(EAppState newAppState)
    {
        // Update state
        base.UpdateState(newAppState);

        if (newAppState.ContainedIn(MaleficusConsts.APP_STATES_THAT_TRIGGER_SCENE_CHANGE))
        {
            EScene newScene = MaleficusConsts.FROM_SCENE_TO[CurrentScene];

            UpdateScene(newScene);
        }


        //TODO  if connected before scene loaded [Leon]
        /*
        if (CurrentState == EAppState.IN_ENTRY)
        {
            List<AbstractNetMessage> msgs = NetworkManager.Instance.AllReceivedMsgs;
            if ((msgs != null) && (msgs.Count != 0))
            {
                if (msgs[NetworkManager.Instance.AllReceivedMsgs.Count - 1].ID == ENetMessageID.CONNECTED)
                {
                    UpdateState(EAppState.IN_ENTRY_IN_LOGIN);
                }
            }
        }*/
    }

    #region Scene Update
    private void UpdateScene(EScene newScene)
    {
        EventManager.Instance.APP_SceneWillChange.Invoke(new Event_GenericHandle<EScene>(newScene));

        // Wait some frames before changing scene
        StartCoroutine(LateUpdateSceneCoroutine(newScene));
    }

    private IEnumerator LateUpdateSceneCoroutine(EScene newScene)
    {
        int frameCounter = 0;
        while (frameCounter < MaleficusConsts.NUMBERS_OF_FRAMES_TO_WAIT_BEFORE_CHANGING_SCENE)
        {
            frameCounter++;
            yield return new WaitForEndOfFrame();
        }

        LoadScene(newScene);
    }

    private void LoadScene(EScene sceneToLoad)
    {
        switch (sceneToLoad)
        {
            case EScene.ENTRY:
                SceneManager.LoadScene(MaleficusConsts.SCENE_ENTRY);
                currentScene = EScene.ENTRY;
                break;
            case EScene.MENU:
                SceneManager.LoadScene(MaleficusConsts.SCENE_MENU);
                currentScene = EScene.MENU;
                break;

            case EScene.GAME:
                SceneManager.LoadScene(MaleficusConsts.SCENE_GAME);
                currentScene = EScene.GAME;
                break;
            case EScene.MENU_DUNGEON:
                SceneManager.LoadScene(MaleficusConsts.SCENE_DUNGEON_SELECTION);
                currentScene = EScene.MENU_DUNGEON;
                break;

            default:
                Debug.LogError(sceneToLoad + " is not a valid scene to load!");
                break;
        }
    }
    #endregion


    private void On_NETWORK_ReceivedMessageUpdated(ENetworkMessageType receivedMsg)
    {
        // if, to prevent scene change through loss of connection during game
        if (CurrentState == EAppState.IN_ENTRY || CurrentState == EAppState.IN_ENTRY_IN_LOGIN)  // Added this to prevent change of Menu outside correct context
        {
            switch (receivedMsg)
            {
                case ENetworkMessageType.CONNECTED:
                    UpdateState(EAppState.IN_ENTRY_IN_LOGIN);
                    break;
                case ENetworkMessageType.LOGGED_IN:
                    UpdateState(EAppState.IN_ENTRY_IN_LOADING);
                    break;
                case ENetworkMessageType.REGISTERED:
                    UpdateState(EAppState.IN_ENTRY_IN_LOADING);
                    break;
            }
        }

        switch (receivedMsg)
        {
            case ENetworkMessageType.DATA_ONINITLOBBY:
                UpdateState(EAppState.IN_MENU_IN_STARTING_GAME);
                break;
        }
    }

    private void FindAndBindButtonActions()
    {
        // Connect Players Action
        StartConnectingPlayersAction[] connectPlayersActions = FindObjectsOfType<StartConnectingPlayersAction>();
        foreach (StartConnectingPlayersAction action in connectPlayersActions)
        {
            action.ActionButtonPressed += () =>
            {
                UpdateState(EAppState.IN_MENU_IN_CONNECTING_GAMEPADS);
            };
        }

        // Back to last state
        BackAction[] backActions = FindObjectsOfType<BackAction>();
        foreach (BackAction action in backActions)
        {
            if (action.IsStaysInSameAppState == false)
            {
                action.ActionButtonPressed += () =>
                {
                    UpdateState(LastState);
                };
            }
        }

        // Launch game (change scene)
        PlayAction[] playActions = FindObjectsOfType<PlayAction>();
        foreach (PlayAction action in playActions)
        {
            action.ActionButtonPressed += () =>
            {
                UpdateState(EAppState.IN_MENU_IN_STARTING_GAME);
            };
        }

          // Launch AR game (change scene)
        foreach (StartDungeonAction action in FindObjectsOfType<StartDungeonAction>())
        {
            action.StartDungeonPressed += (EDungeonID dungeonID) =>
            {
                Debug.Log("Start dungeon " + dungeonID + "pressed");
                dungeonIDtoLoad = dungeonID;
                UpdateState(EAppState.IN_MENU_IN_STARTING_AR_GAME);
            };
        }

        foreach(StartGameAction action in FindObjectsOfType<StartGameAction>())
        {
            action.ActionButtonPressed += () =>
            {
                EClientID clientID = NetworkManager.Instance.OwnClientID;
                NetEvent_GameStarted gameStarted = new NetEvent_GameStarted(clientID);
                EventManager.Instance.NETWORK_GameStarted.Invoke(gameStarted, EEventInvocationType.TO_SERVER_ONLY);
            };
        }
    }


    #region Event Callbacks
    private void On_SceneLoaded(Scene newScene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("Loading level done : " + newScene.name);
        EventManager.Instance.APP_SceneChanged.Invoke(new Event_GenericHandle<EScene>(CurrentScene));

        // Validity test
        if ((newScene.name != MaleficusConsts.SCENE_GAME)
            && (newScene.name != MaleficusConsts.SCENE_MENU)
            && (newScene.name != MaleficusConsts.SCENE_ENTRY)
            && (newScene.name != MaleficusConsts.SCENE_DUNGEON_SELECTION)
            )
        {
            Debug.LogError("Loaded level doesn't match to build levels");
        }
    }

    private void On_GAME_GameEnded(EGameMode obj, bool wasAborted)
    {
        StartCoroutine(EndGameCoroutine());
    }
    private IEnumerator EndGameCoroutine()
    {
        UpdateState(EAppState.IN_GAME_IN_ENDED);
        yield return new WaitForSeconds(MaleficusConsts.ENG_GAME_SCENE_TRANSITION_DURATION);
        UpdateState(EAppState.IN_GAME_IN_END_SCENE);
    }


    private void On_GAME_GameUnPaused(EGameMode obj)
    {
        UpdateState(EAppState.IN_GAME_IN_RUNNING);
    }

    private void On_GAME_GamePaused(EGameMode obj)
    {
        UpdateState(EAppState.IN_GAME_IN_PAUSED);
    }

    private void On_GAME_GameStarted(EGameMode obj)
    {
        UpdateState(EAppState.IN_GAME_IN_RUNNING);
    }
    #endregion


    public void SetUpDebugStartScene(EScene startedScene)
    {
        currentScene = startedScene;
    }


}