using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : SingletonStateMachine<UIManager, EMenuState>
{
    private MenuButton selectedButton;                                                                         // TODO: Update selected button on menu change

    protected override void Awake()
    {
        base.Awake();

        startState = EMenuState.MAIN;                                                                           // TODO: for testing. Change to correct one later
        debugStateID = 50;

        FindAndBindButtonCommands();
    }

    protected override void Start()
    {
        base.Start();

        // Bind state machine event
        StateUpdateEvent += EventManager.Instance.Invoke_UI_MenuStateUpdated;

        EventManager.Instance.INPUT_ButtonPressed += On_INPUT_ButtonPressed;

        StartCoroutine(LateStartCoroutine());
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

    public void OnSelectedButton(MenuButton selectedButton)
    {
        this.selectedButton = selectedButton;
    }

    private void FindAndBindButtonCommands()
    {
        // Menu Navigation Command
        MenuNavigationCommand[] commands = FindObjectsOfType<MenuNavigationCommand>();
        foreach (MenuNavigationCommand command in commands)
        {
            command.MenuNavigationCommandPressed += UpdateState;
        }
    }

}
