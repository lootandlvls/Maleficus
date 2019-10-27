using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Responsible for managing input controller sources attached to the same gameobject
/// </summary>
public class InputManager : AbstractSingletonManager<InputManager>
{
    public EInputMode                                   InputMode                               { get { return MotherOfManagers.Instance.InputMode; } }

    ///// <summary> Joysticks inputs for every player (movement, rotation). </summary>
    //public Dictionary<EControllerID, JoystickInput>     JoysticksInput                          { get { return JoystickInput; } }

    /// <summary> Mapping from controllerID to playerID </summary> 
    public Dictionary<EControllerID, EPlayerID>         ConnectedControllers                    { get { return connectedControllers; } }



    /// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    private Dictionary<EControllerID, bool>             canPerformHorizontalDirectionalButton   = new Dictionary<EControllerID, bool>();          
    /// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    private Dictionary<EControllerID, bool>             canPerformVerticalDirectionalButton     = new Dictionary<EControllerID, bool>();
    //private Dictionary<EControllerID, JoystickInput>    JoystickInput                           = new Dictionary<EControllerID, JoystickInput>();
    private Dictionary<EControllerID, EPlayerID>        connectedControllers                    = new Dictionary<EControllerID, EPlayerID>();

    protected override void Awake()
    {
        base.Awake();

        ReInitializeDirectionalMaps();

        AbstractInputSource[] inputSources = GetComponents<AbstractInputSource>();
        foreach (AbstractInputSource inputSource in inputSources)
        {
            inputSource.ButtonPressed += On_InputSource_ButtonPressed;
            inputSource.ButtonReleased += On_InputSource_ButtonReleased;
            inputSource.JoystickMoved += On_InputSource_JoystickMoved;
        }
    }


    private void Start()
    {

        EventManager.Instance.NETWORK_ReceivedGameSessionInfo.AddListener           (On_NETWORK_ReceivedGameSessionInfo);

        //// Listen to broadcasted inputs 
        //EventManager.Instance.INPUT_JoystickMoved.AddListener(On_INPUT_JoystickMoved);


        // Connect Touch player as first player
        if ((MotherOfManagers.Instance.InputMode == EInputMode.TOUCH) 
            && (MotherOfManagers.Instance.IsSpawnTouchAsPlayer1 == true) 
            && (ConnectedControllers.ContainsKey(EControllerID.TOUCH) == false))
        {
            ConnectControllerToPlayer(EControllerID.TOUCH, EPlayerID.PLAYER_1);
        }

    }

    

    public override void OnSceneStartReinitialize()
    {
        ReInitializeDirectionalMaps();
    }

    private void Update()
    {
        //FlushControllersInput();

        //CheckButtonsAndJoysticksInput();

    }

    private void LateUpdate()
    {
        //BroadcastLocalControllersInput();
    }


    private void On_InputSource_ButtonPressed(EControllerID controllerID, EInputButton inputButton)
    {
        if (ConnectedControllers.ContainsKey(controllerID))
        {
            EPlayerID playerID = PlayerManager.Instance.GetPlayerIDFrom(controllerID);
            EClientID clientID = MaleficusUtilities.GetClientIDFrom(playerID);

            if (inputButton != EInputButton.NONE)
            {
                NetEvent_ButtonPressed buttonPressed = new NetEvent_ButtonPressed(clientID, inputButton);
                EventManager.Instance.INPUT_ButtonPressed.Invoke(buttonPressed, EEventInvocationType.TO_SERVER_ONLY);
            }
        }
    }

    private void On_InputSource_ButtonReleased(EControllerID controllerID, EInputButton inputButton)
    {
        if (ConnectedControllers.ContainsKey(controllerID))
        {
            EPlayerID playerID = PlayerManager.Instance.GetPlayerIDFrom(controllerID);
            EClientID clientID = MaleficusUtilities.GetClientIDFrom(playerID);

            if (inputButton != EInputButton.NONE)
            {
                NetEvent_ButtonReleased buttonReleased = new NetEvent_ButtonReleased(clientID, inputButton);
                EventManager.Instance.INPUT_ButtonReleased.Invoke(buttonReleased, EEventInvocationType.TO_SERVER_ONLY);
            }
        }
    }


    private void On_InputSource_JoystickMoved(EControllerID controllerID, EJoystickType joystickType, float x, float y)
    {
        if (ConnectedControllers.ContainsKey(controllerID))
        {
            EPlayerID playerID = PlayerManager.Instance.GetPlayerIDFrom(controllerID);
            EClientID clientID = MaleficusUtilities.GetClientIDFrom(playerID);

            if (joystickType != EJoystickType.NONE)
            {
                NetEvent_JoystickMoved joystickMoved = new NetEvent_JoystickMoved(clientID, joystickType, x, y);
                EventManager.Instance.INPUT_JoystickMoved.Invoke(joystickMoved, EEventInvocationType.TO_SERVER_ONLY);
            }
        }
    }




    #region Network Input
    //private void On_INPUT_JoystickMoved(NetEvent_JoystickMoved eventHandle)
    //{
    //    EJoystickType joystickType = eventHandle.JoystickType;
    //    float joystick_X = eventHandle.Joystick_X;
    //    float joystick_Y = eventHandle.Joystick_Y;
    //    EPlayerID playerID = MaleficusUtilities.GetPlayerIDFrom(eventHandle.SenderID);
    //    EControllerID controllerID = PlayerManager.Instance.GetControllerFrom(playerID);

    //    if ((controllerID.ContainedIn(MaleficusConsts.NETWORK_CONTROLLERS))
    //    || (controllerID == EControllerID.TOUCH))
    //    {
    //        if (joystickType == EJoystickType.MOVEMENT)
    //        {
    //            JoysticksInput[controllerID].JoystickValues[EInputAxis.MOVE_X] = joystick_X;
    //            JoysticksInput[controllerID].JoystickValues[EInputAxis.MOVE_Y] = joystick_Y;
    //        }
    //        else if (joystickType == EJoystickType.ROTATION)
    //        {
    //            JoysticksInput[controllerID].JoystickValues[EInputAxis.ROTATE_X] = joystick_X;
    //            JoysticksInput[controllerID].JoystickValues[EInputAxis.ROTATE_Y] = joystick_Y;
    //        }
    //    }
    //}

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
                ConnectControllerToPlayer(MaleficusUtilities.GetControllerNeteworkID(playerID), playerID);
            }
        }
    }
    #endregion


    public void ConnectControllerToPlayer(EControllerID controllerID, EPlayerID playerID)
    {
        // Check parameters
        if (ConnectedControllers.ContainsKey(controllerID) == true)
        {
            Debug.LogError("Trying to connect a controller that is already connected.");
            return;
        }
        if ((playerID == EPlayerID.NONE)
            || (controllerID == EControllerID.NONE))
        {
            Debug.LogError("Trying to connect a controller or player that is NONE.");
            return;
        }

        ConnectedControllers.Add(controllerID, playerID);

        // Initialize dictionaries
        canPerformHorizontalDirectionalButton.Add(controllerID, true);
        canPerformVerticalDirectionalButton.Add(controllerID, true);

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

        canPerformHorizontalDirectionalButton.Remove(controllerID);
        canPerformVerticalDirectionalButton.Remove(controllerID);
    }

    //private void FlushControllersInput()
    //{
    //    foreach (EControllerID controllerID in ConnectedControllers.Keys)
    //    {
    //        if (controllerID.ContainedIn(MaleficusConsts.NETWORK_CONTROLLERS) == true)
    //        {
    //            continue;
    //        }
    //        JoysticksInput[controllerID].Flush();
    //    }
    //}

    private void ReInitializeDirectionalMaps()
    {
        foreach (EControllerID controllerID in ConnectedControllers.Keys)
        {
            canPerformHorizontalDirectionalButton[controllerID] = true;
        }

        foreach (EControllerID controllerID in ConnectedControllers.Keys)
        {
            canPerformVerticalDirectionalButton[controllerID] = true;
        }
    }


    public bool IsControllerConnected(EControllerID controllerID)
    {
        return ConnectedControllers.ContainsKey(controllerID);
    }
}
