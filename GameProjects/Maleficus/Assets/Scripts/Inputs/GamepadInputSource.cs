using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadInputSource : AbstractInputSource
{

    private void CheckButtonsAndJoysticksInput()
    {
        // Confirm
        Check_Confirm(EControllerID.GAMEPAD_A);
        Check_Confirm(EControllerID.GAMEPAD_B);
        Check_Confirm(EControllerID.GAMEPAD_C);
        Check_Confirm(EControllerID.GAMEPAD_D);

        // TODO: Add missing Cancel buttons in Input Setting
        // Cancel
        //Check_Cancel(EpellIDllerID.CONTROLLER_A);
        //Check_Cancel(EControllerID.CONTROLLER_B);
        //Check_Cancel(EControllerID.CONTROLLER_C);
        //Check_Cancel(EControllerID.CONTROLLER_D);

        /* Charging spell check */
        // Spell 1
        Check_ChargingSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_A);
        Check_ChargingSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_B);
        Check_ChargingSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_C);
        Check_ChargingSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_D);
        //spell 2
        Check_ChargingSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_A);
        Check_ChargingSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_B);
        Check_ChargingSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_C);
        Check_ChargingSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_D);
        // spell 3
        Check_ChargingSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_A);
        Check_ChargingSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_B);
        Check_ChargingSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_C);
        Check_ChargingSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_D);

        /* Casted spell check */
        // Spell 1
        Check_CastedSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_A);
        Check_CastedSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_B);
        Check_CastedSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_C);
        Check_CastedSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_D);
        //spell 2
        Check_CastedSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_A);
        Check_CastedSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_B);
        Check_CastedSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_C);
        Check_CastedSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_D);
        // spell 3
        Check_CastedSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_A);
        Check_CastedSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_B);
        Check_CastedSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_C);
        Check_CastedSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_D);

        // Horizontal 
        // Left 
        Check_Axis("Horizontal", 'L', EControllerID.GAMEPAD_A);
        Check_Axis("Horizontal", 'L', EControllerID.GAMEPAD_B);
        Check_Axis("Horizontal", 'L', EControllerID.GAMEPAD_C);
        Check_Axis("Horizontal", 'L', EControllerID.GAMEPAD_D);

        // Right
        Check_Axis("Horizontal", 'R', EControllerID.GAMEPAD_A);
        Check_Axis("Horizontal", 'R', EControllerID.GAMEPAD_B);
        Check_Axis("Horizontal", 'R', EControllerID.GAMEPAD_C);
        Check_Axis("Horizontal", 'R', EControllerID.GAMEPAD_D);

        // Vertical 
        // Left 
        Check_Axis("Vertical", 'L', EControllerID.GAMEPAD_A);
        Check_Axis("Vertical", 'L', EControllerID.GAMEPAD_B);
        Check_Axis("Vertical", 'L', EControllerID.GAMEPAD_C);
        Check_Axis("Vertical", 'L', EControllerID.GAMEPAD_D);

        // Right
        Check_Axis("Vertical", 'R', EControllerID.GAMEPAD_A);
        Check_Axis("Vertical", 'R', EControllerID.GAMEPAD_B);
        Check_Axis("Vertical", 'R', EControllerID.GAMEPAD_C);
        Check_Axis("Vertical", 'R', EControllerID.GAMEPAD_D);
    }

    private void Check_Confirm(EControllerID controllerID)
    {
        char controllerIDName = MaleficusUtilities.ControllerIDToChar(controllerID);
        if (Input.GetButtonDown("Confirm_" + controllerIDName))
        {
            if ((InputManager.Instance.IsControllerConnected(controllerID) == true) || (InputManager.Instance.InputMode == EInputMode.TEST))
            {
                InvokeButtonPressed(controllerID, EInputButton.CONFIRM);
            }
            else if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_CONNECTING_GAMEPADS)
                 // Connect players
            {
                EPlayerID playerID = PlayerManager.Instance.GetNextFreePlayerID();
                if ((playerID != EPlayerID.TEST)
                    && (playerID != EPlayerID.NONE))
                {
                    InputManager.Instance.ConnectControllerToPlayer(controllerID, playerID);
                }
            }
        }
    }

    private void Check_Cancel(EControllerID controllerID)
    {
        char controllerIDName = MaleficusUtilities.ControllerIDToChar(controllerID);

        // TODO: Test with controller if it works
        if (Input.GetButtonDown("Cancel_" + controllerIDName))
        {
            if ((InputManager.Instance.IsControllerConnected(controllerID) == true) || (InputManager.Instance.InputMode == EInputMode.TEST))
            {
                //EPlayerID playerID = GetPlayerIDTo(controllerID);

                if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_CONNECTING_GAMEPADS)
                // Disconnect Player
                {
                    PlayerManager.Instance.DisconnectPlayerWithController(controllerID);
                    //playerControllerMapping.Remove(controllerID);
                    InputManager.Instance.DisconnectController(controllerID);
                }
                else // Not in connecting players state
                {
                    //EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.CANCEL, playerID);
                    //controllersInput[controllerID].IsButtonPressed[EInputButton.CANCEL] = true;
                }
            }
        }
    }


    private void Check_ChargingSpell(ESpellSlot spellID, EControllerID controllerID)
    {
        char controllerIDName = MaleficusUtilities.ControllerIDToChar(controllerID);
        EInputButton inputButton = MaleficusUtilities.GetInputButtonFrom(spellID);
        if (inputButton == EInputButton.NONE)
        {
            return;
        }

        if ((InputManager.Instance.IsControllerConnected(controllerID) == true) || (InputManager.Instance.InputMode == EInputMode.TEST))
        {
            // Did player press button?
            if (Input.GetButtonDown("CastSpell_" + spellID + '_' + controllerIDName))
            {
                //EPlayerID playerID = GetPlayerIDTo(controllerID);
                //EventManager.Instance.Invoke_INPUT_ButtonPressed(inputButton, playerID);
                //controllersInput[controllerID].IsButtonPressed[inputButton] = true;
            }
        }
    }

    private void Check_CastedSpell(ESpellSlot spellID, EControllerID controllerID)
    {
        char controllerIDName = MaleficusUtilities.ControllerIDToChar(controllerID);
        EInputButton inputButton = MaleficusUtilities.GetInputButtonFrom(spellID);
        if (inputButton == EInputButton.NONE)
        {
            return;
        }

        if ((InputManager.Instance.IsControllerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
        {
            // Did player press button?
            if (Input.GetButtonUp("CastSpell_" + spellID + '_' + controllerIDName))
            {
                //EPlayerID playerID = GetPlayerIDTo(controllerID);
                //EventManager.Instance.Invoke_INPUT_ButtonReleased(inputButton, playerID);
                //controllersInput[controllerID].IsButtonReleased[inputButton] = true;
            }
        }
    }


    private void Check_Axis(string axisName, char axisSide, EControllerID controllerID)
    {
        char controllerIDName = MaleficusUtilities.ControllerIDToChar(controllerID);

        if ((InputManager.Instance.IsControllerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
        {
            // Did player move joystick
            float axisValue = Input.GetAxis(axisName + '_' + axisSide + '_' + controllerIDName);
            if (axisValue != 0.0f)
            {
                //EPlayerID playerID = GetPlayerIDTo(controllerID);

                // Is it a MOVE or ROTATE joystick?
                EInputAxis inputAxis = EInputAxis.MOVE_Y;
                if (axisSide == 'L')
                {
                    if (axisName == "Horizontal")
                    {
                        inputAxis = EInputAxis.MOVE_X;
                    }
                    else if (axisName == "Vertical")
                    {
                        inputAxis = EInputAxis.MOVE_Y;
                    }

                }
                else if (axisSide == 'R')
                {
                    if (axisName == "Horizontal")
                    {
                        inputAxis = EInputAxis.ROTATE_X;
                    }
                    else if (axisName == "Vertical")
                    {
                        inputAxis = EInputAxis.ROTATE_Y;
                    }
                }

                // Axis event
                //EventManager.Instance.INPUT_JoystickMoved.Invoke(new JoystickMovedEventHandle(inputAxis, axisValue, playerID));
                ControllersInput[controllerID].JoystickValues[inputAxis] = axisValue;

                // Directional button event                                                                                              
                if (axisSide == 'L')
                {
                    if (axisName == "Horizontal")
                    {
                        if ((Mathf.Abs(axisValue) > MaleficusConsts.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformHorizontalDirectionalButton[controllerID] == true))
                        {
                            canPerformHorizontalDirectionalButton[controllerID] = false;

                            if (axisValue > 0.0f)  // Which direction?  
                                                   // positive value
                            {
                                //EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.RIGHT, playerID);
                                //controllersInput[controllerID].IsButtonPressed[EInputButton.RIGHT] = true;

                            }
                            else
                            // negative value
                            {
                                //EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.LEFT, playerID);
                                //controllersInput[controllerID].IsButtonPressed[EInputButton.LEFT] = true;
                            }
                        }
                        else if ((Mathf.Abs(axisValue) < MaleficusConsts.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformHorizontalDirectionalButton[controllerID] == false))
                        {
                            canPerformHorizontalDirectionalButton[controllerID] = true;
                        }
                    }
                    else if (axisName == "Vertical")
                    {
                        if ((Mathf.Abs(axisValue) > MaleficusConsts.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformVerticalDirectionalButton[controllerID] == true))
                        {
                            canPerformVerticalDirectionalButton[controllerID] = false;

                            if (axisValue > 0.0f)    // Which direction?
                                                     // positive value
                            {
                                //EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.DOWN, playerID);
                                //controllersInput[controllerID].IsButtonPressed[EInputButton.DOWN] = true;
                            }
                            else
                            // negative value
                            {
                                //EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.UP, playerID);
                                //controllersInput[controllerID].IsButtonPressed[EInputButton.UP] = true;
                            }
                        }
                        else if ((Mathf.Abs(axisValue) < MaleficusConsts.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformVerticalDirectionalButton[controllerID] == false))
                        {
                            canPerformVerticalDirectionalButton[controllerID] = true;
                        }
                    }
                }
            }

            // Debug
            DebugManager.Instance.Log(2, "joystick " + axisName + " " + axisSide + " by " + controllerID + " : " + axisValue);
        }
    }

}
