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

        FindAndBindButtonCommands();
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

    private void FindAndBindButtonCommands()
    {
        // Menu Navigation Command
        MenuNavigationCommand[] MNCcommands = FindObjectsOfType<MenuNavigationCommand>();
        foreach (MenuNavigationCommand command in MNCcommands)
        {
            command.MenuNavigationCommandPressed += UpdateState;
        }

        OpenLoginPopUpCommand[] OLPUcommands = FindObjectsOfType<OpenLoginPopUpCommand>();
        foreach(OpenLoginPopUpCommand command in OLPUcommands)
        {
            command.OpenLoginPopUpCommandPressed += On_OpenLoginPopUpCommandPressed;
        }

        OpenRegisterPopUpCommand[] ORPcommands = FindObjectsOfType<OpenRegisterPopUpCommand>();
        foreach(OpenRegisterPopUpCommand command in ORPcommands)
        {
            command.OpenRegisterPopUpCommandPressed += On_OpenRegisterPopUpCommandPressed;
        }

        GoBackToLoginCommand[] GBLcommands = FindObjectsOfType<GoBackToLoginCommand>();
        foreach(GoBackToLoginCommand command in GBLcommands)
        {
            command.GoBackToLoginCommandPressed += On_GoBackToLoginCommandPressed;
        }

        LoginRequestCommand[] LRcommands = FindObjectsOfType<LoginRequestCommand>();
        foreach(LoginRequestCommand command in LRcommands)
        {
            command.LoginRequestCommandPressed += On_LoginRequestCommandPressed;
        }

        RegisterRequestCommand[] RRcommands = FindObjectsOfType<RegisterRequestCommand>();
        foreach(RegisterRequestCommand command in RRcommands)
        {
            command.RegisterRequestCommandPressed += On_RegisterRequestCommandPressed;
        }
    }

    private void On_RegisterRequestCommandPressed()
    {
        RegisterContext.Instance.OnClickCreateAccount();
    }

    private void On_LoginRequestCommandPressed()
    {
        LoginContext.Instance.OnClickLoginRequest();
    }

    private void On_GoBackToLoginCommandPressed()
    {
        UpdateState(EMenuState.LOGIN);
    }

    private void On_OpenRegisterPopUpCommandPressed()
    {
        UpdateState(EMenuState.LOGIN_REGISTER);
    }

    private void On_OpenLoginPopUpCommandPressed()
    {
        UpdateState(EMenuState.LOGIN_LOGIN);
    }
}
