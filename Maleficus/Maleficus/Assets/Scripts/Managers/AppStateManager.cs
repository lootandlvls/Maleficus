using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AppStateManager : SingletonStateMachine<AppStateManager, EAppState>
{
    public bool IsInAStateWithUI        { get { return isInAStateWithUI; } }                            // TODO: use this

    private bool isInAStateWithUI = false;

    protected override void Awake()
    {
        base.Awake();

        FindAndBindButtonActions();

        startState = MotherOfManagers.Instance.DebugStartState;
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

        if (newAppState.ContainedIn(MaleficusTypes.APP_STATES_WITH_UI))
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
            Action.ConnectPlayersActionPressed += () =>
            {
                //if (currentState.ContainedIn(MaleficusTypes.APP_STATES_IN_MENU))                               // TODO: Doesn't work
                //{
                    UpdateState(EAppState.IN_MENU_IN_CONNECTING_PLAYERS);
                //}
            };
        }

        BackAction[] backActions = FindObjectsOfType<BackAction>();
        foreach (BackAction Action in backActions)
        {
            Action.BackActionPressed += () =>
            {
                UpdateState(LastState);
            };
        }
    }
}
