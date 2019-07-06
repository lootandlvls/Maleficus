using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AppStateManager : SingletonStateMachine<AppStateManager, EAppState>
{
    public bool IsInAStateWithUI        { get { return isInAStateWithUI; } }
    public bool IsCanControlPlayers     { get { return true; } } // Todo: use correct context

    [SerializeField] private EAppState debugStartState;
    private bool isInAStateWithUI = false;

    protected override void Awake()
    {
        base.Awake();

        FindAndBindButtonActions();

        startState = debugStartState;
        debugStateID = 51;
    }

    protected override void Start()
    {
        base.Start();

        // Bind state machine event
        StateUpdateEvent += EventManager.Instance.Invoke_GAME_AppStateUpdated;

    }


    protected override void UpdateState(EAppState newAppState)
    {
        base.UpdateState(newAppState);

        if (newAppState.ContainedIn(MaleficusTypes.STATES_WITH_UI))
        {
            Debug.Log("Is in state wit UI");
            isInAStateWithUI = true;
        }
        else
        {
            Debug.Log("Is NOT in state wit UI");
            isInAStateWithUI = false;
        }

    }

    private void FindAndBindButtonActions()
    {
        // Connect Players Action
        StartConnectingPlayersAction[] Actions = FindObjectsOfType<StartConnectingPlayersAction>();
        foreach (StartConnectingPlayersAction Action in Actions)
        {
            Action.ConnectPlayersActionPressed += OnConnectPlayersActionPressed;
        }
    }

    private void OnConnectPlayersActionPressed()
    {
        Debug.Log("AppStateManager: On connect player");
        if (currentState.ContainedIn(MaleficusTypes.STATES_IN_LOBBY))                               // TODO: Doesn't work
        {
            UpdateState(EAppState.IN_LOBBY_CONNECTING_PLAYERS);
        }
    }

    //Todo does this belong here?
    private void OnConnectedToServer()
    {
        Debug.Log("AppStateManager: On connected to server");
        if(currentState == EAppState.IN_STARTUP)
        {
            UpdateState(EAppState.IN_LOGIN);
        }
    }
}
