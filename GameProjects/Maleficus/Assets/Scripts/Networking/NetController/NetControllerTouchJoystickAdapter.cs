﻿using UnityEngine;
using static Maleficus.Utils;
using static Maleficus.Consts;

public class NetControllerTouchJoystickAdapter : BNJMOBehaviour
{
    private Vector2 oldMovementInput = new Vector2(0.0f, 0.0f);
    private Vector2 oldRotationInput = new Vector2(0.0f, 0.0f);
    private bool canPerformDirectionalButton = true;

    protected override void InitializeObjecsInScene()
    {
        base.InitializeObjecsInScene();

        MaleficusJoystick[] maleficusJoystics = FindObjectsOfType<MaleficusJoystick>();
        foreach (MaleficusJoystick maleficusJoystick in maleficusJoystics)
        {
            maleficusJoystick.TouchJoystickPressed += On_MaleficusJoystick_TouchJoystickPressed;
            maleficusJoystick.TouchJoystickMoved += On_MaleficusJoystick_TouchJoystickMoved;
            maleficusJoystick.TouchJoystickReleased += On_MaleficusJoystick_TouchJoystickReleased;
        }
    }

    protected override void Update()
    {
        base.Update();

        UpdateDirectionalInput();
    }

    private void On_MaleficusJoystick_TouchJoystickPressed(ETouchJoystickType touchJoystickType)
    {
        EInputButton inputButton = GetInputButtonFrom(touchJoystickType);

        if (inputButton != EInputButton.NONE)
        {
            NetControllerClient.Instance.SendButtonPressed(inputButton);
        }
    }

    private void On_MaleficusJoystick_TouchJoystickMoved(ETouchJoystickType touchJoystickType, Vector2 joystickInput)
    {
        if (Mathf.Abs(joystickInput.x) + Mathf.Abs(joystickInput.y) > THRESHOLD_JOYSTICK_ACTIVATION)
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

                    NetControllerClient.Instance.SendJoystickMoved(EJoystickType.MOVEMENT, joystickInput.x, joystickInput.y);
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

                    NetControllerClient.Instance.SendJoystickMoved(EJoystickType.ROTATION, joystickInput.x, joystickInput.y);
                }
            }
        }
    }

    private void On_MaleficusJoystick_TouchJoystickReleased(ETouchJoystickType touchJoystickType)
    {
        // Reinitialize Movement and Rotation
        if (touchJoystickType == ETouchJoystickType.MOVE)
        {
            NetControllerClient.Instance.SendJoystickMoved(EJoystickType.MOVEMENT, 0.0f, 0.0f);
            oldMovementInput = Vector2.zero;
        }
        else
        {
            NetControllerClient.Instance.SendJoystickMoved(EJoystickType.ROTATION, 0.0f, 0.0f);
            oldRotationInput = Vector2.zero;
        }

        // Specific Button Released
        EInputButton inputButton = GetInputButtonFrom(touchJoystickType);
        if (inputButton != EInputButton.NONE)
        {
            NetControllerClient.Instance.SendButtonReleased(inputButton);
        }
    }




    private void UpdateDirectionalInput()
    {
        if (canPerformDirectionalButton == true)
        {
            // Joystick moved beyond threshold
            if (Vector2.Distance(oldMovementInput, Vector2.zero) > 0.5f)
            {
                // Horizontal move
                if (Mathf.Abs(oldMovementInput.x) > Mathf.Abs(oldMovementInput.y))
                {
                    // Right move
                    if (oldMovementInput.x > 0.0f)
                    {
                        NetControllerClient.Instance.SendButtonPressed(EInputButton.RIGHT);
                    }
                    // Left move
                    else
                    {
                        NetControllerClient.Instance.SendButtonPressed(EInputButton.LEFT);
                    }
                }
                // Vertical move
                else
                {
                    // Up move
                    if (oldMovementInput.y > 0.0f)
                    {
                        NetControllerClient.Instance.SendButtonPressed(EInputButton.UP);
                    }
                    // Down move
                    else
                    {
                        NetControllerClient.Instance.SendButtonPressed(EInputButton.DOWN);
                    }
                }
                canPerformDirectionalButton = false;
            }
        }
        else
        {
            // Joystick moved below threshold
            if (Vector2.Distance(oldMovementInput, Vector2.zero) < 0.5f)
            {
                canPerformDirectionalButton = true;
            }
        }
    }
}
