using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : Singleton<EventManager>
{


    #region GAME
    public event Action<AppState> GAME_AppStateUpdated;
    public void Invoke_GAME_AppStateUpdated(AppState newAppState)
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
    public event Action<ISpell, PlayerID> SPELLS_SpellSpawned;
    public void Invoke_SPELLS_SpellSpawned(ISpell castedSpell, PlayerID castingPlayerID)
    {

        if (SPELLS_SpellSpawned != null)
        {
            SPELLS_SpellSpawned.Invoke(castedSpell, castingPlayerID);
        }
    }

    public event Action<HitInfo> SPELLS_SpellHitPlayer;
    public void Invoke_SPELLS_SpellHitPlayer(HitInfo hitInfo)
    {

        if (SPELLS_SpellHitPlayer != null)
        {
            SPELLS_SpellHitPlayer.Invoke(hitInfo);
        }
    }
    #endregion




    #region UI
    public event Action<MenuState, MenuState> UI_MenuStateUpdated;
    public void Invoke_UI_MenuStateUpdated(MenuState newState, MenuState lastState)
    {
        if (UI_MenuStateUpdated != null)
        {
            UI_MenuStateUpdated.Invoke(newState, lastState);
        }
    }
    #endregion

    #region INPUT
    public event Action<InputButton, PlayerID> INPUT_ButtonPressed;
    public void Invoke_INPUT_ButtonPressed(InputButton buttonType, PlayerID playerID)
    {
        if (INPUT_ButtonPressed != null)
        {
            INPUT_ButtonPressed.Invoke(buttonType, playerID);
        }
    }

    public event Action<InputAxis, float, PlayerID> INPUT_JoystickMoved;
    public void Invoke_INPUT_JoystickMoved(InputAxis axisType, float axisValue, PlayerID playerID)
    {
        if (INPUT_JoystickMoved != null)
        {
            INPUT_JoystickMoved.Invoke(axisType, axisValue, playerID);
        }
    }
    #endregion
}
