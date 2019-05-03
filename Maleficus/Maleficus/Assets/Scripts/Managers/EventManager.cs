using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : Singleton<EventManager>
{




    #region Players

    #endregion

    #region Spells

    #endregion

    #region UI
    public event Action<MenuState, MenuState> MenuStateUpdated;
    public void InvokeMenuStateUpdated(MenuState newState, MenuState lastState)
    {
        if (MenuStateUpdated != null)
        {
            MenuStateUpdated.Invoke(newState, lastState);
        }
    }
    #endregion
}
