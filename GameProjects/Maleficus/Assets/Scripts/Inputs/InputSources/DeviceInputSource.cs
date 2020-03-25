using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Maleficus.Consts;

public class DeviceInputSource : AbstractInputSource
{
    private Dictionary<EControllerID, PlayerInputListener> connectedDeviceControllers = new Dictionary<EControllerID, PlayerInputListener>();

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.APP_AppStateUpdated.Event += On_APP_AppStateUpdated;
    }


    // TODO: disconnect player
    private void On_APP_AppStateUpdated(Event_StateUpdated<EAppState> eventHandle)
    {
        //switch(eventHandle.NewState)
        //{
        //    case EAppState.IN_MENU_IN_CONNECTING_CONTROLLERS:
        //        PlayerInputManager.instance.EnableJoining();
        //        break;

        //    default:
        //        PlayerInputManager.instance.DisableJoining();
        //        break;
        //}
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        foreach (EControllerID controllerID in connectedDeviceControllers.Keys)
        {
            PlayerInputListener playerInputListener = connectedDeviceControllers[controllerID];
            PlayerInputListener_OnJoystickMoved(controllerID, EJoystickType.MOVEMENT, playerInputListener.MoveAxis);
            PlayerInputListener_OnJoystickMoved(controllerID, EJoystickType.ROTATION, playerInputListener.RotateAxis);
        }
    }

    /// <summary>
    /// Called from a PlayerInputListener when spawned to connect to the next available ControllerID
    /// </summary>
    /// <param name="playerInput"> New player input listener </param>
    /// <returns> Assigned ControllerID </returns>
    public EControllerID OnNewDeviceJoined(PlayerInputListener playerInputListener)
    {
        IS_NOT_NULL(playerInputListener);

        if (MotherOfManagers.Instance.InputMode == EInputMode.CONTROLLER)
        {
            // Assign a ControllerID
            EControllerID controllerID = GetNextFreeDeviceControllerID();

            if (controllerID != EControllerID.NONE)
            {
                // Connect controller on Input Manager
                if (InputManager.Instance.ConnectController(controllerID) == true)
                {
                    connectedDeviceControllers.Add(controllerID, playerInputListener);

                    // Bind Input events
                    playerInputListener.ButtonPressed += PlayerInputListener_OnButtonPressed;
                    playerInputListener.ButtonReleased += PlayerInputListener_OnButtonReleased;
                }
                else
                {
                    return EControllerID.NONE;
                }
            }
            else
            {
                LogConsoleWarning("No free Controller ID found for new connected device : " + playerInputListener.DeviceName);
            }

            return controllerID;
        }
        return EControllerID.NONE;
    }

    // TODO
    public void OnDeviceHasLeft(EControllerID controllerID, PlayerInputListener playerInputListener)
    {
        //IS_VALID(playerInputListener);

        //if ((connectedControllers.ContainsKey(controllerID)
        //    && (connectedControllers[controllerID] == playerInputListener))
        //{

        //}
        //else
        //{
        //    LogConsoleWarning("An invalid device has left : " + playerInputListener.DeviceName);
        //}
    }


    private void PlayerInputListener_OnButtonPressed(EControllerID controllerID, EInputButton inputButton)
    {
        InvokeButtonPressed(controllerID, inputButton);
    }

    private void PlayerInputListener_OnButtonReleased(EControllerID controllerID, EInputButton inputButton)
    {
        InvokeButtonReleased(controllerID, inputButton);
    }

    private void PlayerInputListener_OnJoystickMoved(EControllerID controllerID, EJoystickType joystickType, Vector2 axisValues)
    {
        InvokeJoystickMoved(controllerID, joystickType, axisValues.x, axisValues.y);
    }

    private EControllerID GetNextFreeDeviceControllerID()
    {
        EControllerID controllerID = EControllerID.NONE;
        foreach (EControllerID controllerIDitr in DEVICE_CONTROLLERS)
        {
            if (connectedDeviceControllers.ContainsKey(controllerIDitr) == false)
            {
                controllerID = controllerIDitr;
                break;
            }
        }
        return controllerID;
    }
}
