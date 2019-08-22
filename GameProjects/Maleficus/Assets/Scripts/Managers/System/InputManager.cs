using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputManager : AbstractSingletonManager<InputManager>
{
    public EInputMode InputMode { get { return MotherOfManagers.Instance.InputMode; } }

    public EPlayerID TouchPlayerID { get; set; }

    /// <summary> Mapping from controllerID to playerID </summary> 
    private Dictionary<EControllerID, EPlayerID> playerControllerMapping = new Dictionary<EControllerID, EPlayerID>();

    /// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    private Dictionary<EPlayerID, bool> canPerformHorizontalDirectionalButton = new Dictionary<EPlayerID, bool>();

    /// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    private Dictionary<EPlayerID, bool> canPerformVerticalDirectionalButton = new Dictionary<EPlayerID, bool>();

    protected override void Awake()
    {
        base.Awake();

        InitializeDirectionalMaps();
    }

    private void Start()
    {

    }

    public override void Initialize()
    {
        InitializeDirectionalMaps();
    }

    private void Update()
    {
        CheckButtonsAndJoysticks();

    }



    #region Controller
    private void CheckButtonsAndJoysticks()
    {
        // Confirm
        Check_Confirm(EControllerID.CONTROLLER_A);
        Check_Confirm(EControllerID.CONTROLLER_B);
        Check_Confirm(EControllerID.CONTROLLER_C);
        Check_Confirm(EControllerID.CONTROLLER_D);

        // TODO: Add missing Cancel buttons in Input Setting
        // Cancel
        //Check_Cancel(EControllerID.CONTROLLER_A);
        //Check_Cancel(EControllerID.CONTROLLER_B);
        //Check_Cancel(EControllerID.CONTROLLER_C);
        //Check_Cancel(EControllerID.CONTROLLER_D);

        /* Charging spell check */
        // Spell 1
        Check_ChargingSpell(1, EControllerID.CONTROLLER_A);     // TODO [BNJMO]: Change to SpellID
        Check_ChargingSpell(1, EControllerID.CONTROLLER_B);
        Check_ChargingSpell(1, EControllerID.CONTROLLER_C);
        Check_ChargingSpell(1, EControllerID.CONTROLLER_D);
        //spell 2
        Check_ChargingSpell(2, EControllerID.CONTROLLER_A);
        Check_ChargingSpell(2, EControllerID.CONTROLLER_B);
        Check_ChargingSpell(2, EControllerID.CONTROLLER_C);
        Check_ChargingSpell(2, EControllerID.CONTROLLER_D);
        // spell 3
        Check_ChargingSpell(3, EControllerID.CONTROLLER_A);
        Check_ChargingSpell(3, EControllerID.CONTROLLER_B);
        Check_ChargingSpell(3, EControllerID.CONTROLLER_C);
        Check_ChargingSpell(3, EControllerID.CONTROLLER_D);

        /* Casted spell check */
        // Spell 1
        Check_CastedSpell(1, EControllerID.CONTROLLER_A);
        Check_CastedSpell(1, EControllerID.CONTROLLER_B);
        Check_CastedSpell(1, EControllerID.CONTROLLER_C);
        Check_CastedSpell(1, EControllerID.CONTROLLER_D);
        //spell 2
        Check_CastedSpell(2, EControllerID.CONTROLLER_A);
        Check_CastedSpell(2, EControllerID.CONTROLLER_B);
        Check_CastedSpell(2, EControllerID.CONTROLLER_C);
        Check_CastedSpell(2, EControllerID.CONTROLLER_D);
        // spell 3
        Check_CastedSpell(3, EControllerID.CONTROLLER_A);
        Check_CastedSpell(3, EControllerID.CONTROLLER_B);
        Check_CastedSpell(3, EControllerID.CONTROLLER_C);
        Check_CastedSpell(3, EControllerID.CONTROLLER_D);

        // Horizontal 
        // Left 
        Check_Axis("Horizontal", 'L', EControllerID.CONTROLLER_A);
        Check_Axis("Horizontal", 'L', EControllerID.CONTROLLER_B);
        Check_Axis("Horizontal", 'L', EControllerID.CONTROLLER_C);
        Check_Axis("Horizontal", 'L', EControllerID.CONTROLLER_D);

        // Right
        Check_Axis("Horizontal", 'R', EControllerID.CONTROLLER_A);
        Check_Axis("Horizontal", 'R', EControllerID.CONTROLLER_B);
        Check_Axis("Horizontal", 'R', EControllerID.CONTROLLER_C);
        Check_Axis("Horizontal", 'R', EControllerID.CONTROLLER_D);

        // Vertical 
        // Left 
        Check_Axis("Vertical", 'L', EControllerID.CONTROLLER_A);
        Check_Axis("Vertical", 'L', EControllerID.CONTROLLER_B);
        Check_Axis("Vertical", 'L', EControllerID.CONTROLLER_C);
        Check_Axis("Vertical", 'L', EControllerID.CONTROLLER_D);

        // Right
        Check_Axis("Vertical", 'R', EControllerID.CONTROLLER_A);
        Check_Axis("Vertical", 'R', EControllerID.CONTROLLER_B);
        Check_Axis("Vertical", 'R', EControllerID.CONTROLLER_C);
        Check_Axis("Vertical", 'R', EControllerID.CONTROLLER_D);
    }


    private void Check_Confirm(EControllerID controllerID)
    {
        char controllerIDName = MaleficusUtilities.ControllerIDToChar(controllerID);
        if (Input.GetButtonDown("Confirm_" + controllerIDName))
        {
            if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
            {
                // Send Input
                EPlayerID playerID = GetPlayerID(controllerID);
                EventManager.Instance.INPUT_ButtonPressed.Invoke(new SpellInputEventHandle(EInputButton.CONFIRM, playerID, true), false);
            }
            else //if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_CONNECTING_PLAYERS)
                 // Connect players
            {
                EPlayerID connectedPlayerID = PlayerManager.Instance.ConnectNextPlayerToController();
                if (connectedPlayerID != EPlayerID.TEST)
                {
                    playerControllerMapping[controllerID] = connectedPlayerID;
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
            if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
            {
                EPlayerID playerID = GetPlayerID(controllerID);

                if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_CONNECTING_PLAYERS)
                // Disconnect Player
                {
                    PlayerManager.Instance.DisconnectPlayer(playerID);
                    playerControllerMapping.Remove(controllerID);
                }
                else // Not in connecting players state
                {
                    EventManager.Instance.INPUT_ButtonPressed.Invoke(new SpellInputEventHandle(EInputButton.CANCEL, playerID, true), false);
                }
            }
        }
    }


    private void Check_ChargingSpell(int spellID, EControllerID controllerID)
    {
        char controllerIDName = MaleficusUtilities.ControllerIDToChar(controllerID);

        if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
        {
            // Did player press button?
            if (Input.GetButtonDown("CastSpell_" + spellID + '_' + controllerIDName))
            {
                EPlayerID playerID = GetPlayerID(controllerID);
                SpellInputEventHandle eventHandle = new SpellInputEventHandle(EInputButton.CAST_SPELL_1, playerID, true);
                if (spellID == 1)
                {
                    eventHandle = new SpellInputEventHandle(EInputButton.CAST_SPELL_1, playerID, true);
                }
                else if (spellID == 2)
                {
                    eventHandle = new SpellInputEventHandle(EInputButton.CAST_SPELL_2, playerID, true);
                }
                else if (spellID == 3)
                {
                    eventHandle = new SpellInputEventHandle(EInputButton.CAST_SPELL_3, playerID, true);
                }
                EventManager.Instance.INPUT_ButtonPressed.Invoke(eventHandle);
            }
        }
    }

    private void Check_CastedSpell(int spellID, EControllerID controllerID)
    {
        char controllerIDName = MaleficusUtilities.ControllerIDToChar(controllerID);

        if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
        {
            // Did player press button?
            if (Input.GetButtonUp("CastSpell_" + spellID + '_' + controllerIDName))
            {
                EPlayerID playerID = GetPlayerID(controllerID);
                SpellInputEventHandle eventHandle = new SpellInputEventHandle(EInputButton.CAST_SPELL_1, playerID, false);
                if (spellID == 1)
                {
                    eventHandle = new SpellInputEventHandle(EInputButton.CAST_SPELL_1, playerID, false);
                }
                else if (spellID == 2)
                {
                    eventHandle = new SpellInputEventHandle(EInputButton.CAST_SPELL_2, playerID, false);
                }
                else if (spellID == 3)
                {
                    eventHandle = new SpellInputEventHandle(EInputButton.CAST_SPELL_3, playerID, false);
                }
                EventManager.Instance.INPUT_ButtonPressed.Invoke(eventHandle);
            }
        }
    }


    private void Check_Axis(string axisName, char axisSide, EControllerID controllerID)
    {
        char controllerIDName = MaleficusUtilities.ControllerIDToChar(controllerID);

        if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
        {
            // Did player move joystick
            float axisValue = Input.GetAxis(axisName + '_' + axisSide + '_' + controllerIDName);
            if (axisValue != 0.0f)
            {
                EPlayerID playerID = GetPlayerID(controllerID);

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
                EventManager.Instance.INPUT_Movement.Invoke(new MovementInputEventHandle(inputAxis, axisValue, playerID));


                // Directional button event                                                                                              
                if (axisSide == 'L')
                {
                    if (axisName == "Horizontal")
                    {
                        if ((Mathf.Abs(axisValue) > MaleficusConsts.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformHorizontalDirectionalButton[playerID] == true))
                        {
                            canPerformHorizontalDirectionalButton[playerID] = false;

                            if (axisValue > 0.0f)  // Which direction?  
                                                   // positive value
                            {
                                EventManager.Instance.INPUT_ButtonPressed.Invoke(new SpellInputEventHandle(EInputButton.RIGHT, playerID, true));
                            }
                            else
                            // negative value
                            {
                                EventManager.Instance.INPUT_ButtonPressed.Invoke(new SpellInputEventHandle(EInputButton.LEFT, playerID, true));
                            }
                        }
                        else if ((Mathf.Abs(axisValue) < MaleficusConsts.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformHorizontalDirectionalButton[playerID] == false))
                        {
                            canPerformHorizontalDirectionalButton[playerID] = true;
                        }
                    }
                    else if (axisName == "Vertical")
                    {
                        if ((Mathf.Abs(axisValue) > MaleficusConsts.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformVerticalDirectionalButton[playerID] == true))
                        {
                            canPerformVerticalDirectionalButton[playerID] = false;

                            if (axisValue > 0.0f)    // Which direction?
                                                     // positive value
                            {
                                EventManager.Instance.INPUT_ButtonPressed.Invoke(new SpellInputEventHandle(EInputButton.DOWN, playerID, true));
                            }
                            else
                            // negative value
                            {
                                EventManager.Instance.INPUT_ButtonPressed.Invoke(new SpellInputEventHandle(EInputButton.UP, playerID, true));
                            }
                        }
                        else if ((Mathf.Abs(axisValue) < MaleficusConsts.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformVerticalDirectionalButton[playerID] == false))
                        {
                            canPerformVerticalDirectionalButton[playerID] = true;
                        }
                    }
                }
            }

            // Debug
            DebugManager.Instance.Log(2, "joystick " + axisName + " " + axisSide + " by " + controllerID + " : " + axisValue);
        }
    }
    #endregion

    #region Touch
    public void OnTouchJoystickPressed(ETouchJoystickType joystickType)
    {
        if (InputMode == EInputMode.TOUCH)
        {
            EInputButton inputButton = MaleficusUtilities.GetInputButtonFrom(joystickType);
            if (inputButton != EInputButton.NONE)
            {
                EventManager.Instance.INPUT_ButtonPressed.Invoke(new SpellInputEventHandle(inputButton, TouchPlayerID, true));
            }
        }
    }

    public void OnTouchJoystickMoved(Vector2 joystickInput, ETouchJoystickType joystickType)
    {
        if ((InputMode == EInputMode.TOUCH) && (TouchPlayerID != EPlayerID.NONE))
        {
            if (joystickType == ETouchJoystickType.MOVE)
            {
                float x = joystickInput.x;
                float y = joystickInput.y;
                Vector3 cameraForward = Camera.main.transform.forward;
                MaleficusUtilities.TransformAxisToCamera(ref x, ref y, cameraForward);

                EventManager.Instance.INPUT_Movement.Invoke(new MovementInputEventHandle(EInputAxis.MOVE_X, x, TouchPlayerID));
                EventManager.Instance.INPUT_Movement.Invoke(new MovementInputEventHandle(EInputAxis.MOVE_Y, y, TouchPlayerID));
            }
            else // Spell joystick
            {
                float x = joystickInput.x;
                float y = -joystickInput.y;
                Vector3 cameraForward = Camera.main.transform.forward;
                MaleficusUtilities.TransformAxisToCamera(ref x, ref y, cameraForward, true);

                EventManager.Instance.INPUT_Movement.Invoke(new MovementInputEventHandle(EInputAxis.ROTATE_X, x, TouchPlayerID));
                EventManager.Instance.INPUT_Movement.Invoke(new MovementInputEventHandle(EInputAxis.ROTATE_Y, y, TouchPlayerID));
            }
        }
    }


    public void OnTouchJoystickReleased(ETouchJoystickType joystickType)
    {
        if (InputMode == EInputMode.TOUCH)
        {
            EInputButton inputButton = MaleficusUtilities.GetInputButtonFrom(joystickType);
            if (inputButton != EInputButton.NONE)
            {
                EventManager.Instance.INPUT_ButtonReleased.Invoke(new SpellInputEventHandle(inputButton, TouchPlayerID, false));
            }
        }
    }
    #endregion

    private EPlayerID GetPlayerID(EControllerID controllerID)
    {
        if (InputMode == EInputMode.TEST)
        {
            return EPlayerID.TEST;
        }
        return playerControllerMapping[controllerID];
    }

    private EControllerID GetControllerID(EPlayerID playerID)
    {
        if (playerControllerMapping[EControllerID.CONTROLLER_A] == playerID)
        {
            return EControllerID.CONTROLLER_A;
        }
        if (playerControllerMapping[EControllerID.CONTROLLER_B] == playerID)
        {
            return EControllerID.CONTROLLER_B;
        }
        if (playerControllerMapping[EControllerID.CONTROLLER_C] == playerID)
        {
            return EControllerID.CONTROLLER_C;
        }
        if (playerControllerMapping[EControllerID.CONTROLLER_D] == playerID)
        {
            return EControllerID.CONTROLLER_D;
        }
        return EControllerID.NONE;
    }

    private void InitializeDirectionalMaps()
    {
        canPerformHorizontalDirectionalButton.Clear();
        canPerformHorizontalDirectionalButton[EPlayerID.TEST] = true; // test ID
        canPerformHorizontalDirectionalButton[EPlayerID.PLAYER_1] = true;
        canPerformHorizontalDirectionalButton[EPlayerID.PLAYER_2] = true;
        canPerformHorizontalDirectionalButton[EPlayerID.PLAYER_3] = true;
        canPerformHorizontalDirectionalButton[EPlayerID.PLAYER_4] = true;

        canPerformVerticalDirectionalButton.Clear();
        canPerformVerticalDirectionalButton[EPlayerID.TEST] = true; // test ID
        canPerformVerticalDirectionalButton[EPlayerID.PLAYER_1] = true;
        canPerformVerticalDirectionalButton[EPlayerID.PLAYER_2] = true;
        canPerformVerticalDirectionalButton[EPlayerID.PLAYER_3] = true;
        canPerformVerticalDirectionalButton[EPlayerID.PLAYER_4] = true;
    }


    private bool IsPlayerConnected(EControllerID controllerID)
    {
        return playerControllerMapping.ContainsKey(controllerID);
    }

}
