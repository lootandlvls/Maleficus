using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuState
{
    NONE,
    MAIN,
    IN_GAME
}

public enum ButtonDirection
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public class UIManager : Singleton<UIManager>
{
    public MenuState CurrentState { get { return currentState; } }
    private MenuState currentState = MenuState.NONE;
    private MenuState lastState = MenuState.NONE;

    private MenuButton selectedButton;                                                                         // TODO: Update selected button on menu change

    private void Start()
    {
        EventManager.Instance.INPUT_ButtonPressed += On_INPUT_ButtonPressed;

        StartCoroutine(LateStartCoroutine());
    }

    private void On_INPUT_ButtonPressed(InputButton buttonType, int playerID)
    { 
        MenuButton nextButton = null;
        switch (buttonType)
        {
            case InputButton.CONFIRM:
                selectedButton.Press();
                break;

            case InputButton.LEFT:
                nextButton = selectedButton.GoToNextButton(ButtonDirection.LEFT);
                break;

            case InputButton.RIGHT:
                nextButton = selectedButton.GoToNextButton(ButtonDirection.RIGHT);
                break;

            case InputButton.UP:
                nextButton = selectedButton.GoToNextButton(ButtonDirection.UP);
                break;

            case InputButton.DOWN:
                nextButton = selectedButton.GoToNextButton(ButtonDirection.DOWN);
                break;
        }

        // Update selected button
        if (nextButton != null)
        {
            selectedButton = nextButton;
        }
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
        EventManager.Instance.UI_InvokeMenuStateUpdated(newState, lastState);
    }

    public void OnSelectedButton(MenuButton selectedButton)
    {
        this.selectedButton = selectedButton;
    }
}
