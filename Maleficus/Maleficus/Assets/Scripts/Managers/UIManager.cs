using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : Singleton<UIManager>
{
    public EMenuState CurrentState { get { return currentState; } }
    private EMenuState currentState = EMenuState.NONE;
    private EMenuState lastState = EMenuState.NONE;

    private MenuButton selectedButton;                                                                         // TODO: Update selected button on menu change

    private void Start()
    {
        EventManager.Instance.INPUT_ButtonPressed += On_INPUT_ButtonPressed;

        StartCoroutine(LateStartCoroutine());
    }

    private IEnumerator LateStartCoroutine()
    {
        yield return new WaitForEndOfFrame();
        UpdateState(EMenuState.MAIN);
    }

    private void On_INPUT_ButtonPressed(EInputButton buttonType, EPlayerID playerID)
    {
        //Debug.Log("Button " + buttonType + " by " + playerID);
        if (AppStateManager.Instance.IsInAStateWithUI == true)         // test case
        {
            MenuButton nextButton = null;
            switch (buttonType)
            {
                case EInputButton.CONFIRM:
                    selectedButton.Press();
                    break;

                case EInputButton.LEFT:
                    nextButton = selectedButton.GoToNextButton(EButtonDirection.LEFT);
                    break;

                case EInputButton.RIGHT:
                    nextButton = selectedButton.GoToNextButton(EButtonDirection.RIGHT);
                    break;

                case EInputButton.UP:
                    nextButton = selectedButton.GoToNextButton(EButtonDirection.UP);
                    break;

                case EInputButton.DOWN:
                    nextButton = selectedButton.GoToNextButton(EButtonDirection.DOWN);
                    break;
            }

            // Update selected button
            if (nextButton != null)
            {
                selectedButton = nextButton;
            }
        }
    }

    

    private void Update()
    {
        DebugManager.Instance.Log(1, "Menu State : " + currentState);
    }

    public void UpdateState(EMenuState newState)
    {
        if (newState == currentState)
        {
            return;
        }
        lastState = currentState;
        currentState = newState;
        EventManager.Instance.Invoke_UI_MenuStateUpdated(newState, lastState);
    }

    public void OnSelectedButton(MenuButton selectedButton)
    {
        this.selectedButton = selectedButton;
    }
}
