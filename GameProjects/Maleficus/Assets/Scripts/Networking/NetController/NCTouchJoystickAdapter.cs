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

            // Buttons
            ConfirmUIAction confirmUIAction = FindObjectOfType<ConfirmUIAction>();
            if (IS_NOT_NULL(confirmUIAction))
            {
                confirmUIAction.ActionButtonExecuted += On_ConfirmUIAction_ActionButtonPressed;
            }
            CancelUIAction cancelUIAction = FindObjectOfType<CancelUIAction>();
            if (IS_NOT_NULL(cancelUIAction))
            {
                cancelUIAction.ActionButtonExecuted += On_CancelUIAction_ActionButtonPressed; ;
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
                //NCClient.Instance.SendButtonPressed(inputButton);
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

                        InvokeEventIfBound(JoystickMoved, EJoystickType.MOVEMENT, joystickInput.x, joystickInput.y);
                        //NCClient.Instance.SendJoystickMoved(EJoystickType.MOVEMENT, joystickInput.x, joystickInput.y);
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

                        InvokeEventIfBound(JoystickMoved, EJoystickType.ROTATION, joystickInput.x, joystickInput.y);
                        //NCClient.Instance.SendJoystickMoved(EJoystickType.ROTATION, joystickInput.x, joystickInput.y);
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
                //NCClient.Instance.SendJoystickMoved(EJoystickType.MOVEMENT, 0.0f, 0.0f);
                oldMovementInput = Vector2.zero;
            }
            else
            {
                InvokeEventIfBound(JoystickMoved, EJoystickType.ROTATION, 0.0f, 0.0f);
                //NCClient.Instance.SendJoystickMoved(EJoystickType.ROTATION, 0.0f, 0.0f);
                oldRotationInput = Vector2.zero;
            }

            // Specific Button Released
            EInputButton inputButton = GetInputButtonFrom(touchJoystickType);
            if (inputButton != EInputButton.NONE)
            {
                InvokeEventIfBound(ButtonReleased, inputButton);
                //NCClient.Instance.SendButtonReleased(inputButton);
            }
        }

        private void On_ConfirmUIAction_ActionButtonPressed()
        {
            //if (client.isConnected)
            //{
            //    StringMessage message = new StringMessage();
            //    message.value = controllerGuid + "|" + ((int)EInputButton.CONFIRM).ToString();
            //    client.Send(NET_CONTROLLER_MESSAGE_BUTTON_PRESSED, message);
            //}

            InvokeEventIfBound(ButtonReleased, EInputButton.CONFIRM);
        }

        private void On_CancelUIAction_ActionButtonPressed()
        {
            //if (client.isConnected)
            //{
            //    StringMessage message = new StringMessage();
            //    message.value = controllerGuid + "|" + ((int)EInputButton.CANCEL).ToString();
            //    client.Send(NET_CONTROLLER_MESSAGE_BUTTON_PRESSED, message);
            //}
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
                            //NCClient.Instance.SendButtonPressed(EInputButton.RIGHT);
                        }
                        // Left move
                        else
                        {
                            InvokeEventIfBound(ButtonReleased, EInputButton.LEFT);
                            //NCClient.Instance.SendButtonPressed(EInputButton.LEFT);
                        }
                    }
                    // Vertical move
                    else
                    {
                        // Up move
                        if (oldMovementInput.y > 0.0f)
                        {
                            InvokeEventIfBound(ButtonReleased, EInputButton.UP);
                            //NCClient.Instance.SendButtonPressed(EInputButton.UP);
                        }
                        // Down move
                        else
                        {
                            InvokeEventIfBound(ButtonReleased, EInputButton.DOWN);
                            //NCClient.Instance.SendButtonPressed(EInputButton.DOWN);
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