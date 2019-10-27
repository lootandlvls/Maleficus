using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchJoystickInputSource : AbstractInputSource
{

    private void Awake()
    {
        MaleficusJoystick[] maleficusJoystics = FindObjectsOfType<MaleficusJoystick>();
        foreach(MaleficusJoystick maleficusJoystick in maleficusJoystics)
        {
            maleficusJoystick.TouchJoystickPressed += On_MaleficusJoystick_TouchJoystickPressed;
            maleficusJoystick.TouchJoystickMoved += On_MaleficusJoystick_TouchJoystickMoved;
            maleficusJoystick.TouchJoystickReleased += On_MaleficusJoystick_TouchJoystickReleased;
        }
    }

    private void On_MaleficusJoystick_TouchJoystickPressed(ETouchJoystickType touchJoystickType)
    {
        if (InputManager.Instance.InputMode == EInputMode.TOUCH)
        {
            EInputButton inputButton = MaleficusUtilities.GetInputButtonFrom(touchJoystickType);
            InvokeButtonPressed(EControllerID.TOUCH, inputButton);
        }
    }

    private void On_MaleficusJoystick_TouchJoystickMoved(ETouchJoystickType touchJoystickType, Vector2 joystickInput)
    {
        Vector2 newInput = joystickInput;

        if ((InputManager.Instance.InputMode == EInputMode.TOUCH)
            && (ControllersInput.ContainsKey(EControllerID.TOUCH))
            && (Mathf.Abs(newInput.x) + Mathf.Abs(newInput.y) > MaleficusConsts.THRESHOLD_JOYSTICK_ACTIVATION))
        {
            EPlayerID playerID = PlayerManager.Instance.GetPlayerIDFrom(EControllerID.TOUCH);
            EClientID clientID = MaleficusUtilities.GetClientIDFrom(playerID);

            //EClientID clientID = NetworkManager.Instance.OwnClientID;

            // Move joystick
            if (touchJoystickType == ETouchJoystickType.MOVE)
            {
                EJoystickType joysticksType = EJoystickType.MOVEMENT;
                var joystickValues = ControllersInput[EControllerID.TOUCH].JoystickValues;
                Vector2 oldInput = new Vector2(joystickValues[EInputAxis.MOVE_X], joystickValues[EInputAxis.MOVE_Y]);
                float inputDistance = Vector2.Distance(oldInput.normalized, newInput.normalized);

                if (inputDistance > MaleficusConsts.THRESHOLD_JOYSTICK_DISTANCE_MOVEMENT)
                // Is new joystick input different enough from last registred one?
                {
                    newInput.Normalize();
                    float x = newInput.x;
                    float y = newInput.y;
                    MaleficusUtilities.TransformAxisToCamera(ref x, ref y, Camera.main.transform.forward);
                    newInput = new Vector2(x, y);

                    // TODO:  check if correct state
                    NetEvent_JoystickMoved joystickMoved = new NetEvent_JoystickMoved(clientID, joysticksType, newInput.x, newInput.y);
                    EventManager.Instance.INPUT_JoystickMoved.Invoke(joystickMoved, EEventInvocationType.TO_SERVER_ONLY);
                }
            }
            else // Spell joystick
            {
                newInput.y = -newInput.y;

                EJoystickType joysticksType = EJoystickType.ROTATION;
                var joystickValues = ControllersInput[EControllerID.TOUCH].JoystickValues;
                Vector2 oldInput = new Vector2(joystickValues[EInputAxis.ROTATE_X], joystickValues[EInputAxis.ROTATE_Y]);
                float inputDistance = Vector2.Distance(oldInput.normalized, newInput.normalized);

                if (inputDistance > MaleficusConsts.THRESHOLD_JOYSTICK_DISTANCE_ROTATION)
                // Is new joystick input different enough from last registred one?
                {
                    newInput.Normalize();
                    float x = newInput.x;
                    float y = newInput.y;
                    MaleficusUtilities.TransformAxisToCamera(ref x, ref y, Camera.main.transform.forward, true);
                    newInput = new Vector2(x, y);

                    // TODO:  check if correct state
                    NetEvent_JoystickMoved joystickMoved = new NetEvent_JoystickMoved(clientID, joysticksType, newInput.x, newInput.y);
                    EventManager.Instance.INPUT_JoystickMoved.Invoke(joystickMoved, EEventInvocationType.TO_SERVER_ONLY);
                }
            }

        }
    }

    private void On_MaleficusJoystick_TouchJoystickReleased(ETouchJoystickType touchJoystickType)
    {
        if (InputManager.Instance.InputMode == EInputMode.TOUCH)
        {
            // Reinitialize Movement and Rotation
            if (touchJoystickType == ETouchJoystickType.MOVE)
            {
                InvokeJoystickMoved(EControllerID.TOUCH, EJoystickType.MOVEMENT, 0.0f, 0.0f);

            }
            else
            {
                InvokeJoystickMoved(EControllerID.TOUCH, EJoystickType.ROTATION, 0.0f, 0.0f);
            }

            // Specific Button Released
            EInputButton inputButton = MaleficusUtilities.GetInputButtonFrom(touchJoystickType);
            if (inputButton != EInputButton.NONE)
            {
                InvokeButtonPressed(EControllerID.TOUCH, inputButton);
            }
        }
    }


    
}
