using UnityEngine;
using System;
using static Maleficus.Utils;
using static Maleficus.Consts;

namespace NetController
{
    public class NCTouchJoystickAdapter : BNJMOBehaviour
    {
        public event Action<EJoystickType, float, float> JoystickMoved;
        public event Action<EInputButton> ButtonPressed;
        public event Action<EInputButton> ButtonReleased;

        private Vector2 oldMovementInput = new Vector2(0.0f, 0.0f);
        private Vector2 oldRotationInput = new Vector2(0.0f, 0.0f);
        private bool canPerformDirectionalButton = true;

        protected override void InitializeObjecsInScene()
        {
            base.InitializeObjecsInScene();

            // Joysticks
            MaleficusJoystick[] maleficusJoystics = FindObjectsOfType<MaleficusJoystick>();
            foreach (MaleficusJoystick maleficusJoystick in maleficusJoystics)
            {
                maleficusJoystick.TouchJoystickPressed += On_MaleficusJoystick_TouchJoystickPressed;
                maleficusJoystick.TouchJoystickMoved += On_MaleficusJoystick_TouchJoystickMoved;
                maleficusJoystick.TouchJoystickReleased += On_MaleficusJoystick_TouchJoystickReleased;
            }

            // Confirm Button
            ConfirmUIAction confirmUIAction = FindObjectOfType<ConfirmUIAction>();
            if (IS_NOT_NULL(confirmUIAction))
            {
                confirmUIAction.Button.ButtonPressed += On_ConfirmButton_ButtonPressed;
                confirmUIAction.Button.ButtonSuccessfullyReleased += On_ConfirmButton_ButtonSuccessfullyReleased; ;
            }

            // Cancel Button
            CancelUIAction cancelUIAction = FindObjectOfType<CancelUIAction>();
            if (IS_NOT_NULL(cancelUIAction))
            {
                cancelUIAction.Button.ButtonPressed += On_CancelButton_ButtonPressed;
                cancelUIAction.Button.ButtonSuccessfullyReleased += On_CanelButton_ButtonSuccessfullyReleased;
            }
        }


        protected override void Update()
        {
            base.Update();

            UpdateDirectionalInput();
        }


# region Events Callbacks
        private void On_MaleficusJoystick_TouchJoystickPressed(ETouchJoystickType touchJoystickType)
        {
            EInputButton inputButton = GetInputButtonFrom(touchJoystickType);

            if (inputButton != EInputButton.NONE)
            {
                InvokeEventIfBound(ButtonPressed, inputButton);
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

                    // Is new joystick input different enough from last registred one?
                    if (inputDistance > THRESHOLD_JOYSTICK_DISTANCE_MOVEMENT)
                    {
                        joystickInput.Normalize();
                        float x = joystickInput.x;
                        float y = joystickInput.y;
                        TransformAxisToCamera(ref x, ref y, Camera.main.transform.forward);                  // TODO: remove if not needed
                        joystickInput = new Vector2(x, y);

                        oldMovementInput = joystickInput;

                        InvokeEventIfBound(JoystickMoved, EJoystickType.MOVEMENT, joystickInput.x, joystickInput.y);
                    }
                }
                else // Spell (Rotation) joystick
                {
                    joystickInput.y = -joystickInput.y;
                    float inputDistance = Vector2.Distance(oldRotationInput.normalized, joystickInput.normalized);

                    // Is new joystick input different enough from last registred one?
                    if (inputDistance > THRESHOLD_JOYSTICK_DISTANCE_ROTATION)
                    {
                        joystickInput.Normalize();
                        float x = joystickInput.x;
                        float y = joystickInput.y;
                        TransformAxisToCamera(ref x, ref y, Camera.main.transform.forward, true);              // TODO: remove if not needed
                        joystickInput = new Vector2(x, y);

                        oldRotationInput = joystickInput;

                        InvokeEventIfBound(JoystickMoved, EJoystickType.ROTATION, joystickInput.x, joystickInput.y);
                    }
                }
            }
        }

        private void On_MaleficusJoystick_TouchJoystickReleased(ETouchJoystickType touchJoystickType)
        {
            // Reinitialize Movement and Rotation
            if (touchJoystickType == ETouchJoystickType.MOVE)
            {
                InvokeEventIfBound(JoystickMoved, EJoystickType.MOVEMENT, 0.0f, 0.0f);
                oldMovementInput = Vector2.zero;
            }
            else
            {
                InvokeEventIfBound(JoystickMoved, EJoystickType.ROTATION, 0.0f, 0.0f);
                oldRotationInput = Vector2.zero;
            }

            // Specific Button Released
            EInputButton inputButton = GetInputButtonFrom(touchJoystickType);
            if (inputButton != EInputButton.NONE)
            {
                InvokeEventIfBound(ButtonReleased, inputButton);
            }
        }

        private void On_ConfirmButton_ButtonPressed(BNJMOButton button)
        {
            InvokeEventIfBound(ButtonPressed, EInputButton.CONFIRM);
        }

        private void On_ConfirmButton_ButtonSuccessfullyReleased(BNJMOButton button)
        {
            InvokeEventIfBound(ButtonReleased, EInputButton.CONFIRM);

        }

        private void On_CancelButton_ButtonPressed(BNJMOButton button)
        {
            InvokeEventIfBound(ButtonPressed, EInputButton.CANCEL);

        }

        private void On_CanelButton_ButtonSuccessfullyReleased(BNJMOButton button)
        {
            InvokeEventIfBound(ButtonReleased, EInputButton.CANCEL);

        }
        #endregion

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
                            InvokeEventIfBound(ButtonReleased, EInputButton.RIGHT);
                        }
                        // Left move
                        else
                        {
                            InvokeEventIfBound(ButtonReleased, EInputButton.LEFT);
                        }
                    }
                    // Vertical move
                    else
                    {
                        // Up move
                        if (oldMovementInput.y > 0.0f)
                        {
                            InvokeEventIfBound(ButtonReleased, EInputButton.UP);
                        }
                        // Down move
                        else
                        {
                            InvokeEventIfBound(ButtonReleased, EInputButton.DOWN);
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
}