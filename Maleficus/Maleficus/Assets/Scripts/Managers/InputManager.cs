using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputManager : Singleton<InputManager>
{
    public EInputMode InputMode { get { return inputMode; } }

    /// Threshold to know what joystick value can be considered as a directional button
    private const float DIRECTIONAL_BUTTON_THRESHOLD = 0.0f;

    [SerializeField] private EInputMode inputMode;
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
        if (inputMode == EInputMode.TOUCH)
        {
            touchPlayerID = PlayerManager.Instance.ConnectNextPlayerToController();
        }
    }


    private void Update()
    {
        // Confirm
        Check_Confirm('A');
        Check_Confirm('B');
        Check_Confirm('C');
        Check_Confirm('D');

        // Cancel
        Check_Cancel('A');
        Check_Cancel('B');
        Check_Cancel('C');
        Check_Cancel('D');


        // Spell 1
        Check_Spell(1, 'A');
        Check_Spell(1, 'B');
        Check_Spell(1, 'C');
        Check_Spell(1, 'D');
        //spell 2
        Check_Spell(2, 'A');
        // spell 3
        Check_Spell(3, 'A');

        // TODO: Add missing buttons in Input settings

        // Spell 2                                                                                                                           // TODO: Add in input settings
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

    public void OnJoystickMoved(Vector2 joystickInput, ETouchJoystickType joystickType)
    {
        if (inputMode == EInputMode.TOUCH)
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
        switch(joystickType)
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

    private void Check_Confirm(char controllerID)
    {
        if (Input.GetButtonDown("Confirm_" + controllerID))
        {
            // Player already connected?
            if ((playerControllerMapping.ContainsKey(controllerID) == true) || (inputMode == EInputMode.TEST))
            {
                EPlayerID playerID = GetPlayerID(controllerID);
                EventManager.Instance.Invoke_INPUT_ButtonPressed(EInputButton.CONFIRM, playerID);
            }
            else // Connect player
            {
                // In Connecting Players state ?
                if (AppStateManager.Instance.CurrentAppState == EAppState.CONNECTING_PLAYERS)
                {
                    EPlayerID connectedPlayerID = PlayerManager.Instance.ConnectNextPlayerToController();
                    if (connectedPlayerID != EPlayerID.TEST)
                    {
                        playerControllerMapping[controllerID] = connectedPlayerID;
                    }
                }
            }
        }
    }

    private void Check_Cancel(char controllerID)
    {
        if ((playerControllerMapping.ContainsKey(controllerID) == true) || (inputMode == EInputMode.TEST))
        {

                                                                                                                                    // TODO: implement
            EPlayerID playerID = GetPlayerID(controllerID);
        }
                                                                                                                                
    }
    

    private void Check_Spell(int spellID, char controllerID)
    {
        // Player already connected?
        if ((playerControllerMapping.ContainsKey(controllerID) == true) || (inputMode == EInputMode.TEST))
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


    private void Check_Axis(string axisName, char axisSide, char controllerID)
    {
        // Player already connected?
        if ((playerControllerMapping.ContainsKey(controllerID) == true) || (inputMode == EInputMode.TEST))
        {
            // Did player move joystick
            float axisValue = Input.GetAxis(axisName + '_' + axisSide + '_' + controllerID);
         //if   (axisValue != 0.0f)                                                                                 // TODO: set back if?
            if ( Mathf.Abs(axisValue) > DIRECTIONAL_BUTTON_THRESHOLD)
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
                        if ((Mathf.Abs(axisValue) > DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformHorizontalDirectionalButton[playerID] == true))
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
                        else if ((Mathf.Abs(axisValue) < DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformHorizontalDirectionalButton[playerID] == false))
                        {
                            canPerformHorizontalDirectionalButton[playerID] = true;
                        }
                    }
                    else if (axisName == "Vertical")
                    {
                        if ((Mathf.Abs(axisValue) > DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformVerticalDirectionalButton[playerID] == true))
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
                        else if ((Mathf.Abs(axisValue) < DIRECTIONAL_BUTTON_THRESHOLD) && (canPerformVerticalDirectionalButton[playerID] == false))
                        {
                            canPerformVerticalDirectionalButton[playerID] = true;
                        }
                    }
                }
                    

                // Debug
                DebugManager.Instance.Log(2, "joystick " + axisName + " " + axisSide + " by " + controllerID + " : " + axisValue);
                
            }
        }
    }
   

    private EPlayerID GetPlayerID(char controllerID)
    {
        if (inputMode == EInputMode.TEST)
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
}
