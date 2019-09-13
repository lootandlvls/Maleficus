using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputManager : AbstractSingletonManager<InputManager>
{
    public EInputMode                                   InputMode                               { get { return MotherOfManagers.Instance.InputMode; } }

    /// <summary> Controller inputs for every player (movement, rotation and button press/spell casts). </summary>
    public Dictionary<EControllerID, ControllerInput>   ControllersInput                        { get { return controllersInput; } }


    /// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    private Dictionary<EControllerID, bool>             canPerformHorizontalDirectionalButton   = new Dictionary<EControllerID, bool>();          

    /// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    private Dictionary<EControllerID, bool>             canPerformVerticalDirectionalButton     = new Dictionary<EControllerID, bool>();

    private Dictionary<EControllerID, ControllerInput>  controllersInput                        = new Dictionary<EControllerID, ControllerInput>();


    protected override void Awake()
    {
        base.Awake();

        ReInitializeDirectionalMaps();
    }

    private void Start()
    {
        // Listen to broadcasted inputs 
        EventManager.Instance.INPUT_JoystickMoved.AddListener(On_NeT_INPUT_JoystickMoved);

    }

    
    public override void OnSceneStartReinitialize()
    {
        ReInitializeDirectionalMaps();
    }

    private void Update()
    {
        //FlushControllersInput();

        CheckButtonsAndJoysticksInput();

    }

    private void LateUpdate()
    {
        //BroadcastLocalControllersInput();
    }



    #region Controller Input
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
            if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
            {
                // Send Input
                //EPlayerID playerID = GetPlayerIDTo(controllerID);
                //EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.CONFIRM, playerID);
                //controllersInput[controllerID].IsButtonPressed[EInputButton.CONFIRM] = true;
            }
            else //if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_CONNECTING_PLAYERS)
                // Connect players
            {
                EPlayerID connectedPlayerID = PlayerManager.Instance.ConnectNextPlayerToController(controllerID);
                if (connectedPlayerID != EPlayerID.TEST)
                {
                    //playerControllerMapping.Add(controllerID, connectedPlayerID);
                    ConnectController(controllerID);
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
                //EPlayerID playerID = GetPlayerIDTo(controllerID);

                if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_CONNECTING_PLAYERS)
                    // Disconnect Player
                {
                    PlayerManager.Instance.DisconnectPlayer(controllerID);
                    //playerControllerMapping.Remove(controllerID);
                    DisconnectController(controllerID);
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

        if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
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

        if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
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

        if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
        {
            // Did player move joystick
            float axisValue = Input.GetAxis(axisName + '_' + axisSide + '_' + controllerIDName);
            if   (axisValue != 0.0f)
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
    #endregion

    #region Touch Input
    public void OnTouchJoystickPressed(ETouchJoystickType touchJoystickType)
    {
        if (InputMode == EInputMode.TOUCH)
        {
            EPlayerID playerID = PlayerManager.Instance.GetPlayerIDFrom(EControllerID.TOUCH);
            EClientID clientID = MaleficusUtilities.GetClientIDFrom(playerID);

            EInputButton inputButton = MaleficusUtilities.GetInputButtonFrom(touchJoystickType);
            Debug.Log("inputButton : " + inputButton + " | joystickType : " + touchJoystickType);
            if (inputButton != EInputButton.NONE)
            {
                EventManager.Instance.INPUT_ButtonPressed.Invoke(new ButtonPressedEventHandle(clientID, inputButton));
                //controllersInput[EControllerID.TOUCH].IsButtonPressed[inputButton] = true;
            }
        }
    }

    public void OnTouchJoystickMoved(Vector2 joystickInput, ETouchJoystickType touchJoystickType)
    {
        Vector2 newInput = joystickInput;

        if ((InputMode == EInputMode.TOUCH)
            && (ControllersInput.ContainsKey(EControllerID.TOUCH))
            && (Mathf.Abs(newInput.x) + Mathf.Abs(newInput.y) > MaleficusConsts.THRESHOLD_JOYSTICK_ACTIVATION))
        {
            EPlayerID playerID = PlayerManager.Instance.GetPlayerIDFrom(EControllerID.TOUCH);
            EClientID clientID = MaleficusUtilities.GetClientIDFrom(playerID);

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

                    joystickValues[EInputAxis.MOVE_X] = newInput.x;
                    joystickValues[EInputAxis.MOVE_Y] = newInput.y;



                    // TODO:  check if correct state

                    EventManager.Instance.INPUT_JoystickMoved.Invoke(new JoystickMovedEventHandle(clientID, joysticksType, newInput.x, newInput.y));
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

                    joystickValues[EInputAxis.ROTATE_X] = newInput.x;
                    joystickValues[EInputAxis.ROTATE_Y] = newInput.y;


                    // TODO:  check if correct state
                    EventManager.Instance.INPUT_JoystickMoved.Invoke(new JoystickMovedEventHandle(clientID, joysticksType, newInput.x, newInput.y));
                }
            }

        }
    }

    public void OnTouchJoystickReleased(ETouchJoystickType touchJoystickType)
    {
        if ((InputMode == EInputMode.TOUCH)
            && (ControllersInput.ContainsKey(EControllerID.TOUCH)))
        {
            EPlayerID playerID = PlayerManager.Instance.GetPlayerIDFrom(EControllerID.TOUCH);
            EClientID clientID = MaleficusUtilities.GetClientIDFrom(playerID);

            // Reinitialize Movement and Rotation
            if (touchJoystickType == ETouchJoystickType.MOVE)
            {
                EJoystickType joystickType = EJoystickType.MOVEMENT;

                ControllersInput[EControllerID.TOUCH].JoystickValues[EInputAxis.MOVE_X] = 0.0f;
                ControllersInput[EControllerID.TOUCH].JoystickValues[EInputAxis.MOVE_Y] = 0.0f;

                                                                                                        // TODO:  check if correct state
                EventManager.Instance.INPUT_JoystickMoved.Invoke(new JoystickMovedEventHandle(clientID, joystickType, 0.0f, 0.0f));
            }
            else
            {
                EJoystickType joystickType = EJoystickType.ROTATION;

                ControllersInput[EControllerID.TOUCH].JoystickValues[EInputAxis.ROTATE_X] = 0.0f;
                ControllersInput[EControllerID.TOUCH].JoystickValues[EInputAxis.ROTATE_Y] = 0.0f;
                
                                                                                                            // TODO:  check if correct state
                EventManager.Instance.INPUT_JoystickMoved.Invoke(new JoystickMovedEventHandle(clientID, joystickType, 0.0f, 0.0f));
            }

            // Specific Button Released
            EInputButton inputButton = MaleficusUtilities.GetInputButtonFrom(touchJoystickType);
            if (inputButton != EInputButton.NONE)
            {
                                                                                                                    // TODO:  check if correct state
                EventManager.Instance.INPUT_ButtonReleased.Invoke(new ButtonReleasedEventHandle(clientID, inputButton));

                //ControllersInput[EControllerID.TOUCH].IsButtonReleased[inputButton] = true;
            }
        }
    }
    #endregion

    #region Network Input
    private void On_NeT_INPUT_JoystickMoved(JoystickMovedEventHandle eventHandle)
    {
        EJoystickType joystickType = eventHandle.JoystickType;
        float joystick_X = eventHandle.Joystick_X;
        float joystick_Y = eventHandle.Joystick_Y;
        EPlayerID playerID = MaleficusUtilities.GetPlayerIDFrom(eventHandle.SenderID);

        if (playerID != NetworkManager.Instance.OwnPlayerID)
        {

            EControllerID controllerID = PlayerManager.Instance.GetControllerFrom(playerID);
            if (controllerID.ContainedIn(MaleficusConsts.NETWORK_CONTROLLERS))
            {
                if (joystickType == EJoystickType.MOVEMENT)
                {
                    ControllersInput[controllerID].JoystickValues[EInputAxis.MOVE_X] = joystick_X;
                    ControllersInput[controllerID].JoystickValues[EInputAxis.MOVE_Y] = joystick_Y;
                }
                else if (joystickType == EJoystickType.ROTATION)
                {
                    ControllersInput[controllerID].JoystickValues[EInputAxis.ROTATE_X] = joystick_X;
                    ControllersInput[controllerID].JoystickValues[EInputAxis.ROTATE_Y] = joystick_Y;
                }
            }
        }
    }

    //private void On_INPUT_ButtonPressed(ButtonPressedEventHandle eventHandle)
    //{

    //}

    //private void On_INPUT_ButtonReleased(ButtonReleasedEventHandle eventHandle)
    //{

    //}
    #endregion

    public void ConnectController(EControllerID controllerID)
    {
        if (ControllersInput.ContainsKey(controllerID) == true)
        {
            Debug.LogError("Trying to connect a controller that is already connected.");
            return;
        }

        ControllersInput.Add(controllerID, new ControllerInput());
        canPerformHorizontalDirectionalButton.Add(controllerID, true);
        canPerformVerticalDirectionalButton.Add(controllerID, true);
    }

    public void DisconnectController(EControllerID controllerID)
    {
        if (ControllersInput.ContainsKey(controllerID) == false)
        {
            Debug.LogError("Trying to disconnect a controller that is not connected.");
            return;
        }

        ControllersInput.Remove(controllerID);
        canPerformHorizontalDirectionalButton.Remove(controllerID);
        canPerformVerticalDirectionalButton.Remove(controllerID);
    }

    private void FlushControllersInput()
    {
        foreach (EControllerID controllerID in ControllersInput.Keys)
        {
            if (controllerID.ContainedIn(MaleficusConsts.NETWORK_CONTROLLERS) == true)
            {
                continue;
            }
            ControllersInput[controllerID].Flush();
        }
    }

    private void ReInitializeDirectionalMaps()
    {
        foreach(EControllerID controllerID in ControllersInput.Keys)
        {
            ControllersInput[controllerID].Flush();
        }

        foreach (EControllerID controllerID in ControllersInput.Keys)
        {
            Debug.Log("QWDQDQWDQ : " + controllerID);
            canPerformHorizontalDirectionalButton[controllerID] = true;
        }

        foreach (EControllerID controllerID in ControllersInput.Keys)
        {
            canPerformVerticalDirectionalButton[controllerID] = true;
        }
    }


    private bool IsPlayerConnected(EControllerID controllerID)
    {
        return ControllersInput.ContainsKey(controllerID);
    }
}
