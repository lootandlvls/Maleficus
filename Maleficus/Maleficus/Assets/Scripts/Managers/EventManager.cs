using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : Singleton<EventManager>
{


    #region GAME
    public event Action<EAppState> GAME_AppStateUpdated;
    public void Invoke_GAME_AppStateUpdated(EAppState newAppState)
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
    public event Action<ISpell, EPlayerID> SPELLS_SpellSpawned;
    public void Invoke_SPELLS_SpellSpawned(ISpell castedSpell, EPlayerID castingPlayerID)
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
    public event Action<EMenuState, EMenuState> UI_MenuStateUpdated;
    public void Invoke_UI_MenuStateUpdated(EMenuState newState, EMenuState lastState)
    {
        if (UI_MenuStateUpdated != null)
        {
            UI_MenuStateUpdated.Invoke(newState, lastState);
        }
    }
    #endregion

    #region INPUT
    public event Action<EInputButton, EPlayerID> INPUT_ButtonPressed;
    public void Invoke_INPUT_ButtonPressed(EInputButton buttonType, EPlayerID playerID)
    {
        if (INPUT_ButtonPressed != null)
        {
            INPUT_ButtonPressed.Invoke(buttonType, playerID);
        }
    }

    public event Action<EInputAxis, float, EPlayerID> INPUT_JoystickMoved;
    public void Invoke_INPUT_JoystickMoved(EInputAxis axisType, float axisValue, EPlayerID playerID)
    {
        if (INPUT_JoystickMoved != null)
        {
            INPUT_JoystickMoved.Invoke(axisType, axisValue, playerID);
        }
    }
    #endregion
}
