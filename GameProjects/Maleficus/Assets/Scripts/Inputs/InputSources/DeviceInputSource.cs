using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Maleficus.MaleficusConsts;

public class DeviceInputSource : AbstractInputSource
{
    private Dictionary<EControllerID, PlayerInputListener> connectedControllers = new Dictionary<EControllerID, PlayerInputListener>();

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.APP_AppStateUpdated.Event += On_APP_AppStateUpdated;
    }

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

        foreach (EControllerID controllerID in connectedControllers.Keys)
        {
            PlayerInputListener playerInputListener = connectedControllers[controllerID];
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
        IS_VALID(playerInputListener);

        // Assign a ControllerID
        EControllerID controllerID = GetNextFreeControllerID(playerInputListener);

        if (controllerID != EControllerID.NONE)
        {
            // Connect controller on Input Manager
            if (InputManager.Instance.TryToConnectController(controllerID) == true)
            {
                connectedControllers.Add(controllerID, playerInputListener);

                // Bind Input events
                playerInputListener.ButtonPressed   += PlayerInputListener_OnButtonPressed;
                playerInputListener.ButtonReleased  += PlayerInputListener_OnButtonReleased;
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

    private EControllerID GetNextFreeControllerID(PlayerInputListener playerInputListener)
    {
        EControllerID controllerID = EControllerID.NONE;
        foreach (EControllerID controllerIDitr in GAMEPADS_CONTROLLERS)
        {
            if (connectedControllers.ContainsKey(controllerIDitr) == false)
            {
                controllerID = controllerIDitr;
                break;
            }
        }
        return controllerID;
    }
}
