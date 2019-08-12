using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputManager : AbstractSingletonManager<InputManager>
{
    public EInputMode InputMode { get { return MotherOfManagers.Instance.InputMode; } }


    private EPlayerID touchPlayerID;

    /// <summary> Mapping from controllerID to playerID </summary> 
    private Dictionary<EControllerID, EPlayerID>    playerControllerMapping                 = new Dictionary<EControllerID, EPlayerID>();

    /// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    private Dictionary<EPlayerID, bool>             canPerformHorizontalDirectionalButton   = new Dictionary<EPlayerID, bool>();

    /// <summary> Mapping to know if a specific player can perform a directional button when using the joystick  </summary>
    private Dictionary<EPlayerID, bool>             canPerformVerticalDirectionalButton     = new Dictionary<EPlayerID, bool>();

    protected override void Awake()
    {
        base.Awake();

        InitializeDirectionalMaps();
    }

    private void Start()
    {
        if (InputMode == EInputMode.TOUCH)
        {
            touchPlayerID = PlayerManager.Instance.ConnectNextPlayerToController();
        }
        EventManager.Instance.NETWORK_ReceivedMessageUpdated += On_NETWORK_ReceivedMessageUpdated;
    }

    public override void Initialize()
    {
        canPerformHorizontalDirectionalButton.Clear();
        canPerformVerticalDirectionalButton.Clear();
    }

    private void Update()
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
        Check_ChargingSpell(1, EControllerID.CONTROLLER_A);
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
        Check_CastedSpell(3, EControllerID.CONTROLLER_B);        // TODO: Add missing Spell buttons in Input settings
        Check_CastedSpell(3, EControllerID.CONTROLLER_C);
        Check_CastedSpell(3, EControllerID.CONTROLLER_D);
        // Spell 2                                                                                                                   
        //CheckAndCallSpell(2, EControllerID.CONTROLLER_A);
        //CheckAndCallSpell(2, EControllerID.CONTROLLER_B);
        //CheckAndCallSpell(2, EControllerID.CONTROLLER_C);
        //CheckAndCallSpell(2, EControllerID.CONTROLLER_D);

        // Spell 3
        //CheckAndCallSpell(3, EControllerID.CONTROLLER_A);
        //CheckAndCallSpell(3, EControllerID.CONTROLLER_B);
        //CheckAndCallSpell(3, EControllerID.CONTROLLER_C);
        //CheckAndCallSpell(3, EControllerID.CONTROLLER_D);

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


    #region Controller
    private void Check_Confirm(EControllerID controllerID)
    {
        char controllerIDName = MaleficusUtilities.ControllerIDToChar(controllerID);
        if (Input.GetButtonDown("Confirm_" + controllerIDName))
        {


            if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
            {
                Debug.Log("Confrim " + controllerID);
                // Send Input
                EPlayerID playerID = GetPlayerID(controllerID);
                EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.CONFIRM, playerID);
            }
            else //if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_CONNECTING_PLAYERS)
                // Connect players
            {
                EPlayerID connectedPlayerID = PlayerManager.Instance.ConnectNextPlayerToController();
                if (connectedPlayerID != EPlayerID.TEST)
                {
                    Debug.Log("Confrim " + controllerID);
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
                    EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.CANCEL, playerID);
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
                if (spellID == 1)
                {
                    EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.CAST_SPELL_1, playerID);                
                }
                else if (spellID == 2)
                {
                    EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.CAST_SPELL_2, playerID);
                }
                else if (spellID == 3)
                {
                    EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.CAST_SPELL_3, playerID);
                }
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
                if (spellID == 1)
                {
                    EventManager.Instance.Invoke_INPUT_ButtonReleased(EInputButton.CAST_SPELL_1, playerID);                
                }
                else if (spellID == 2)
                {
                    EventManager.Instance.Invoke_INPUT_ButtonReleased(EInputButton.CAST_SPELL_2, playerID);
                }
                else if (spellID == 3)
                {
                    EventManager.Instance.Invoke_INPUT_ButtonReleased(EInputButton.CAST_SPELL_3, playerID);
                }
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
                EventManager.Instance.Invoke_INPUT_JoystickMoved(inputAxis, axisValue, playerID);


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
                                EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.RIGHT, playerID);
                            }
                            else                    
                                // negative value
                            {
                                EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.LEFT, playerID);
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
                                EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.DOWN, playerID);
                            }
                            else                    
                                // negative value
                            {
                                EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.UP, playerID);
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
                EventManager.Instance.Invoke_INPUT_ButtonPressed(inputButton, touchPlayerID);
            }
        }
    }

    public void OnTouchJoystickMoved(Vector2 joystickInput, ETouchJoystickType joystickType)
    {
        if (InputMode == EInputMode.TOUCH)
        {
            if (joystickType == ETouchJoystickType.MOVE)
            {
                EventManager.Instance.Invoke_INPUT_JoystickMoved(EInputAxis.MOVE_X, joystickInput.x, touchPlayerID);
                EventManager.Instance.Invoke_INPUT_JoystickMoved(EInputAxis.MOVE_Y, joystickInput.y, touchPlayerID);
            }
            else // Spell joystick
            {
                EventManager.Instance.Invoke_INPUT_JoystickMoved(EInputAxis.ROTATE_X, joystickInput.x, touchPlayerID);
                EventManager.Instance.Invoke_INPUT_JoystickMoved(EInputAxis.ROTATE_Y, -joystickInput.y, touchPlayerID);
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
                EventManager.Instance.Invoke_INPUT_ButtonReleased(inputButton, touchPlayerID);
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

    #region Listeners
    private void On_NETWORK_ReceivedMessageUpdated(ENetworkMessage receivedMsg)
    {
        if(AppStateManager.Instance.CurrentScene == EScene.GAME)
        {
            switch (receivedMsg)
            {
                case ENetworkMessage.DATA_SPELLINPUT:
                    List<AbstractNetMessage> msgs = NetworkManager.Instance.allReceivedMsgs;
                    for(int i = msgs.Count - 1; i > -1; i--)
                    {
                        if(msgs[i].ID == 15)                                    // TODO [Leon]:  use network ID (NetID.SpellInput)
                        {              
                            Net_SpellInput si = (Net_SpellInput)msgs[i];
                            int spellid = 0;
                            EControllerID controllerid = EControllerID.NONE;

                            switch(si.spellId)
                            {
                                case EInputButton.CAST_SPELL_1:
                                    spellid = 1;
                                    break;
                                case EInputButton.CAST_SPELL_2:
                                    spellid = 2;
                                    break;
                                case EInputButton.CAST_SPELL_3:
                                    spellid = 3;
                                    break;
                            }

                            switch (si.ePlayerID)
                            {
                                case EPlayerID.PLAYER_1:
                                    controllerid = EControllerID.CONTROLLER_A;
                                    break;
                                case EPlayerID.PLAYER_2:
                                    controllerid = EControllerID.CONTROLLER_B;
                                    break;
                                case EPlayerID.PLAYER_3:
                                    controllerid = EControllerID.CONTROLLER_C;
                                    break;
                                case EPlayerID.PLAYER_4:
                                    controllerid = EControllerID.CONTROLLER_D;
                                    break;
                            }

                            Check_ChargingSpell(spellid, controllerid);
                        }
                    }
                    break;
            }
        }
    }
    #endregion
}
