using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.Consts;
using static Maleficus.Utils;

public class GamepadInputSource : AbstractInputSource
{
    ///// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    //private Dictionary<EControllerID, bool> canPerformHorizontalDirectionalButton = new Dictionary<EControllerID, bool>();
    ///// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    //private Dictionary<EControllerID, bool> canPerformVerticalDirectionalButton = new Dictionary<EControllerID, bool>();


    //protected override void InitializeEventsCallbacks()
    //{
    //    base.InitializeEventsCallbacks();

    //    EventManager.Instance.INPUT_ControllerConnected.AddListener(On_INPUT_ControllerConnected);
    //    EventManager.Instance.INPUT_ControllerDisconnected.AddListener(On_INPUT_ControllerDisconnected);
    //}

    ///// <summary>
    ///// Add controller in dictionary if connected
    ///// </summary>
    //private void On_INPUT_ControllerConnected(Event_GenericHandle<EControllerID> evenHandle)
    //{
    //    EControllerID controllerID = evenHandle.Arg1;

    //    if (controllerID.ContainedIn(GAMEPADS_CONTROLLERS))
    //    {
    //        canPerformHorizontalDirectionalButton.Add(controllerID, true);
    //        canPerformVerticalDirectionalButton.Add(controllerID, true);
    //    }
    //}

    ///// <summary>
    ///// Remove controller from dictionary if disconnected
    ///// </summary>
    //private void On_INPUT_ControllerDisconnected(Event_GenericHandle<EControllerID> evenHandle)
    //{
    //    EControllerID controllerID = evenHandle.Arg1;

    //    if (controllerID.ContainedIn(GAMEPADS_CONTROLLERS))
    //    {
    //        canPerformHorizontalDirectionalButton.Remove(controllerID);
    //        canPerformVerticalDirectionalButton.Remove(controllerID);
    //    }
    //}

 
    //protected override void Update()
    //{
    //    base.Update();

    //    // Confirm
    //    Check_Confirm(EControllerID.GAMEPAD_1);
    //    Check_Confirm(EControllerID.GAMEPAD_2);
    //    Check_Confirm(EControllerID.GAMEPAD_3);
    //    Check_Confirm(EControllerID.GAMEPAD_4);

    //    // TODO: Add missing Cancel buttons in Input Setting
    //    // Cancel
    //    Check_Cancel(EControllerID.GAMEPAD_1);
    //    Check_Cancel(EControllerID.GAMEPAD_2);
    //    Check_Cancel(EControllerID.GAMEPAD_3);
    //    Check_Cancel(EControllerID.GAMEPAD_4);

    //    /* Charging spell check */
    //    // Spell 1
    //    Check_ChargingSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_1);
    //    Check_ChargingSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_2);
    //    Check_ChargingSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_3);
    //    Check_ChargingSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_4);
    //    //spell 2
    //    Check_ChargingSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_1);
    //    Check_ChargingSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_2);
    //    Check_ChargingSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_3);
    //    Check_ChargingSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_4);
    //    // spell 3
    //    Check_ChargingSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_1);
    //    Check_ChargingSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_2);
    //    Check_ChargingSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_3);
    //    Check_ChargingSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_4);

    //    /* Casted spell check */
    //    // Spell 1
    //    Check_CastedSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_1);
    //    Check_CastedSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_2);
    //    Check_CastedSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_3);
    //    Check_CastedSpell(ESpellSlot.SPELL_1, EControllerID.GAMEPAD_4);
    //    //spell 2
    //    Check_CastedSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_1);
    //    Check_CastedSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_2);
    //    Check_CastedSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_3);
    //    Check_CastedSpell(ESpellSlot.SPELL_2, EControllerID.GAMEPAD_4);
    //    // spell 3
    //    Check_CastedSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_1);
    //    Check_CastedSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_2);
    //    Check_CastedSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_3);
    //    Check_CastedSpell(ESpellSlot.SPELL_3, EControllerID.GAMEPAD_4);

    //    // Left joystick 
    //    Check_Axis(EJoystickType.MOVEMENT, EControllerID.GAMEPAD_1);
    //    Check_Axis(EJoystickType.MOVEMENT, EControllerID.GAMEPAD_2);
    //    Check_Axis(EJoystickType.MOVEMENT, EControllerID.GAMEPAD_3);
    //    Check_Axis(EJoystickType.MOVEMENT, EControllerID.GAMEPAD_4);

    //    // Right joystick 
    //    Check_Axis(EJoystickType.ROTATION, EControllerID.GAMEPAD_1);
    //    Check_Axis(EJoystickType.ROTATION, EControllerID.GAMEPAD_2);
    //    Check_Axis(EJoystickType.ROTATION, EControllerID.GAMEPAD_3);
    //    Check_Axis(EJoystickType.ROTATION, EControllerID.GAMEPAD_4);

    //}

    //private void Check_Confirm(EControllerID controllerID)
    //{

    //    char controllerIDName = GetCharFrom(controllerID);
    //    if (controllerIDName != 'X')
    //    {
    //        if (Input.GetButtonDown("Confirm_" + controllerIDName))
    //        {
    //            if ((InputManager.Instance.IsControllerConnected(controllerID) == true)
    //                || (InputManager.Instance.InputMode == EInputMode.TEST))
    //            {
    //                LogConsole("Confirm pressed by : " + controllerID, "Input");
    //                InvokeButtonPressed(controllerID, EInputButton.CONFIRM);
    //            }
    //            // Connect controller
    //            else if ((AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_SPELL_SELECTION)
    //                || (MotherOfManagers.Instance.IsConnectControllerInAnyState == true))
    //            {
    //                InputManager.Instance.ConnectController(controllerID);
    //            }
    //        }
    //    }
    //}

    //private void Check_Cancel(EControllerID controllerID)         // TODO: Test with controller if it works
    //{
    //    // Is button pressed?
    //    char controllerIDName = GetCharFrom(controllerID);
    //    if (controllerIDName != 'X')
    //    {
    //        if (Input.GetButtonDown("Cancel_" + controllerIDName))
    //        {
    //            if ((InputManager.Instance.IsControllerConnected(controllerID) == true)
    //                || (InputManager.Instance.InputMode == EInputMode.TEST))
    //            {
    //                // Disconnect Player
    //                if ((AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_SPELL_SELECTION)
    //                    | (MotherOfManagers.Instance.IsConnectControllerInAnyState == true))
    //                {
    //                    InputManager.Instance.DisconnectController(controllerID);
    //                }
    //                else // Not in connecting players state
    //                {
    //                    InvokeButtonPressed(controllerID, EInputButton.CANCEL);
    //                }
    //            }
    //        }
    //    }
    //}

    //private void Check_ChargingSpell(ESpellSlot spellSlot, EControllerID controllerID)
    //{
    //    char controllerIDName = GetCharFrom(controllerID);
    //    int spellID = GetIntFrom(spellSlot);

    //    if ((controllerIDName != 'X')  && (spellID != 0))
    //    {
    //        // Is button pressed?
    //        if (Input.GetButtonDown("CastSpell_" + spellID + '_' + controllerIDName))
    //        {
    //            EInputButton inputButton = GetInputButtonFrom(spellSlot);
    //            if (inputButton != EInputButton.NONE)
    //            {
    //                if ((InputManager.Instance.IsControllerConnected(controllerID) == true)
    //                    || (InputManager.Instance.InputMode == EInputMode.TEST))
    //                {
    //                    InvokeButtonPressed(controllerID, inputButton);
    //                }
    //            }
    //        }
    //    }
    //}

    //private void Check_CastedSpell(ESpellSlot spellSlot, EControllerID controllerID)
    //{
    //    char controllerIDName = GetCharFrom(controllerID);
    //    int spellID = GetIntFrom(spellSlot);

    //    if ((controllerIDName != 'X') && (spellID != 0))
    //    {
    //        // Is button pressed?
    //        if (Input.GetButtonUp("CastSpell_" + spellID + '_' + controllerIDName))
    //        {
    //            EInputButton inputButton = GetInputButtonFrom(spellSlot);
    //            if (inputButton != EInputButton.NONE)
    //            {
    //                if ((InputManager.Instance.IsControllerConnected(controllerID) == true)
    //                    || (InputManager.Instance.InputMode == EInputMode.TEST))
    //                {
    //                    InvokeButtonReleased(controllerID, inputButton);
    //                }
    //            }
    //        }
    //    }
    //}

    //private void Check_Axis(EJoystickType joystickType, EControllerID controllerID)
    //{
    //    char controllerIDName = GetCharFrom(controllerID);
    //    char axisSide = GetCharFrom(joystickType);
    //    if (axisSide != 'X')
    //    {
    //        if ((InputManager.Instance.IsControllerConnected(controllerID) == true)
    //            || (InputManager.Instance.InputMode == EInputMode.TEST))
    //        {
    //            float x = Input.GetAxis("Horizontal" + '_' + axisSide + '_' + controllerIDName);
    //            float y = Input.GetAxis("Vertical" + '_' + axisSide + '_' + controllerIDName);
    //            DebugManager.Instance.Log(2, controllerID +  "> x : " + x + " | y : " + y);

    //            InvokeJoystickMoved(controllerID, joystickType, x, y);
    //        }
    //    }
    //}

    //private void ReInitializeDirectionalMaps()
    //{
    //    //foreach (EControllerID controllerID in InputManager.Instance.connectedControllers)
    //    //{
    //    //    canPerformHorizontalDirectionalButton[controllerID] = true;
    //    //}

    //    //foreach (EControllerID controllerID in InputManager.Instance.connectedControllers)
    //    //{
    //    //    canPerformVerticalDirectionalButton[controllerID] = true;
    //    //}
    //}
}
