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

        // 3) Bind event in start method of child class!
        StateUpdateEvent += EventManager.Instance.APP_AppStateUpdated.Invoke;

        SceneManager.sceneLoaded += On_SceneLoaded;

        EventManager.Instance.GAME_GameStarted += On_GAME_GameStarted;
        EventManager.Instance.GAME_GamePaused += On_GAME_GamePaused;
        EventManager.Instance.GAME_GameUnPaused += On_GAME_GameUnPaused;
        EventManager.Instance.GAME_GameEnded += On_GAME_GameEnded;

        EventManager.Instance.NETWORK_ReceivedMessageUpdated += On_NETWORK_ReceivedMessageUpdated;
    }

    public override void Initialize()
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
            // For AR 
            if (CurrentState == EAppState.IN_MENU_IN_STARTING_AR_GAME)
            {
                newScene = EScene.AR_GAME;
            }
            UpdateScene(newScene);
        }


        // if connected before scene loaded
        if (CurrentState == EAppState.IN_ENTRY)
        {
            List<AbstractNetMessage> msgs = NetworkManager.Instance.allReceivedMsgs;
            if (msgs.Count != 0)
            {
                if (msgs[NetworkManager.Instance.allReceivedMsgs.Count - 1].ID == NetID.Connected)
                {
                    UpdateState(EAppState.IN_ENTRY_IN_LOGIN);
                }
            }
        }
    }

    #region Scene Update
    private void UpdateScene(EScene newScene)
    {
        EventManager.Instance.APP_SceneWillChange.Invoke(new BasicEventHandle<EScene>(newScene));

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

            case EScene.AR_GAME:
                string ScenePath = "";
                switch (dungeonIDtoLoad)
                {
                    case EDungeonID.ONE:
                        ScenePath = MaleficusConsts.SCENE_DUNGEON_1;
                        break;

                    case EDungeonID.TWO:
                        ScenePath = MaleficusConsts.SCENE_DUNGEON_2;
                        break;

                    case EDungeonID.THREE:
                        ScenePath = MaleficusConsts.SCENE_DUNGEON_3;
                        break;

                    case EDungeonID.NONE:
                        Debug.LogError("Not a valid Dungeon scene selected to load");
                        break;
                }

                if (ScenePath != "")
                {
                    SceneManager.LoadScene(ScenePath);
                    currentScene = EScene.AR_GAME;
                }
                break;

            case EScene.DUNGEON_SELECTION:
                SceneManager.LoadScene(MaleficusConsts.SCENE_DUNGEON_SELECTION);
                currentScene = EScene.DUNGEON_SELECTION;
                break;

            default:
                Debug.LogError(sceneToLoad + " is not a valid scene to load!");
                break;
        }
    }
    #endregion


    private void On_NETWORK_ReceivedMessageUpdated(ENetworkMessage receivedMsg)
    {
        // if, to prevent scene change through loss of connection during game
        if (CurrentState == EAppState.IN_ENTRY || CurrentState == EAppState.IN_ENTRY_IN_LOGIN)  // Added this to prevent change of Menu outside correct context
        {
            switch (receivedMsg)
            {
                case ENetworkMessage.CONNECTED:
                    UpdateState(EAppState.IN_ENTRY_IN_LOGIN);
                    break;
                case ENetworkMessage.LOGGED_IN:
                    UpdateState(EAppState.IN_ENTRY_IN_LOADING);
                    break;
                case ENetworkMessage.REGISTERED:
                    UpdateState(EAppState.IN_ENTRY_IN_LOADING);
                    break;
            }
        }

        switch (receivedMsg)
        {
            case ENetworkMessage.DATA_ONINITLOBBY:
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
                UpdateState(EAppState.IN_MENU_IN_CONNECTING_PLAYERS);
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
    }


    #region Event Callbacks
    private void On_SceneLoaded(Scene newScene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("Loading level done : " + newScene.name);
        EventManager.Instance.APP_SceneChanged.Invoke(new BasicEventHandle<EScene>(CurrentScene));

        // Validity test
        if ((newScene.name != MaleficusConsts.SCENE_GAME)
            && (newScene.name != MaleficusConsts.SCENE_MENU)
            && (newScene.name != MaleficusConsts.SCENE_ENTRY)
            && (newScene.name != MaleficusConsts.SCENE_DUNGEON_SELECTION)
            && (newScene.name != MaleficusConsts.SCENE_DUNGEON_1)
            && (newScene.name != MaleficusConsts.SCENE_DUNGEON_2)
            && (newScene.name != MaleficusConsts.SCENE_DUNGEON_3)
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