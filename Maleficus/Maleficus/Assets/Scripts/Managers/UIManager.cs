using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : SingletonStateMachine<UIManager, EMenuState>
{
    private MenuButton selectedButton;                                                                         // TODO: Update selected button on menu change

    protected override void Awake()
    {
        base.Awake();

        startState = EMenuState.IN_STARTUP;                                                                        
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
        if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_LOGING_IN)  // Added this to prevent change of Menu outside correct context // TODO: Make sure to switch to "IN_MENU_LOGING_IN" before when the following code is needed 
        {
            switch (receivedMsg)
            {
                case ENetworkMessage.CONNECTED:
                    UpdateState(EMenuState.IN_LOGIN);
                    break;
                case ENetworkMessage.LOGGED_IN:
                    UpdateState(EMenuState.IN_MAIN);
                    break;
                case ENetworkMessage.REGISTERED:
                    UpdateState(EMenuState.IN_LOGIN);
                    break;
            }
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
        BackAction[] backActions = FindObjectsOfType<BackAction>();
        foreach (BackAction Action in backActions)
        {
            Action.BackActionPressed += () =>
            {
                UpdateState(LastState);
            };
        }

        MenuNavigationAction[] MNCActions = FindObjectsOfType<MenuNavigationAction>();
        foreach (MenuNavigationAction Action in MNCActions)
        {
            Action.MenuNavigationActionPressed += UpdateState;
        }

        OpenLoginPopUpAction[] OLPUActions = FindObjectsOfType<OpenLoginPopUpAction>();
        foreach(OpenLoginPopUpAction Action in OLPUActions)
        {
            Action.OpenLoginPopUpActionPressed += () =>
            {
                UpdateState(EMenuState.IN_LOGIN_IN_LOGIN);
            };
        }


        OpenRegisterPopUpAction[] ORPActions = FindObjectsOfType<OpenRegisterPopUpAction>();
        foreach(OpenRegisterPopUpAction Action in ORPActions)
        {
            Action.OpenRegisterPopUpActionPressed += () =>
            {
                UpdateState(EMenuState.IN_LOGIN_IN_REGISTER);

            };
        }

        GoBackToLoginAction[] GBLActions = FindObjectsOfType<GoBackToLoginAction>();
        foreach(GoBackToLoginAction Action in GBLActions)
        {
            Action.GoBackToLoginActionPressed += () =>
            {
                UpdateState(EMenuState.IN_LOGIN);


            };
        }

        LoginRequestAction[] LRActions = FindObjectsOfType<LoginRequestAction>();
        foreach(LoginRequestAction Action in LRActions)
        {
            Action.LoginRequestActionPressed += () =>
            {
                LoginContext.Instance.OnClickLoginRequest();

            };
        }

        RegisterRequestAction[] RRActions = FindObjectsOfType<RegisterRequestAction>();
        foreach(RegisterRequestAction Action in RRActions)
        {
            Action.RegisterRequestActionPressed += () =>
            {
                RegisterContext.Instance.OnClickCreateAccount();


            };
        }

        AddFriendAction[] AFActions = FindObjectsOfType<AddFriendAction>();
        foreach(AddFriendAction Action in AFActions)
        {
            Action.AddFriendActionPressed += () =>
            {
                FriendsContext.Instance.OnClickAddFollow();
            };
        }
    }




    

    
}
