using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.Consts;
using static Maleficus.Utils;

/// <summary>
/// Responsible for managing input controller sources attached to the same gameobject
/// </summary>
public class InputManager : AbstractSingletonManager<InputManager>
{
    public EInputMode                                   InputMode                               { get { return MotherOfManagers.Instance.InputMode; } }

    /// <summary> Mapping from controllerID to playerID </summary> 
    public Dictionary<EControllerID, EPlayerID>         ConnectedControllers                    { get { return connectedControllers; } }


    private Dictionary<EControllerID, EPlayerID>        connectedControllers                    = new Dictionary<EControllerID, EPlayerID>();
    AbstractInputSource[] inputSources;

    protected override void InitializeObjecsInScene()
    {
        base.InitializeObjecsInScene();

        inputSources = GetComponents<AbstractInputSource>();
        foreach (AbstractInputSource inputSource in inputSources)
        {
            inputSource.ButtonPressed += On_InputSource_ButtonPressed;
            inputSource.ButtonReleased += On_InputSource_ButtonReleased;
            inputSource.JoystickMoved += On_InputSource_JoystickMoved;
        }
    }


    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.NETWORK_ReceivedGameSessionInfo.AddListener       (On_NETWORK_ReceivedGameSessionInfo);
        EventManager.Instance.GAME_GameOver.AddListener                         (On_GAME_GameOver);
    }

    protected override void LateStart()
    {
        base.LateStart();

        // Connect Touch player as first player
        if ((MotherOfManagers.Instance.InputMode == EInputMode.TOUCH)
            && (MotherOfManagers.Instance.IsSpawnTouchAsPlayer1 == true))
        {
            ConnectControllerToPlayer(EControllerID.TOUCH, EPlayerID.PLAYER_1);
        }
    }

    private void On_GAME_GameOver(NetEvent_GameOver eventHandle)
    {
        // Disconnect all network controllers and AI
        List<EControllerID> controllerIDsToRemove = new List<EControllerID>();
        foreach (EControllerID controllerID in ConnectedControllers.Keys)
        {
            if (controllerID.ContainedIn(NETWORK_CONTROLLERS)
                || (controllerID == EControllerID.AI_1))
            {
                controllerIDsToRemove.Add(controllerID);
            }
        }
        foreach (EControllerID controllerID in controllerIDsToRemove)
        {
            ConnectedControllers.Remove(controllerID);
        }
    }

    #region Input Source Callbacks
    private void On_InputSource_ButtonPressed(EControllerID controllerID, EInputButton inputButton)
    {
        if (ConnectedControllers.ContainsKey(controllerID))
        {
            EPlayerID playerID = ConnectedControllers[controllerID];
            EClientID clientID = GetClientIDFrom(playerID);

            if (inputButton != EInputButton.NONE)
            {
                NetEvent_ButtonPressed buttonPressed = new NetEvent_ButtonPressed(clientID, inputButton);
                EventManager.Instance.INPUT_ButtonPressed.Invoke(buttonPressed, EEventInvocationType.TO_SERVER_ONLY, MotherOfManagers.Instance.CanDebugButtonEvents);
            }
        }
    }

    private void On_InputSource_ButtonReleased(EControllerID controllerID, EInputButton inputButton)
    {
        if (ConnectedControllers.ContainsKey(controllerID))
        {
            EPlayerID playerID = ConnectedControllers[controllerID];
            EClientID clientID = GetClientIDFrom(playerID);

            if (inputButton != EInputButton.NONE)
            {
                NetEvent_ButtonReleased buttonReleased = new NetEvent_ButtonReleased(clientID, inputButton);
                EventManager.Instance.INPUT_ButtonReleased.Invoke(buttonReleased, EEventInvocationType.TO_SERVER_ONLY, MotherOfManagers.Instance.CanDebugButtonEvents);
            }
        }
    }


    private void On_InputSource_JoystickMoved(EControllerID controllerID, EJoystickType joystickType, float x, float y)
    {
        if (ConnectedControllers.ContainsKey(controllerID))
        {
            EPlayerID playerID = ConnectedControllers[controllerID];
            EClientID clientID = GetClientIDFrom(playerID);

            if (joystickType != EJoystickType.NONE)
            {
                NetEvent_JoystickMoved joystickMoved = new NetEvent_JoystickMoved(clientID, joystickType, x, y);
                EventManager.Instance.INPUT_JoystickMoved.Invoke(joystickMoved, EEventInvocationType.TO_SERVER_ONLY, MotherOfManagers.Instance.CanDebugJoystickEvents);
            }
        }
    }
    #endregion

    #region Server Input
    private void On_NETWORK_ReceivedGameSessionInfo(Event_GenericHandle<List<EPlayerID>, EPlayerID> eventHandle)
    {
        List<EPlayerID> connectedPlayers = eventHandle.Arg1;
        EPlayerID ownPlayer = eventHandle.Arg2;

        // Connect controllers
        foreach (EPlayerID playerID in connectedPlayers)
        {
            if (playerID == ownPlayer)
            {
                ConnectControllerToPlayer(EControllerID.TOUCH, playerID);
            }
            else
            {
                ConnectControllerToPlayer(GetControllerNeteworkID(playerID), playerID);
            }
        }
    }
    #endregion

    #region Controller Connection
    /// <summary>
    /// Tries to connect the given controllerID (if not already connected) to the next free PlayerID available
    /// </summary>
    /// <param name="controllerID"> contrllerID </param>
    /// <returns></returns>
    public bool TryToConnectController(EControllerID controllerID)
    {
        // Check ControllerID
        if (controllerID == EControllerID.NONE)
        {
            Debug.LogError("Warning! Trying to connect a controller that is NONE.");
            return false;
        }

        // Get and check PlayerID
        EPlayerID playerID = PlayerManager.Instance.GetNextFreePlayerID();
        if (playerID == EPlayerID.NONE)
        {
            Debug.Log("Warning! Couldn't get a valid PlayerID for : " + controllerID);
            return false;
        }

        // Check parameters
        if ((ConnectedControllers.ContainsKey(controllerID) == true)
            || (ConnectedControllers.ContainsValue(playerID) == true))
        {
            if (MotherOfManagers.Instance.IsSpawnRemainingAIPlayersOnGameStart == false)
            {
                Debug.LogError("Warning! Trying to connect a controller that is already connected.");
            }
            return false;
        }

        // Successful
        ConnectedControllers.Add(controllerID, playerID);

        // Invoke event
        Event_GenericHandle<EControllerID, EPlayerID> controllerConnected = new Event_GenericHandle<EControllerID, EPlayerID>(controllerID, playerID);
        EventManager.Instance.INPUT_ControllerConnected.Invoke(controllerConnected);

        return true;
    }

    private void ConnectControllerToPlayer(EControllerID controllerID, EPlayerID playerID)
    {
        // Check parameters
        if ((ConnectedControllers.ContainsKey(controllerID) == true)
            || (ConnectedControllers.ContainsValue(playerID) == true))
        {
            if (MotherOfManagers.Instance.IsSpawnRemainingAIPlayersOnGameStart == false)
            {
                Debug.LogError("Trying to connect a controller that is already connected.");
            }
            return;
        }
        if ((playerID == EPlayerID.NONE)
            || (controllerID == EControllerID.NONE))
        {
            Debug.LogError("Trying to connect a controller or player that is NONE.");
            return;
        }

        ConnectedControllers.Add(controllerID, playerID);

        Debug.Log("Connecting new controller " + controllerID + " to player : " + playerID);

        // Invoke event
        Event_GenericHandle<EControllerID, EPlayerID> controllerConnected = new Event_GenericHandle<EControllerID, EPlayerID>(controllerID, playerID);
        EventManager.Instance.INPUT_ControllerConnected.Invoke(controllerConnected);
    }


    public void DisconnectController(EControllerID controllerID)
    {
        if (ConnectedControllers.ContainsKey(controllerID) == false)
        {
            Debug.LogError("Trying to disconnect a controller that is not connected.");
            return;
        }

        EPlayerID playerID = connectedControllers[controllerID];

        ConnectedControllers.Remove(controllerID);

        // Invoke event
        Event_GenericHandle<EControllerID, EPlayerID> controllerDisconnected = new Event_GenericHandle<EControllerID, EPlayerID>(controllerID, playerID);
        EventManager.Instance.INPUT_ControllerDisconnected.Invoke(controllerDisconnected);
    }

    public void ConnectAllRemainingAIPlayers()
    {
        LogConsole("Connecting remaining AI playeres");

        foreach (EControllerID aIControllerID in AI_CONTROLLERS)
        {
            if (IsControllerConnected(aIControllerID) == false)
            {
                ConnectControllerToPlayer(EControllerID.AI_1, EPlayerID.PLAYER_1);
                ConnectControllerToPlayer(EControllerID.AI_2, EPlayerID.PLAYER_2);
                ConnectControllerToPlayer(EControllerID.AI_3, EPlayerID.PLAYER_3);
                ConnectControllerToPlayer(EControllerID.AI_4, EPlayerID.PLAYER_4);
            }
        }
    }

    public bool IsControllerConnected(EControllerID controllerID)
    {
        return ConnectedControllers.ContainsKey(controllerID);
    }
    #endregion


    /// <summary>
    /// Gets the connected PlayerID to the given ControllerID
    /// </summary>
    /// <returns> NONE if given controllerID is not connected</returns>
    public EPlayerID GetConnectedPlayerID(EControllerID controllerID)
    {
        if (ConnectedControllers.ContainsKey(controllerID))
        {
            return ConnectedControllers[controllerID];
        }
        return EPlayerID.NONE;
    }


    /// <summary>
    /// Gets the connected ControllerID from the given PlayerID
    /// </summary>
    /// <returns> NONE if given playerID is not connected</returns>
    public EControllerID GetConnectedControllerID(EPlayerID playerID)
    {
        foreach(EControllerID controllerID in ConnectedControllers.Keys)
        {
            if (ConnectedControllers[controllerID] == playerID)
            {
                return controllerID;
            }
        }
        return EControllerID.NONE;
    }

    /// <summary>
    /// Returns (the first) Input Source of type "A" attached on the Input Manager.
    /// </summary>
    /// <typeparam name="A"> Specification from AbstractInputSource </typeparam>
    /// <returns> The first Inpupt Source found </returns>
    public A GetInputSource<A>() where A : AbstractInputSource
    {
        A result = null;

        foreach (AbstractInputSource inputSource in inputSources)
        {
            if (inputSource.GetType() == typeof(A))
            {
                result = (A)inputSource;
                break;
            }
        }

        if (result == null)
        {
            Debug.Log("Warning! No Input Source of the given type found attached on InputManager!");
        }

        return result;
    }

}
