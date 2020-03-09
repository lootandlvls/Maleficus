using System.Collections.Generic;
using UnityEngine;
using static Maleficus.Consts;
using static Maleficus.Utils;

/// <summary>
/// Responsible for managing input controller sources attached to the same gameobject
/// </summary>
public class InputManager : AbstractSingletonManager<InputManager>
{
    public EInputMode           InputMode                       { get { return MotherOfManagers.Instance.InputMode; } }

    /// <summary> Mapping from controllerID to playerID </summary> 
    private List<EControllerID>  connectedControllers         { get; } = new List<EControllerID>();

    AbstractInputSource[] inputSources;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

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

        //EventManager.Instance.NETWORK_ReceivedGameSessionInfo.Event += On_NETWORK_ReceivedGameSessionInfo;
    }

    protected override void LateStart()
    {
        base.LateStart();

        ConnectAllAIControllers();

        // Connect Touch player as first player
        //if ((MotherOfManagers.Instance.InputMode == EInputMode.TOUCH)
        //    && (MotherOfManagers.Instance.IsSpawnTouchAsPlayer1 == true))
        //{
        //    ConnectControllerToPlayer(EControllerID.TOUCH, EPlayerID.PLAYER_1);
        //}
    }

    protected override void Update()
    {
        base.Update();

        UpdateControllersDebugText();
    }

    #region Input Source Callbacks
    private void On_InputSource_ButtonPressed(EControllerID controllerID, EInputButton inputButton)
    {
        if (connectedControllers.Contains(controllerID))
        {
            EPlayerID playerID = PlayerManager.Instance.GetAssignedPlayerID(controllerID);
            EClientID clientID = GetClientIDFrom(playerID);

            if (inputButton != EInputButton.NONE)
            {
                NetEvent_ButtonPressed buttonPressed = new NetEvent_ButtonPressed(clientID, controllerID, inputButton);
                EventManager.Instance.INPUT_ButtonPressed.Invoke(buttonPressed, EEventInvocationType.TO_SERVER_ONLY, MotherOfManagers.Instance.CanDebugButtonEvents);
            }
        }
    }

    private void On_InputSource_ButtonReleased(EControllerID controllerID, EInputButton inputButton)
    {
        if (connectedControllers.Contains(controllerID))
        {
            EPlayerID playerID = PlayerManager.Instance.GetAssignedPlayerID(controllerID);
            EClientID clientID = GetClientIDFrom(playerID);

            if (inputButton != EInputButton.NONE)
            {
                NetEvent_ButtonReleased buttonReleased = new NetEvent_ButtonReleased(clientID, controllerID, inputButton);
                EventManager.Instance.INPUT_ButtonReleased.Invoke(buttonReleased, EEventInvocationType.TO_SERVER_ONLY, MotherOfManagers.Instance.CanDebugButtonEvents);
            }
        }
    }

    private void On_InputSource_JoystickMoved(EControllerID controllerID, EJoystickType joystickType, float x, float y)
    {
        if (connectedControllers.Contains(controllerID))
        {
            EPlayerID playerID = PlayerManager.Instance.GetAssignedPlayerID(controllerID);
            EClientID clientID = GetClientIDFrom(playerID);

            if (joystickType != EJoystickType.NONE)
            {
                NetEvent_JoystickMoved joystickMoved = new NetEvent_JoystickMoved(clientID, controllerID, joystickType, x, y);
                EventManager.Instance.INPUT_JoystickMoved.Invoke(joystickMoved, EEventInvocationType.TO_SERVER_ONLY, MotherOfManagers.Instance.CanDebugJoystickEvents);
            }
        }
    }
    #endregion

    #region Server Input
    //private void On_NETWORK_ReceivedGameSessionInfo(Event_GenericHandle<List<EPlayerID>, EPlayerID> eventHandle)
    //{
    //    List<EPlayerID> connectedPlayers = eventHandle.Arg1;
    //    EPlayerID ownPlayer = eventHandle.Arg2;

    //    // Connect controllers
    //    foreach (EPlayerID playerID in connectedPlayers)
    //    {
    //        if (playerID == ownPlayer)
    //        {
    //            ConnectControllerToPlayer(EControllerID.TOUCH, playerID);
    //        }
    //        else
    //        {
    //            ConnectControllerToPlayer(GetControllerNeteworkID(playerID), playerID);
    //        }
    //    }
    //}

    #endregion

    #region Controller Connection
    public bool ConnectController(EControllerID controllerID)
    {
        if (controllerID == EControllerID.NONE)
        {
            Debug.LogError("Trying to connect a controller that is NONE.");
            return false;
        }
        if (connectedControllers.Contains(controllerID))
        {
            Debug.LogError("Trying to connect a controller that is already connected.");
            return false;
        }

        connectedControllers.Add(controllerID);

        Debug.Log("Connecting new controller " + controllerID);

        // Invoke event
        Event_GenericHandle<EControllerID> controllerConnected = new Event_GenericHandle<EControllerID>(controllerID);
        EventManager.Instance.INPUT_ControllerConnected.Invoke(controllerConnected);

        return true;
    }

    public void DisconnectController(EControllerID controllerID)
    {
        if (connectedControllers.Contains(controllerID) == false)
        {
            Debug.LogError("Trying to disconnect a controller that is not connected.");
            return;
        }

        connectedControllers.Remove(controllerID);

        // Invoke event
        Event_GenericHandle<EControllerID> controllerDisconnected = new Event_GenericHandle<EControllerID>(controllerID);
        EventManager.Instance.INPUT_ControllerDisconnected.Invoke(controllerDisconnected);
    }

    private void ConnectAllAIControllers()
    {
        foreach (EControllerID aIControllerID in AI_CONTROLLERS)
        {
            ConnectController(aIControllerID);
        }
    }

    public bool IsControllerConnected(EControllerID controllerID)
    {
        return connectedControllers.Contains(controllerID);
    }
    #endregion

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

    private void UpdateControllersDebugText()
    {
        string playerStatusLog = "Connected controllers : \n";
        foreach (EControllerID controllerID in connectedControllers)
        {
            playerStatusLog += controllerID + "\n";
        }
        LogCanvas(13, playerStatusLog);
    }
}
