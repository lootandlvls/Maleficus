using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchJoystickInputSource : AbstractInputSource
{
    private Vector2 oldInput = new Vector2(0.0f, 0.0f);

    private void Awake()
    {
        MaleficusJoystick[] maleficusJoystics = FindObjectsOfType<MaleficusJoystick>();
        foreach(MaleficusJoystick maleficusJoystick in maleficusJoystics)
        {
            Debug.Log("adding a touch joystick : " + maleficusJoystick.JoystickType);

            maleficusJoystick.TouchJoystickPressed  += On_MaleficusJoystick_TouchJoystickPressed;
            maleficusJoystick.TouchJoystickMoved    += On_MaleficusJoystick_TouchJoystickMoved;
            maleficusJoystick.TouchJoystickReleased += On_MaleficusJoystick_TouchJoystickReleased;
        }
    }

    private void On_MaleficusJoystick_TouchJoystickPressed(ETouchJoystickType touchJoystickType)
    {
        if (InputManager.Instance.IsControllerConnected(EControllerID.TOUCH))
        {
            EInputButton inputButton = MaleficusUtilities.GetInputButtonFrom(touchJoystickType);
            InvokeButtonPressed(EControllerID.TOUCH, inputButton);
        }
    }

    private void On_MaleficusJoystick_TouchJoystickMoved(ETouchJoystickType touchJoystickType, Vector2 joystickInput)
    {
        Vector2 newInput = joystickInput;

        if ((InputManager.Instance.IsControllerConnected(EControllerID.TOUCH))
            && (Mathf.Abs(newInput.x) + Mathf.Abs(newInput.y) > MaleficusConsts.THRESHOLD_JOYSTICK_ACTIVATION))
        {
            // Move joystick
            if (touchJoystickType == ETouchJoystickType.MOVE)
            {
                float inputDistance = Vector2.Distance(oldInput.normalized, newInput.normalized);

                if (inputDistance > MaleficusConsts.THRESHOLD_JOYSTICK_DISTANCE_MOVEMENT)
                // Is new joystick input different enough from last registred one?
                {
                    newInput.Normalize();
                    float x = newInput.x;
                    float y = newInput.y;
                    MaleficusUtilities.TransformAxisToCamera(ref x, ref y, Camera.main.transform.forward);                  // TODO: remove if not needed
                    newInput = new Vector2(x, y);

                    InvokeJoystickMoved(EControllerID.TOUCH, EJoystickType.MOVEMENT, newInput.x, newInput.y);
                }
            }
            else // Spell joystick
            {
                newInput.y = -newInput.y;
                float inputDistance = Vector2.Distance(oldInput.normalized, newInput.normalized);

                if (inputDistance > MaleficusConsts.THRESHOLD_JOYSTICK_DISTANCE_ROTATION)
                // Is new joystick input different enough from last registred one?
                {
                    newInput.Normalize();
                    float x = newInput.x;
                    float y = newInput.y;
                    MaleficusUtilities.TransformAxisToCamera(ref x, ref y, Camera.main.transform.forward, true);              // TODO: remove if not needed
                    newInput = new Vector2(x, y);

                    InvokeJoystickMoved(EControllerID.TOUCH, EJoystickType.ROTATION, newInput.x, newInput.y);
                }
            }

        }
    }

    private void On_MaleficusJoystick_TouchJoystickReleased(ETouchJoystickType touchJoystickType)
    {
        if (InputManager.Instance.IsControllerConnected(EControllerID.TOUCH))
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
                InvokeButtonReleased(EControllerID.TOUCH, inputButton);
            }
        }
    }


    
}
