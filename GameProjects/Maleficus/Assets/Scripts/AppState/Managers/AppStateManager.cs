using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using static Maleficus.Consts;


public class AppStateManager : AbstractSingletonManagerWithStateMachine<AppStateManager, EAppState>
{
    public EScene CurrentScene { get; private set; } = EScene.ENTRY; 
    public bool IsIntroFinished { get { return isIntroFinished; } }

    private EDungeonID dungeonIDtoLoad = EDungeonID.NONE;
    private bool isLoadingScene = false;
    private bool isIntroFinished = false;

    protected override void Awake()
    {
        base.Awake();

        // 1) Assign appropriate currentState from MaleficusTypes
        startStates = START_APP_STATES;
        // 2) Define "debugStateID" in Awake() of child class
        debugStateID = 51;
    }

    protected override void Start()
    {
        base.Start();

        // 3) Bind event in start method of child class!
        StateUpdateEvent += EventManager.Instance.APP_AppStateUpdated.Invoke;
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        SceneManager.sceneLoaded += On_SceneLoaded;

        EventManager.Instance.GAME_CountdownFinished += On_GAME_CountdownFinished;
        EventManager.Instance.GAME_GameStarted += On_GAME_GameStarted;
        EventManager.Instance.GAME_GamePaused += On_GAME_GamePaused;
        EventManager.Instance.GAME_GameUnPaused += On_GAME_GameUnPaused;
        EventManager.Instance.GAME_GameEnded += On_GAME_GameEnded;
        EventManager.Instance.GAME_IntroFinished += On_GAME_IntroFinished;
        EventManager.Instance.NETWORK_ReceivedMessageUpdated += On_NETWORK_ReceivedMessageUpdated;
        EventManager.Instance.NETWORK_GameStarted.AddListener(On_NETWORK_GameStarted);
    }

    private void On_GAME_IntroFinished(bool isReady)
    {
        isIntroFinished = isReady;
    }

    private void On_NETWORK_GameStarted(NetEvent_GameStarted obj)
    {
        UpdateState(EAppState.IN_GAME_IN_RUNNING);
    }

    protected override void OnReinitializeManager()
    {
        base.OnReinitializeManager();

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

        if (newAppState.ContainedIn(APP_STATES_THAT_TRIGGER_SCENE_CHANGE))
        {
            EScene newScene = FROM_SCENE_TO[CurrentScene];

            UpdateScene(newScene);
        }
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
        while (frameCounter < NUMBERS_OF_FRAMES_TO_WAIT_BEFORE_CHANGING_SCENE)
        {
            frameCounter++;
            yield return new WaitForEndOfFrame();
        }

        LoadScene(newScene);
    }

    private void LoadScene(EScene sceneToLoad)
    {
        LogConsole("Scene to load : " + sceneToLoad);
        isLoadingScene = true;
        switch (sceneToLoad)
        {
            case EScene.ENTRY:
                SceneManager.LoadScene(SCENE_ENTRY);
                CurrentScene = EScene.ENTRY;
                break;

            case EScene.MENU:
                if (MotherOfManagers.Instance.ConnectionMode == EConnectionMode.PLAY_OFFLINE)
                {
                    SceneManager.LoadScene(SCENE_MENU_COUCH);
                }
                else
                {
                    SceneManager.LoadScene(SCENE_MENU_MOBILE);
                }
                CurrentScene = EScene.MENU;
                break;

            case EScene.GAME:
                SceneManager.LoadScene(SCENE_GAME);
                CurrentScene = EScene.GAME;
                break;

            case EScene.MENU_DUNGEON:
                SceneManager.LoadScene(SCENE_DUNGEON_SELECTION);
                CurrentScene = EScene.MENU_DUNGEON;
                break;

            default:
                isLoadingScene = false;
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
                case ENetworkMessageType.OFFLINE:
                    UpdateState(EAppState.IN_ENTRY_IN_LOADING);
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
        StartConnectingPlayersUIAction[] connectPlayersActions = FindObjectsOfType<StartConnectingPlayersUIAction>();
        foreach (StartConnectingPlayersUIAction action in connectPlayersActions)
        {
            action.ActionButtonExecuted += () =>
            {
                UpdateState(EAppState.IN_MENU_IN_SPELL_SELECTION);
            };
        }

        // Back to last state
        BackAction[] backActions = FindObjectsOfType<BackAction>();
        foreach (BackAction action in backActions)
        {
            if (action.IsStaysInSameAppState == false)
            {
                action.ActionButtonExecuted += () =>
                {
                    UpdateState(LastState);
                };
            }
        }

        // Launch game (change scene)
        PlayUIAction[] playActions = FindObjectsOfType<PlayUIAction>();
        foreach (PlayUIAction action in playActions)
        {
            action.ActionButtonExecuted += () =>
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

        foreach(StartGameUIAction action in FindObjectsOfType<StartGameUIAction>())
        {
            action.ActionButtonExecuted += () =>
            {
                EClientID clientID = NetworkManager.Instance.OwnerClientID;
                NetEvent_GameStarted gameStarted = new NetEvent_GameStarted(clientID);
                EventManager.Instance.NETWORK_GameStarted.Invoke(gameStarted, EEventInvocationType.TO_SERVER_ONLY);
            };
        }
    }


    #region Event Callbacks
    private void On_SceneLoaded(Scene newScene, LoadSceneMode loadSceneMode)
    {
        if (isLoadingScene)
        {
            Debug.Log("Loading level done : " + newScene.name + " - load mode : " + loadSceneMode);
            EventManager.Instance.APP_SceneChanged.Invoke(new Event_GenericHandle<EScene>(CurrentScene));
            isLoadingScene = false;
        }
        else
        {
            Debug.Log("Prevented double scene loading.");
        }
    }

    private void On_GAME_CountdownFinished()
    {
        UpdateState(EAppState.IN_MENU_IN_STARTING_GAME);
    }

    private void On_GAME_GameEnded(AbstractGameMode gameMode, bool wasAborted)
    {
       StartCoroutine(EndGameCoroutine());
    }
    private IEnumerator EndGameCoroutine()
    {
        UpdateState(EAppState.IN_GAME_IN_ENDED);
        yield return new WaitForSeconds(ENG_GAME_SCENE_TRANSITION_DURATION);
        UpdateState(EAppState.IN_GAME_IN_END_SCENE);
    }


    private void On_GAME_GameUnPaused(AbstractGameMode gameMode)
    {
        UpdateState(EAppState.IN_GAME_IN_RUNNING);
    }

    private void On_GAME_GamePaused(AbstractGameMode gameMode)
    {
        UpdateState(EAppState.IN_GAME_IN_PAUSED);
    }

    private void On_GAME_GameStarted(AbstractGameMode gameMode)
    {

        UpdateState(EAppState.IN_GAME_IN_RUNNING);
    }
    #endregion


    public void SetUpDebugStartScene(EScene startedScene)
    {
        CurrentScene = startedScene;
    }


}