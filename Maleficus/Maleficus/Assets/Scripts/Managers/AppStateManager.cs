﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class AppStateManager : AbstractSingletonManagerWithStateMachine<AppStateManager, EAppState>
{
    public bool IsInAStateWithUI { get { return CurrentState.ContainedIn(MaleficusTypes.APP_STATES_WITH_UI) == true; } }                            // TODO: use this
    public EScene CurrentScene { get { return currentScene; } }  //return GetScene(CurrentState); } }


    private EScene currentScene = EScene.ENTRY;

    private List<AbstractUIAction> boundActions = new List<AbstractUIAction>();


    #region Unity Functions
    protected override void Awake()
    {
        base.Awake();


        FindAndBindButtonActions();


        // 1) Assign appropriate currentState from MaleficusTypes
        startStates = MaleficusTypes.START_APP_STATES;
        // 2) Define "debugStateID" in Awake() of child class

        debugStateID = 51;
    }

    protected override void Start()
    {
        base.Start();


        startState = MotherOfManagers.Instance.DebugStartState;


        // Bind state machine event
        StateUpdateEvent += EventManager.Instance.Invoke_GAME_AppStateUpdated;

        //// Debug state                                                                                                                // TODO: Use debug states?
        //if (MotherOfManagers.Instance.DebugStartMenuState != EMenuState.NONE)
        //{
        //    startState = MotherOfManagers.Instance.DebugStartAppState;
        //}

        // 3) Bind event in start method of child class!
        StateUpdateEvent += EventManager.Instance.Invoke_APP_AppStateUpdated;

        SceneManager.sceneLoaded += On_SceneLoaded;

        EventManager.Instance.GAME_GameStarted += On_GAME_GameStarted;
        EventManager.Instance.GAME_GamePaused += On_GAME_GamePaused;
        EventManager.Instance.GAME_GameUnPaused += On_GAME_GameUnPaused;
        EventManager.Instance.GAME_GameEnded += On_GAME_GameEnded;
    }

    protected override void Update()
    {
        base.Update();


        DebugManager.Instance.Log(52, CurrentScene.ToString());
    }
    #endregion


    protected override void UpdateState(EAppState newAppState)
    {
        // Update state
        base.UpdateState(newAppState);

        Debug.Log("Updated state : " + LastState + " -> " + CurrentState + " : " + CurrentScene);

        //// Is in entry scene                                                                                                              // TODO: Use debug entry start state
        //if (CurrentScene == EScene.ENTRY && MotherOfManagers.Instance.StartSceneOnEntry != EScene.NONE)
        //{
        //    Debug.Log("Mother of zebbi says : " + MotherOfManagers.Instance.StartSceneOnEntry);
        //    UpdateScene(MotherOfManagers.Instance.StartSceneOnEntry);
        //}
        //// Has scene changed
        //else
        //{

        if (newAppState.ContainedIn(MaleficusTypes.APP_STATES_THAT_TRIGGER_SCENE_CHANGE))
        {
            EScene newScene = MaleficusTypes.FROM_SCENE_TO[CurrentScene];
            UpdateScene(newScene);
        }
    }

    private void UpdateScene(EScene newScene)
    {
        Debug.Log("Update scene from " + CurrentScene + " to " + newScene);

        EventManager.Instance.Invoke_APP_SceneWillChange(newScene);

        // Wait some frames before changing scene
        StartCoroutine(LateUpdateSceneCoroutine(newScene));
    }

    private IEnumerator LateUpdateSceneCoroutine(EScene newScene)
    {
        int frameCounter = 0;
        while (frameCounter < MaleficusTypes.NUMBERS_OF_FRAMES_TO_WAIT_BEFORE_CHANGING_SCENE)
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
            case EScene.MENU:
                SceneManager.LoadScene(MaleficusTypes.SCENE_MENU);
                currentScene = EScene.MENU;
                break;

            case EScene.GAME:
                SceneManager.LoadScene(MaleficusTypes.SCENE_GAME);
                currentScene = EScene.GAME;
                break;

            default:
                Debug.LogError(sceneToLoad + " is not a valid scene to load!");
                break;
        }
    }

    protected override void FindAndBindButtonActions()
    {
        base.FindAndBindButtonActions();

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
    }


    #region Event Callbacks
    private void On_SceneLoaded(Scene newScene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("Loading level done : " + newScene.name);
        EventManager.Instance.Invoke_APP_SceneChanged(CurrentScene);

        // Validity test
        if ((newScene.name != MaleficusTypes.SCENE_GAME)
            && (newScene.name != MaleficusTypes.SCENE_MENU)
            && (newScene.name != MaleficusTypes.SCENE_ENTRY))
        {
            Debug.LogError("Loaded level doesn't match to build levels");
        }
    }

    private void On_GAME_GameEnded(EGameMode obj, bool wasAborted)
    {
        UpdateState(EAppState.IN_GAME_IN_ENDED);
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