﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputManager : AbstractSingletonManager<InputManager>
{
    public EInputMode InputMode { get { return MotherOfManagers.Instance.InputMode; } }


    private EPlayerID touchPlayerID;
    /// mapping from controllerID to playerID
    private Dictionary<char, EPlayerID> playerControllerMapping;
    /// mapping to know if a specific player can perform a directional button when using the joystick 
    private Dictionary<EPlayerID, bool> canPerformHorizontalDirectionalButton;
    private Dictionary<EPlayerID, bool> canPerformVerticalDirectionalButton;

    protected override void Awake()
    {
        base.Awake();
        playerControllerMapping = new Dictionary<char, EPlayerID>();
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
        
    }

    private void Update()
    {
        // Confirm
        Check_Confirm('A');
        Check_Confirm('B');
        Check_Confirm('C');
        Check_Confirm('D');

                                                        // TODO: Add missing Cancel buttons in Input Setting
        // Cancel
        //Check_Cancel('A');
        //Check_Cancel('B');
        //Check_Cancel('C');
        //Check_Cancel('D');

        /* Charging spell check */
        // Spell 1
        Check_ChargingSpell(1, 'A');
        Check_ChargingSpell(1, 'B');
        Check_ChargingSpell(1, 'C');
        Check_ChargingSpell(1, 'D');
        //spell 2
        Check_ChargingSpell(2, 'A');
        Check_ChargingSpell(2, 'B');
        Check_ChargingSpell(2, 'C');
        Check_ChargingSpell(2, 'D');
        // spell 3
        Check_ChargingSpell(3, 'A');
        Check_ChargingSpell(3, 'B');
        Check_ChargingSpell(3, 'C');
        Check_ChargingSpell(3, 'D');

        /* Casted spell check */
        // Spell 1
        Check_CastedSpell(1, 'A');
        Check_CastedSpell(1, 'B');
        Check_CastedSpell(1, 'C');
        Check_CastedSpell(1, 'D');
        //spell 2
        Check_CastedSpell(2, 'A');
        Check_CastedSpell(2, 'B');
        Check_CastedSpell(2, 'C');
        Check_CastedSpell(2, 'D');
        // spell 3
        Check_CastedSpell(3, 'A');
        Check_CastedSpell(3, 'B');        // TODO: Add missing Spell buttons in Input settings
        Check_CastedSpell(3, 'C');
        Check_CastedSpell(3, 'D');
        // Spell 2                                                                                                                   
        //CheckAndCallSpell(2, 'A');
        //CheckAndCallSpell(2, 'B');
        //CheckAndCallSpell(2, 'C');
        //CheckAndCallSpell(2, 'D');

        // Spell 3
        //CheckAndCallSpell(3, 'A');
        //CheckAndCallSpell(3, 'B');
        //CheckAndCallSpell(3, 'C');
        //CheckAndCallSpell(3, 'D');

        // Horizontal 
        // Left 
        Check_Axis("Horizontal", 'L', 'A');
        Check_Axis("Horizontal", 'L', 'B');
        Check_Axis("Horizontal", 'L', 'C');
        Check_Axis("Horizontal", 'L', 'D');

        // Right
        Check_Axis("Horizontal", 'R', 'A');
        Check_Axis("Horizontal", 'R', 'B');
        Check_Axis("Horizontal", 'R', 'C');
        Check_Axis("Horizontal", 'R', 'D');

        // Vertical 
        // Left 
        Check_Axis("Vertical", 'L', 'A');
        Check_Axis("Vertical", 'L', 'B');
        Check_Axis("Vertical", 'L', 'C');
        Check_Axis("Vertical", 'L', 'D');

        // Right
        Check_Axis("Vertical", 'R', 'A');
        Check_Axis("Vertical", 'R', 'B');
        Check_Axis("Vertical", 'R', 'C');
        Check_Axis("Vertical", 'R', 'D');

    }

    public void OnJoystickPressed(ETouchJoystickType joystickType)
    {
        if (InputMode == EInputMode.TOUCH)
        {
            switch (joystickType)
            {
                case ETouchJoystickType.SPELL_1:
                    EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.CAST_SPELL_1, touchPlayerID);
                    break;

                case ETouchJoystickType.SPELL_2:
                    EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.CAST_SPELL_2, touchPlayerID);
                    break;

                case ETouchJoystickType.SPELL_3:
                    EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.CAST_SPELL_3, touchPlayerID);
                    break;
            }
        }
    }

    public void OnJoystickMoved(Vector2 joystickInput, ETouchJoystickType joystickType)
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



    public void OnJoystickReleased(ETouchJoystickType joystickType)
    {
        if (InputMode == EInputMode.TOUCH)
        {
            switch (joystickType)
            {
                case ETouchJoystickType.SPELL_1:
                    EventManager.Instance.Invoke_INPUT_ButtonReleased(EInputButton.CAST_SPELL_1, touchPlayerID);
                    break;

                case ETouchJoystickType.SPELL_2:
                    EventManager.Instance.Invoke_INPUT_ButtonReleased(EInputButton.CAST_SPELL_2, touchPlayerID);
                    break;

                case ETouchJoystickType.SPELL_3:
                    EventManager.Instance.Invoke_INPUT_ButtonReleased(EInputButton.CAST_SPELL_3, touchPlayerID);
                    break;
            }
        }
    }

    private void Check_Confirm(char controllerID)
    {
        if (Input.GetButtonDown("Confirm_" + controllerID))
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

    private void Check_Cancel(char controllerID)
    {
                                                                                                        // TODO: Test with controller if it works
        if (Input.GetButtonDown("Cancel_" + controllerID))
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
    

    private void Check_ChargingSpell(int spellID, char controllerID)
    {
        if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
        {
            // Did player press button?
            if (Input.GetButtonDown("CastSpell_" + spellID + '_' + controllerID))
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

    private void Check_CastedSpell(int spellID, char controllerID)
    {
        if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
        {
            // Did player press button?
            if (Input.GetButtonUp("CastSpell_" + spellID + '_' + controllerID))
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


    private void Check_Axis(string axisName, char axisSide, char controllerID)
    {
        if ((IsPlayerConnected(controllerID) == true) || (InputMode == EInputMode.TEST))
        {
            // Did player move joystick
            float axisValue = Input.GetAxis(axisName + '_' + axisSide + '_' + controllerID);
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
                        if ((Mathf.Abs(axisValue) > MaleficusTypes.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformHorizontalDirectionalButton[playerID] == true))
                        {
                            canPerformHorizontalDirectionalButton[playerID] = false;
                            // Which direction?
                            if (axisValue > 0.0f)   // positive value
                            {
                                EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.RIGHT, playerID);
                            }
                            else                    // negative value
                            {
                                EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.LEFT, playerID);
                            }
                        }
                        else if ((Mathf.Abs(axisValue) < MaleficusTypes.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformHorizontalDirectionalButton[playerID] == false))
                        {
                            canPerformHorizontalDirectionalButton[playerID] = true;
                        }
                    }
                    else if (axisName == "Vertical")
                    {
                        if ((Mathf.Abs(axisValue) > MaleficusTypes.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformVerticalDirectionalButton[playerID] == true))
                        {
                            canPerformVerticalDirectionalButton[playerID] = false;
                            // Which direction?
                            if (axisValue > 0.0f)   // positive value
                            {
                                EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.DOWN, playerID);
                            }
                            else                    // negative value
                            {
                                EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.UP, playerID);
                            }
                        }
                        else if ((Mathf.Abs(axisValue) < MaleficusTypes.DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformVerticalDirectionalButton[playerID] == false))
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
   

    private EPlayerID GetPlayerID(char controllerID)
    {
        if (InputMode == EInputMode.TEST)
        {
            return EPlayerID.TEST;
        }
        return playerControllerMapping[controllerID];
    }

    private void InitializeDirectionalMaps()
    {
        canPerformHorizontalDirectionalButton = new Dictionary<EPlayerID, bool>();
        canPerformHorizontalDirectionalButton[EPlayerID.TEST] = true; // test ID
        canPerformHorizontalDirectionalButton[EPlayerID.PLAYER_1] = true;
        canPerformHorizontalDirectionalButton[EPlayerID.PLAYER_2] = true;
        canPerformHorizontalDirectionalButton[EPlayerID.PLAYER_3] = true;
        canPerformHorizontalDirectionalButton[EPlayerID.PLAYER_4] = true;

        canPerformVerticalDirectionalButton = new Dictionary<EPlayerID, bool>();
        canPerformVerticalDirectionalButton[EPlayerID.TEST] = true; // test ID
        canPerformVerticalDirectionalButton[EPlayerID.PLAYER_1] = true;
        canPerformVerticalDirectionalButton[EPlayerID.PLAYER_2] = true;
        canPerformVerticalDirectionalButton[EPlayerID.PLAYER_3] = true;
        canPerformVerticalDirectionalButton[EPlayerID.PLAYER_4] = true;


    }


    private bool IsPlayerConnected(char controllerID)
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
                    List<NetMsg> msgs = NetworkManager.Instance.allReceivedMsgs;
                    for(int i = msgs.Count - 1; i > -1; i--)
                    {
                        if(msgs[i].OP == 15){
                            Net_SpellInput si = (Net_SpellInput)msgs[i];
                            int spellid = 0;
                            char controllerid = 'Z';

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
                                    controllerid = 'A';
                                    break;
                                case EPlayerID.PLAYER_2:
                                    controllerid = 'B';
                                    break;
                                case EPlayerID.PLAYER_3:
                                    controllerid = 'C';
                                    break;
                                case EPlayerID.PLAYER_4:
                                    controllerid = 'D';
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
