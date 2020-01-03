using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.MaleficusUtilities;
using static Maleficus.MaleficusConsts;

public class TouchJoystickInputSource : AbstractInputSource
{
    private Vector2 oldMovementInput = new Vector2(0.0f, 0.0f);
    private Vector2 oldRotationInput = new Vector2(0.0f, 0.0f);

    protected override void InitializeObjecsInScene()
    {
        base.InitializeObjecsInScene();

        MaleficusJoystick[] maleficusJoystics = FindObjectsOfType<MaleficusJoystick>();
        foreach(MaleficusJoystick maleficusJoystick in maleficusJoystics)
        {
            maleficusJoystick.TouchJoystickPressed  += On_MaleficusJoystick_TouchJoystickPressed;
            maleficusJoystick.TouchJoystickMoved    += On_MaleficusJoystick_TouchJoystickMoved;
            maleficusJoystick.TouchJoystickReleased += On_MaleficusJoystick_TouchJoystickReleased;
        }
    }

    private void On_MaleficusJoystick_TouchJoystickPressed(ETouchJoystickType touchJoystickType)
    {
        if (InputManager.Instance.IsControllerConnected(EControllerID.TOUCH))
        {
            EInputButton inputButton = GetInputButtonFrom(touchJoystickType);
            InvokeButtonPressed(EControllerID.TOUCH, inputButton);
        }
    }

    private void On_MaleficusJoystick_TouchJoystickMoved(ETouchJoystickType touchJoystickType, Vector2 joystickInput)
    {
        if ((InputManager.Instance.IsControllerConnected(EControllerID.TOUCH))
            && (Mathf.Abs(joystickInput.x) + Mathf.Abs(joystickInput.y) > THRESHOLD_JOYSTICK_ACTIVATION))
        {
            // Move joystick
            if (touchJoystickType == ETouchJoystickType.MOVE)
            {
                float inputDistance = Vector2.Distance(oldMovementInput.normalized, joystickInput.normalized);

                if (inputDistance > THRESHOLD_JOYSTICK_DISTANCE_MOVEMENT)
                // Is new joystick input different enough from last registred one?
                {
                    joystickInput.Normalize();
                    float x = joystickInput.x;
                    float y = joystickInput.y;
                    TransformAxisToCamera(ref x, ref y, Camera.main.transform.forward);                  // TODO: remove if not needed
                    joystickInput = new Vector2(x, y);

                    oldMovementInput = joystickInput;

                    InvokeJoystickMoved(EControllerID.TOUCH, EJoystickType.MOVEMENT, joystickInput.x, joystickInput.y);
                }
            }
            else // Spell (Rotation) joystick
            {
                joystickInput.y = -joystickInput.y;
                float inputDistance = Vector2.Distance(oldRotationInput.normalized, joystickInput.normalized);

                if (inputDistance > THRESHOLD_JOYSTICK_DISTANCE_ROTATION)
                // Is new joystick input different enough from last registred one?
                {
                    joystickInput.Normalize();
                    float x = joystickInput.x;
                    float y = joystickInput.y;
                    TransformAxisToCamera(ref x, ref y, Camera.main.transform.forward, true);              // TODO: remove if not needed
                    joystickInput = new Vector2(x, y);

                    oldRotationInput = joystickInput;

                    InvokeJoystickMoved(EControllerID.TOUCH, EJoystickType.ROTATION, joystickInput.x, joystickInput.y);
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
            EInputButton inputButton = GetInputButtonFrom(touchJoystickType);
            if (inputButton != EInputButton.NONE)
            {
                InvokeButtonReleased(EControllerID.TOUCH, inputButton);
            }
        }
    }


    
}
