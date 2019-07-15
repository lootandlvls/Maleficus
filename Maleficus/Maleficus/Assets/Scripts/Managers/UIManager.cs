using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : AbstractSingletonManagerWithStateMachine<UIManager, EMenuState>
{
    private MaleficusButton selectedButton;                                                                         // TODO: Update selected button on menu change

    protected override void Awake()
    {
        base.Awake();

        // 1) Assign appropriate currentState from MaleficusTypes
        startStates = MaleficusTypes.START_MENU_STATES;
        // 2) Define "debugStateID" in Awake() of child class
        debugStateID = 50;
    }

    protected override void Start()
    {
        base.Start();

        //// Debug state                                                                                            // TODO: Use debug states?
        //if (MotherOfManagers.Instance.DebugStartMenuState != EMenuState.NONE)
        //{
        //    startState = MotherOfManagers.Instance.DebugStartMenuState;
        //}

        // 3) Bind event in start method of child class!
        StateUpdateEvent += EventManager.Instance.Invoke_UI_MenuStateUpdated;

        EventManager.Instance.NETWORK_ReceivedMessageUpdated += On_NETWORK_ReceivedMessageUpdated;

        EventManager.Instance.INPUT_ButtonPressed += On_INPUT_ButtonPressed;

        EventManager.Instance.GAME_GameStarted += On_GAME_GameStarted;
        EventManager.Instance.GAME_GamePaused += On_GAME_GamePaused;
        EventManager.Instance.GAME_GameUnPaused += On_GAME_GameUnPaused;
        EventManager.Instance.GAME_GameEnded += On_GAME_GameEnded;
    }

   
    /// <summary>
    /// When a pressed is highlighted (selected by controller)
    /// </summary>
    /// <param name="selectedButton"></param>
    public void OnSelectedButton(MaleficusButton selectedButton)
    {
        this.selectedButton = selectedButton;
    }



    protected override void FindAndBindButtonActions()
    {
        // Menu Navigation Action
        BackAction[] backActions = FindObjectsOfType<BackAction>();
        foreach (BackAction Action in backActions)
        {
            Action.ActionButtonPressed += () =>
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
        foreach (OpenLoginPopUpAction Action in OLPUActions)
        {
            Action.ActionButtonPressed += () =>
            {
                UpdateState(EMenuState.IN_LOGIN_IN_LOGIN);
            };
        }


        OpenRegisterPopUpAction[] ORPActions = FindObjectsOfType<OpenRegisterPopUpAction>();
        foreach (OpenRegisterPopUpAction Action in ORPActions)
        {
            Action.ActionButtonPressed += () =>
            {
                UpdateState(EMenuState.IN_LOGIN_IN_REGISTER);

            };
        }

        GoBackToLoginAction[] GBLActions = FindObjectsOfType<GoBackToLoginAction>();
        foreach (GoBackToLoginAction Action in GBLActions)
        {
            Action.ActionButtonPressed += () =>
            {
                UpdateState(EMenuState.IN_LOGIN);


            };
        }

        LoginRequestAction[] LRActions = FindObjectsOfType<LoginRequestAction>();
        foreach (LoginRequestAction Action in LRActions)
        {
            Action.ActionButtonPressed += () =>
            {
                LoginContext.Instance.OnClickLoginRequest();

            };
        }

        RegisterRequestAction[] RRActions = FindObjectsOfType<RegisterRequestAction>();
        foreach (RegisterRequestAction Action in RRActions)
        {
            Action.ActionButtonPressed += () =>
            {
                RegisterContext.Instance.OnClickCreateAccount();


            };
        }

        AddFriendAction[] AFActions = FindObjectsOfType<AddFriendAction>();
        foreach (AddFriendAction Action in AFActions)
        {
            Action.AddFriendActionPressed += () =>
            {
                FriendsContext.Instance.OnClickAddFollow();
            };
        }

        InitLobbyAction[] ILActions = FindObjectsOfType<InitLobbyAction>();
        foreach (InitLobbyAction Action in ILActions)
        {
            Action.ActionButtonPressed += () =>
            {
                NetworkManager.Instance.SendInitLobby();
            };
        }
    }
        
    #region Events Callbacks
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
            MaleficusButton nextButton = null;
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

    private void On_GAME_GameEnded(EGameMode obj, bool wasAborted)
    {
        UpdateState(EMenuState.IN_GAME_OVER);
    }
    private void On_GAME_GameUnPaused(EGameMode obj)
    {
        UpdateState(EMenuState.IN_GAME_RUNNING);
    }
    private void On_GAME_GamePaused(EGameMode obj)
    {
        UpdateState(EMenuState.IN_GAME_PAUSED);
    }

    private void On_GAME_GameStarted(EGameMode obj)
    {
        UpdateState(EMenuState.IN_GAME_RUNNING);
    }
    #endregion

        
}

