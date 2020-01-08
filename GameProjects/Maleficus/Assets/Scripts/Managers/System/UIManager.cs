using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Maleficus.MaleficusUtilities;
using static Maleficus.MaleficusConsts;


public class UIManager : AbstractSingletonManagerWithStateMachine<UIManager, EMenuState>
{
    private MaleficusButton highlightedButton;

    protected override void Awake()
    {
        base.Awake();

        // 1) Assign appropriate currentState from MaleficusTypes
        startStates = START_MENU_STATES;
        // 2) Define "debugStateID" in Awake() of child class
        debugStateID = 50;
    }

    protected override void Start()
    {
        base.Start();

        // 3) Bind event in start method of child class!
        StateUpdateEvent += EventManager.Instance.UI_MenuStateUpdated.Invoke;

        EventManager.Instance.NETWORK_ReceivedMessageUpdated += On_NETWORK_ReceivedMessageUpdated;
        EventManager.Instance.APP_AppStateUpdated.AddListener(On_APP_AppStateUpdated);

        EventManager.Instance.INPUT_ButtonPressed.AddListener       (On_INPUT_ButtonPressed);

        EventManager.Instance.GAME_GameStarted += On_GAME_GameStarted;
        EventManager.Instance.GAME_GamePaused += On_GAME_GamePaused;
        EventManager.Instance.GAME_GameUnPaused += On_GAME_GameUnPaused;
        EventManager.Instance.GAME_GameEnded += On_GAME_GameEnded;
    }


    protected override void OnReinitializeManager()
    {
        base.OnReinitializeManager();

        FindAndBindButtonActions();
    }


    /// <summary>
    /// Called from a MaleficusButton whenever it gets highlighted (selected)
    /// </summary>
    /// <param name="selectedButton"> calling MaleficusButton </param>
    public void OnButtonHighlighted(MaleficusButton selectedButton)
    {
        this.highlightedButton = selectedButton;
    }

    protected override void UpdateState(EMenuState newMenuState)
    {
        // Update state
        base.UpdateState(newMenuState);
    }


        
    #region Events Callbacks
    private void On_INPUT_ButtonPressed(NetEvent_ButtonPressed eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EPlayerID playerID = GetPlayerIDFrom(eventHandle.SenderID);

        if ((highlightedButton != null)
             && (AppStateManager.Instance.CurrentState.ContainedIn(APP_STATES_THAT_TRIGGER_SCENE_CHANGE) == false)
             && (AppStateManager.Instance.CurrentState!= EAppState.IN_MENU_IN_SPELL_SELECTION)) // TODO : remove when multiplayer UI is defined
        {

            MaleficusButton nextButton = null;
            switch (inputButton)
            {
                case EInputButton.CONFIRM:
                    highlightedButton.Press();
                    break;

                case EInputButton.LEFT:
                    nextButton = highlightedButton.GetNextButton(EButtonDirection.LEFT);
                    break;

                case EInputButton.RIGHT:
                    nextButton = highlightedButton.GetNextButton(EButtonDirection.RIGHT);
                    break;

                case EInputButton.UP:
                    nextButton = highlightedButton.GetNextButton(EButtonDirection.UP);
                    break;

                case EInputButton.DOWN:
                    nextButton = highlightedButton.GetNextButton(EButtonDirection.DOWN);
                    break;
            }

            // Update highlighted button
            if (nextButton != null)
            {
                highlightedButton = nextButton;
                nextButton.Highlight();
            }
        }
    }

    private void On_NETWORK_ReceivedMessageUpdated(ENetworkMessageType receivedMsg)
    {
        // if, to prevent scene change through loss of connection during game
        if (AppStateManager.Instance.CurrentState == EAppState.IN_ENTRY || AppStateManager.Instance.CurrentState == EAppState.IN_ENTRY_IN_LOGIN)  // Added this to prevent change of Menu outside correct context // TODO: Make sure to switch to "IN_MENU_LOGING_IN" before when the following code is needed 
        {
            switch (receivedMsg)
            {
                case ENetworkMessageType.CONNECTED:
                    if (currentState == EMenuState.IN_ENTRY)
                    {
                        UpdateState(EMenuState.IN_ENTRY_IN_LOGIN);
                    }
                    break;
                case ENetworkMessageType.LOGGED_IN:
                    UpdateState(EMenuState.IN_MENU_MAIN);
                    break;
                case ENetworkMessageType.REGISTERED:
                    UpdateState(EMenuState.IN_MENU_MAIN);
                    break;
            }
        }
    }

    public void On_APP_AppStateUpdated(Event_StateUpdated<EAppState> eventHandle)
    {
        if(currentState == EMenuState.IN_ENTRY)
        {
            switch (eventHandle.NewState)
            {
                case EAppState.IN_ENTRY_IN_LOGIN:
                    UpdateState(EMenuState.IN_ENTRY_IN_LOGIN);
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
        UpdateState(EMenuState.IN_GAME_IN_RUNNING);
    }
    private void On_GAME_GamePaused(EGameMode obj)
    {
        UpdateState(EMenuState.IN_GAME_IN_PAUSED);
    }

    private void On_GAME_GameStarted(EGameMode obj)
    {
        UpdateState(EMenuState.IN_GAME_IN_RUNNING);
    }
    #endregion

    private void FindAndBindButtonActions()
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
                UpdateState(EMenuState.IN_ENTRY_IN_LOGIN_IN_LOGIN);
            };
        }


        OpenRegisterPopUpAction[] ORPActions = FindObjectsOfType<OpenRegisterPopUpAction>();
        foreach (OpenRegisterPopUpAction Action in ORPActions)
        {
            Action.ActionButtonPressed += () =>
            {
                UpdateState(EMenuState.IN_ENTRY_IN_LOGIN_IN_REGISTER);

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
                AutoAccountContext.Instance.OnClickCreateAccount();
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

        UpdateAccountRequestAction[] UARActions = FindObjectsOfType<UpdateAccountRequestAction>();
        foreach (UpdateAccountRequestAction Action in UARActions)
        {
            Action.ActionButtonPressed += () =>
            {
                AutoAccountContext.Instance.OnClickSaveCredentials();
            };
        }
    }
}

