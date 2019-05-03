using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuState
{
    NONE,
    MAIN,
    IN_GAME
}

public class UIManager : Singleton<UIManager>
{
    public MenuState CurrentState { get { return currentState; } }
    private MenuState currentState = MenuState.NONE;
    private MenuState lastState = MenuState.NONE;

    private void Start()
    {
        StartCoroutine(LateStartCoroutine());
    }
    private IEnumerator LateStartCoroutine()
    {
        yield return new WaitForEndOfFrame();
        UpdateState(MenuState.MAIN);
    }

    private void Update()
    {
        DebugManager.Instance.Log(1, "Menu State : " + currentState);
    }

    public void UpdateState(MenuState newState)
    {
        if (newState == currentState)
        {
            return;
        }
        lastState = currentState;
        currentState = newState;
        EventManager.Instance.InvokeMenuStateUpdated(newState, lastState);
    }
}
