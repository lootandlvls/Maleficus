using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.MaleficusConsts;
using static Maleficus.MaleficusUtilities;

public class GamepadInputSource : AbstractInputSource
{
    /// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    private Dictionary<EControllerID, bool> canPerformHorizontalDirectionalButton = new Dictionary<EControllerID, bool>();
    /// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    private Dictionary<EControllerID, bool> canPerformVerticalDirectionalButton = new Dictionary<EControllerID, bool>();


    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.INPUT_ControllerConnected.AddListener(On_INPUT_ControllerConnected);
        EventManager.Instance.INPUT_ControllerDisconnected.AddListener(On_INPUT_ControllerDisconnected);
    }

    /// <summary>
    /// Add controller in dictionary if connected
    /// </summary>
    private void On_INPUT_ControllerConnected(Event_GenericHandle<EControllerID, EPlayerID> evenHandle)
    {
        EControllerID controllerID = evenHandle.Arg1;

        if (controllerID.ContainedIn(GAMEPADS_CONTROLLERS))
        {
            canPerformHorizontalDirectionalButton.Add(controllerID, true);
            canPerformVerticalDirectionalButton.Add(controllerID, true);
        }
    }

    /// <summary>
    /// Remove controller from dictionary if disconnected
    /// </summary>
    private void On_INPUT_ControllerDisconnected(Event_GenericHandle<EControllerID, EPlayerID> evenHandle)
    {
        EControllerID controllerID = evenHandle.Arg1;

        if (controllerID.ContainedIn(GAMEPADS_CONTROLLERS))
        {
            canPerformHorizontalDirectionalButton.Remove(controllerID);
            canPerformVerticalDirectionalButton.Remove(controllerID);
        }
    }

 
    protected override void Update()
    {
        base.Update();

        // Confirm
        Check_Confirm(EControllerID.GAMEPAD_A);
        Check_Confirm(EControllerID.GAMEPAD_B);
        Check_Confirm(EControllerID.GAMEPAD_C);
        Check_Confirm(EControllerID.GAMEPAD_D);

        // TODO: Add missing Cancel buttons in Input Setting
        // Cancel
        Check_Cancel(EControllerID.GAMEPAD_A);
        Check_Cancel(EControllerID.GAMEPAD_B);
        Check_Cancel(EControllerID.GAMEPAD_C);
        Check_Cancel(EControllerID.GAMEPAD_D);

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

        // Left joystick 
        Check_Axis(EJoystickType.MOVEMENT, EControllerID.GAMEPAD_A);
        Check_Axis(EJoystickType.MOVEMENT, EControllerID.GAMEPAD_B);
        Check_Axis(EJoystickType.MOVEMENT, EControllerID.GAMEPAD_C);
        Check_Axis(EJoystickType.MOVEMENT, EControllerID.GAMEPAD_D);

        // Right joystick 
        Check_Axis(EJoystickType.ROTATION, EControllerID.GAMEPAD_A);
        Check_Axis(EJoystickType.ROTATION, EControllerID.GAMEPAD_B);
        Check_Axis(EJoystickType.ROTATION, EControllerID.GAMEPAD_C);
        Check_Axis(EJoystickType.ROTATION, EControllerID.GAMEPAD_D);

    }

    private void Check_Confirm(EControllerID controllerID)
    {

        char controllerIDName = ControllerIDToChar(controllerID);
        if (controllerIDName != 'X')
        {
            if (Input.GetButtonDown("Confirm_" + controllerIDName))
            {

                if ((InputManager.Instance.IsControllerConnected(controllerID) == true)
                    || (InputManager.Instance.InputMode == EInputMode.TEST))
                {
                    DebugLog("Confirm pressed by : " + controllerID, "Input");
                    InvokeButtonPressed(controllerID, EInputButton.CONFIRM);
                }
                // Connect controller
                else if ((AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_CONNECTING_GAMEPADS)
                    || (MotherOfManagers.Instance.IsConnectControllerInAnyState == true))
                {
                    EPlayerID playerID = PlayerManager.Instance.GetNextFreePlayerID();
                    if ((playerID != EPlayerID.TEST)
                        && (playerID != EPlayerID.NONE))
                    {
                        DebugLog("Connecting : " + controllerID, "Deafult");
                        InputManager.Instance.ConnectControllerToPlayer(controllerID, playerID);
                    }
                }
            }
        }
    }

    private void Check_Cancel(EControllerID controllerID)         // TODO: Test with controller if it works
    {
        // Is button pressed?
        char controllerIDName = ControllerIDToChar(controllerID);
        if (controllerIDName != 'X')
        {
            if (Input.GetButtonDown("Cancel_" + controllerIDName))
            {
                if ((InputManager.Instance.IsControllerConnected(controllerID) == true)
                    || (InputManager.Instance.InputMode == EInputMode.TEST))
                {
                    // Disconnect Player
                    if ((AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_CONNECTING_GAMEPADS)
                        | (MotherOfManagers.Instance.IsConnectControllerInAnyState == true))
                    {
                        InputManager.Instance.DisconnectController(controllerID);
                    }
                    else // Not in connecting players state
                    {
                        InvokeButtonPressed(controllerID, EInputButton.CANCEL);
                    }
                }
            }
        }
    }

    private void Check_ChargingSpell(ESpellSlot spellSlot, EControllerID controllerID)
    {
        char controllerIDName = ControllerIDToChar(controllerID);
        int spellID = SpellSlotToInt(spellSlot);

        if ((controllerIDName != 'X')  && (spellID != 0))
        {
            // Is button pressed?
            if (Input.GetButtonDown("CastSpell_" + spellID + '_' + controllerIDName))
            {
                EInputButton inputButton = GetInputButtonFrom(spellSlot);
                if (inputButton != EInputButton.NONE)
                {
                    if ((InputManager.Instance.IsControllerConnected(controllerID) == true)
                        || (InputManager.Instance.InputMode == EInputMode.TEST))
                    {
                        InvokeButtonPressed(controllerID, inputButton);
                    }
                }
            }
        }
    }

    private void Check_CastedSpell(ESpellSlot spellSlot, EControllerID controllerID)
    {
        char controllerIDName = ControllerIDToChar(controllerID);
        int spellID = SpellSlotToInt(spellSlot);

        if ((controllerIDName != 'X') && (spellID != 0))
        {
            // Is button pressed?
            if (Input.GetButtonUp("CastSpell_" + spellID + '_' + controllerIDName))
            {
                EInputButton inputButton = GetInputButtonFrom(spellSlot);
                if (inputButton != EInputButton.NONE)
                {
                    if ((InputManager.Instance.IsControllerConnected(controllerID) == true)
                        || (InputManager.Instance.InputMode == EInputMode.TEST))
                    {
                        InvokeButtonReleased(controllerID, inputButton);
                    }
                }
            }
        }
    }

    private void Check_Axis(EJoystickType joystickType, EControllerID controllerID)
    {
        char controllerIDName = ControllerIDToChar(controllerID);
        char axisSide = JoystickTypeToChar(joystickType);
        if (axisSide != 'X')
        {
            if ((InputManager.Instance.IsControllerConnected(controllerID) == true)
                || (InputManager.Instance.InputMode == EInputMode.TEST))
            {
                float x = Input.GetAxis("Horizontal" + '_' + axisSide + '_' + controllerIDName);
                float y = Input.GetAxis("Vertical" + '_' + axisSide + '_' + controllerIDName);
                DebugManager.Instance.Log(2, controllerID +  "> x : " + x + " | y : " + y);

                InvokeJoystickMoved(controllerID, joystickType, x, y);
            }
        }
    }

    private void ReInitializeDirectionalMaps()
    {
        foreach (EControllerID controllerID in InputManager.Instance.ConnectedControllers.Keys)
        {
            canPerformHorizontalDirectionalButton[controllerID] = true;
        }

        foreach (EControllerID controllerID in InputManager.Instance.ConnectedControllers.Keys)
        {
            canPerformVerticalDirectionalButton[controllerID] = true;
        }
    }
}
