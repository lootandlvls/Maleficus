using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum InputButton
{
    CONFIRM,
    CANCEL,
    CAST_SPELL_1,
    CAST_SPELL_2,
    CAST_SPELL_3,
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public enum InputAxis
{
    MOVE_X,
    MOVE_Y,
    ROTATE_X,
    ROTATE_Y
}



public class InputManager : Singleton<InputManager>
{
    /// Threshold to know what joystick value can be considered as a directional button
    private const float DIRECTIONAL_BUTTON_THRESHOLD = 0.5f;

    [SerializeField] private bool isNoConnectionDebugMode = false;

    /// mapping from controllerID to playerID
    private Dictionary<char, int> playerMapping;

    /// mapping to know if a specific player can perform a directional button when using the joystick 
    private Dictionary<int, bool> canPerformHorizontalDirectionalButton;
    private Dictionary<int, bool> canPerformVerticalDirectionalButton;

    protected override void Awake()
    {
        base.Awake();
        playerMapping = new Dictionary<char, int>();
        InitializeDirectionalMaps();

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

    private void Check_Confirm(char controllerID)
    {
        if (Input.GetButtonDown("Confirm_" + controllerID))
        {
            if ((playerMapping.ContainsKey(controllerID) == true) || (isNoConnectionDebugMode == true))
            {
                int playerID = GetPlayerID(controllerID, isNoConnectionDebugMode);
                EventManager.Instance.INPUT_InvokeButtonPressed(InputButton.CONFIRM, playerID);

            }
            else // Connect player
            {
                // In Connecting Players state ?
                if (AppStateManager.Instance.CurrentAppState == AppState.CONNECTING_PLAYERS)
                {
                    int connectedPlayerID = PlayerManager.Instance.ConnectNextPlayerToController();
                    if (connectedPlayerID != 0)
                    {
                        playerMapping[controllerID] = connectedPlayerID;
                    }
                }
            }
 
        }
    }

    private void Check_Cancel(char controllerID)
    {
        if ((playerMapping.ContainsKey(controllerID) == true) || (isNoConnectionDebugMode == true))
        {


            int playerID = GetPlayerID(controllerID, isNoConnectionDebugMode);
        }
                                                                                                                                // TODO: implement
    }
    

    private void Check_Spell(int spellID, char controllerID)
    {
        // Player already connected?
        if ((playerMapping.ContainsKey(controllerID) == true) || (isNoConnectionDebugMode == true))
        {
            // Did player press button?
            if (Input.GetButtonDown("CastSpell_" + spellID + '_' + controllerID))
            {
                int playerID = GetPlayerID(controllerID, isNoConnectionDebugMode);
                if (spellID == 1)
                {
                    EventManager.Instance.INPUT_InvokeButtonPressed(InputButton.CAST_SPELL_1, playerID);
                }
                else if (spellID == 2)
                {
                    EventManager.Instance.INPUT_InvokeButtonPressed(InputButton.CAST_SPELL_2, playerID);
                }
                else if (spellID == 3)
                {
                    EventManager.Instance.INPUT_InvokeButtonPressed(InputButton.CAST_SPELL_3, playerID);
                }
            }
        }
    }


    private void Check_Axis(string axisName, char axisSide, char controllerID)
    {
        // Player already connected?
        if ((playerMapping.ContainsKey(controllerID) == true) || (isNoConnectionDebugMode == true))
        {
            // Did player moved joystick

            float axisValue = Input.GetAxis(axisName + '_' + axisSide + '_' + controllerID);
            if (axisValue != 0.0f)
            {
                int playerID = GetPlayerID(controllerID, isNoConnectionDebugMode);

                // Is it a MOVE or ROTATE joystick?
                InputAxis inputAxis = InputAxis.MOVE_X;
                if (axisSide == 'L') 
                {
                    if (axisName == "Horizontal")
                    {
                        inputAxis = InputAxis.MOVE_X;
                    }
                    else if (axisName == "Vertical")
                    {
                        inputAxis = InputAxis.MOVE_Y;
                    }
                    
                }
                else if (axisSide == 'R') 
                {
                    if (axisName == "Horizontal")
                    {
                        inputAxis = InputAxis.ROTATE_X;
                    }
                    else if (axisName == "Vertical")
                    {
                        inputAxis = InputAxis.ROTATE_Y;
                    }
                }

                // Axis event
                EventManager.Instance.INPUT_InvokeJoystickMoved(inputAxis, axisValue, playerID);

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
                                EventManager.Instance.INPUT_InvokeButtonPressed(InputButton.RIGHT, playerID);
                            }
                            else                    // negative value
                            {
                                EventManager.Instance.INPUT_InvokeButtonPressed(InputButton.LEFT, playerID);
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
                                EventManager.Instance.INPUT_InvokeButtonPressed(InputButton.DOWN, playerID);
                            }
                            else                    // negative value
                            {
                                EventManager.Instance.INPUT_InvokeButtonPressed(InputButton.UP, playerID);
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
   

    private int GetPlayerID(char controllerID, bool debugMode)
    {
        if (debugMode == true)
        {
            return 0;
        }
        return playerMapping[controllerID];
    }

    private void InitializeDirectionalMaps()
    {
        canPerformHorizontalDirectionalButton = new Dictionary<int, bool>();
        canPerformHorizontalDirectionalButton[0] = true; // test ID
        canPerformHorizontalDirectionalButton[1] = true;
        canPerformHorizontalDirectionalButton[2] = true;
        canPerformHorizontalDirectionalButton[3] = true;
        canPerformHorizontalDirectionalButton[4] = true;

        canPerformVerticalDirectionalButton = new Dictionary<int, bool>();
        canPerformVerticalDirectionalButton[0] = true; // test ID
        canPerformVerticalDirectionalButton[1] = true;
        canPerformVerticalDirectionalButton[2] = true;
        canPerformVerticalDirectionalButton[3] = true;
        canPerformVerticalDirectionalButton[4] = true;


    }
}
