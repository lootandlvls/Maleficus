using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : Singleton<EventManager>
{


    #region GAME
    public event Action<AppState> GAME_AppStateUpdated;
    public void GAME_InvokeAppStateUpdated(AppState newAppState)
    {
        if (GAME_AppStateUpdated != null)
        {
            GAME_AppStateUpdated.Invoke(newAppState);
        }
    }
    #endregion

    #region PLAYERS

    #endregion

    #region SPELLS

    #endregion




    #region UI
    public event Action<MenuState, MenuState> UI_MenuStateUpdated;
    public void UI_InvokeMenuStateUpdated(MenuState newState, MenuState lastState)
    {
        if (UI_MenuStateUpdated != null)
        {
            UI_MenuStateUpdated.Invoke(newState, lastState);
        }
    }
    #endregion

    #region INPUT
    public event Action<InputButton, int> INPUT_ButtonPressed;
    public void INPUT_InvokeButtonPressed(InputButton buttonType, int playerID)
    {
        if (INPUT_ButtonPressed != null)
        {
            INPUT_ButtonPressed.Invoke(buttonType, playerID);
        }
    }

    public event Action<InputAxis, float, int> INPUT_JoystickMoved;
    public void INPUT_InvokeJoystickMoved(InputAxis axisType, float axisValue, int playerID)
    {
        if (INPUT_JoystickMoved != null)
        {
            INPUT_JoystickMoved.Invoke(axisType, axisValue, playerID);
        }
    }
    #endregion
}
