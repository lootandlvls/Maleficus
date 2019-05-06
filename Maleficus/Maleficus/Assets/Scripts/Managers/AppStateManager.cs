using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AppState
{
    IN_MENU,
    CONNECTING_PLAYERS,
    IN_GAME
}

public class AppStateManager : Singleton<AppStateManager>
{

    public AppState CurrentAppState { get { return currentAppState; } }

    [SerializeField] private AppState debugStartState;

    private AppState currentAppState;

    private void Start()
    {
        UpdateAppState(debugStartState);
    }


    private void UpdateAppState(AppState newAppState)
    {
        currentAppState = newAppState;
        EventManager.Instance.GAME_InvokeAppStateUpdated(newAppState);
    }
}
