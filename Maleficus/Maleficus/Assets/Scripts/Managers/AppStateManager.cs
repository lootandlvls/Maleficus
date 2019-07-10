using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class AppStateManager : AbstractSingletonManagerWithStateMachine<AppStateManager, EAppState>
{
    public bool IsInAStateWithUI        { get { return CurrentState.ContainedIn(MaleficusTypes.APP_STATES_WITH_UI) == true; } }                            // TODO: use this

    public EScene CurrentScene { get { return currentScene; } }  

    private EScene currentScene = EScene.NONE;
    private List<AbstractUIAction> boundActions = new List<AbstractUIAction>();


    #region Unity Functions

    protected override void Awake()
    {
        base.Awake();

        startState = MotherOfManagers.Instance.DebugStartState;
        debugStateID = 51;
    }
    protected override void Start()
    {
        base.Start();

        // Bind state machine event
        StateUpdateEvent += EventManager.Instance.Invoke_APP_AppStateUpdated;
    }

    protected override void Update()
    {
        base.Update();

        DebugManager.Instance.Log(52, CurrentScene.ToString());
    }
#endregion

    #region Inherited Members
    public override void Initialize()
    {
        base.Initialize();

        Debug.Log("Initializing AppStateManager");
        FindAndBindButtonActions();
    }

    protected override void UpdateState(EAppState newAppState)
    {
        // Has scene changed?
        if (newAppState.ContainedIn(MaleficusTypes.APP_STATES_IN_GAME) && (CurrentScene != EScene.GAME))
        {
            UpdateScene(EScene.GAME);
        }
        else if (newAppState.ContainedIn(MaleficusTypes.APP_STATES_IN_MENU) && (CurrentScene != EScene.MENU))
        {
            UpdateScene(EScene.MENU);
        }
        
        // Update state
        base.UpdateState(newAppState);
    }

    private void UpdateScene(EScene newScene)
    {
        Debug.Log("Update scene");
        currentScene = newScene;

        EventManager.Instance.Invoke_APP_SceneWillChange(newScene);

        // Wait some frames before changing scene
        StartCoroutine(LateUpdateSceneCoroutine(newScene));
    }

    private IEnumerator LateUpdateSceneCoroutine(EScene newScene)
    {
        int frameCounter = 0;
        while (frameCounter != MaleficusTypes.NUMBERS_OF_FRAMES_TO_WAIT_BEFORE_CHANGING_SCENE)
        {
            frameCounter++;
            yield return new WaitForEndOfFrame();
        }


    }

#endregion

    private void LoadScene(EScene sceneToLoad)
    {
        switch (sceneToLoad)
        {
            case EScene.MENU:
                SceneManager.LoadScene(MaleficusTypes.SCENE_MENU);
                EventManager.Instance.Invoke_APP_SceneChanged(sceneToLoad);
                break;

            case EScene.GAME:
                SceneManager.LoadScene(MaleficusTypes.SCENE_GAME);
                EventManager.Instance.Invoke_APP_SceneChanged(sceneToLoad);
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

        // Start game (change scene)
        StartGameAction[] startGameActions = FindObjectsOfType<StartGameAction>();
        foreach (StartGameAction action in startGameActions)
        {
            action.ActionButtonPressed += () =>
            {
                UpdateState(EAppState.IN_GAME_IN_ABOUT_TO_START);
            };
        }
    }
}
