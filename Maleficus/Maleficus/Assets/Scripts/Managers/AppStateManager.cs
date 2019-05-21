using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AppStateManager : Singleton<AppStateManager>
{
    private EAppState[] STATES_WITH_UI = new EAppState[] { EAppState.IN_MENU/*, AppState.CONNECTING_PLAYERS*/ };

    public EAppState CurrentAppState     { get { return currentAppState; } }
    public bool IsInAStateWithUI       { get { return isInAStateWithUI; } }

    [SerializeField] private EAppState debugStartState;
    private bool isInAStateWithUI = false;

    private EAppState currentAppState;

    private void Start()
    {
        UpdateAppState(debugStartState);
    }


    private void UpdateAppState(EAppState newAppState)
    {
        currentAppState = newAppState;

        if (newAppState.ContainedIn(STATES_WITH_UI))
        {
            Debug.Log("Is in state wit UI");
            isInAStateWithUI = true;
        }
        else
        {
            Debug.Log("Is NOT in state wit UI");
            isInAStateWithUI = false;
        }

        EventManager.Instance.Invoke_GAME_AppStateUpdated(newAppState);


    
    }
}
