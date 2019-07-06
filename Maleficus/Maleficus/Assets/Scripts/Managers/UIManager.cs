using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : SingletonStateMachine<UIManager, EMenuState>
{
    private MenuButton selectedButton;                                                                         // TODO: Update selected button on menu change

    protected override void Awake()
    {
        base.Awake();

        startState = EMenuState.STARTUP;                                                                           // TODO: for testing. Change to correct one later
        debugStateID = 50;

        FindAndBindButtonActions();
    }

    protected override void Start()
    {
        base.Start();

        // Bind state machine event
        StateUpdateEvent += EventManager.Instance.Invoke_UI_MenuStateUpdated;
        EventManager.Instance.NETWORK_ReceivedMessageUpdated += On_NETWORK_ReceivedMessageUpdated;

        EventManager.Instance.INPUT_ButtonPressed += On_INPUT_ButtonPressed;

        StartCoroutine(LateStartCoroutine());
    }

    private void On_NETWORK_ReceivedMessageUpdated(ENetworkMessage receivedMsg, ENetworkMessage lastMsg)
    {
        switch(receivedMsg)
        {
            case ENetworkMessage.CONNECTED:
                UpdateState(EMenuState.LOGIN);
                break;
            case ENetworkMessage.LOGGED_IN:
                UpdateState(EMenuState.MAIN);
                break;
            case ENetworkMessage.REGISTERED:
                UpdateState(EMenuState.LOGIN);
                break;
        }
    }

    private void On_INPUT_ButtonPressed(EInputButton buttonType, EPlayerID playerID)
    {
        //Debug.Log("Button " + buttonType + " by " + playerID);
        if (AppStateManager.Instance.IsInAStateWithUI == true)         // test case
        {
            if (selectedButton == null)
            {
                Debug.Log("selected button null");
                return;
            }
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

    private void FindAndBindButtonActions()
    {
        // Menu Navigation Action
        MenuNavigationAction[] MNCActions = FindObjectsOfType<MenuNavigationAction>();
        foreach (MenuNavigationAction Action in MNCActions)
        {
            Action.MenuNavigationActionPressed += UpdateState;
        }

        OpenLoginPopUpAction[] OLPUActions = FindObjectsOfType<OpenLoginPopUpAction>();
        foreach(OpenLoginPopUpAction Action in OLPUActions)
        {
            Action.OpenLoginPopUpActionPressed += On_OpenLoginPopUpActionPressed;
        }

        OpenRegisterPopUpAction[] ORPActions = FindObjectsOfType<OpenRegisterPopUpAction>();
        foreach(OpenRegisterPopUpAction Action in ORPActions)
        {
            Action.OpenRegisterPopUpActionPressed += On_OpenRegisterPopUpActionPressed;
        }

        GoBackToLoginAction[] GBLActions = FindObjectsOfType<GoBackToLoginAction>();
        foreach(GoBackToLoginAction Action in GBLActions)
        {
            Action.GoBackToLoginActionPressed += On_GoBackToLoginActionPressed;
        }

        LoginRequestAction[] LRActions = FindObjectsOfType<LoginRequestAction>();
        foreach(LoginRequestAction Action in LRActions)
        {
            Action.LoginRequestActionPressed += On_LoginRequestActionPressed;
        }

        RegisterRequestAction[] RRActions = FindObjectsOfType<RegisterRequestAction>();
        foreach(RegisterRequestAction Action in RRActions)
        {
            Action.RegisterRequestActionPressed += On_RegisterRequestActionPressed;
        }
    }

    private void On_RegisterRequestActionPressed()
    {
        RegisterContext.Instance.OnClickCreateAccount();
    }

    private void On_LoginRequestActionPressed()
    {
        LoginContext.Instance.OnClickLoginRequest();
    }

    private void On_GoBackToLoginActionPressed()
    {
        UpdateState(EMenuState.LOGIN);
    }

    private void On_OpenRegisterPopUpActionPressed()
    {
        UpdateState(EMenuState.LOGIN_REGISTER);
    }

    private void On_OpenLoginPopUpActionPressed()
    {
        UpdateState(EMenuState.LOGIN_LOGIN);
    }
}
