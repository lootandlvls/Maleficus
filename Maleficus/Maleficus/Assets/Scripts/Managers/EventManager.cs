using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : Singleton<EventManager>
{
    [SerializeField] private bool isDebugLogEvents = false;

    #region GAME
    public event Action<EAppState, EAppState> GAME_AppStateUpdated;
    public void Invoke_GAME_AppStateUpdated(EAppState newAppState, EAppState lastState)
    {
        if (GAME_AppStateUpdated != null)
        {
            GAME_AppStateUpdated.Invoke(newAppState, lastState);
        }
        DebugLog("App state changed from " + lastState + " to " + newAppState);
    }
    #endregion

    #region PLAYERS
    public event Action<EPlayerID> PLAYERS_PlayerConnected;
    public void Invoke_PLAYERS_PlayerConnected(EPlayerID playerID)
    {
        if (PLAYERS_PlayerConnected != null)
        {
            PLAYERS_PlayerConnected.Invoke(playerID);
        }
        DebugLog(playerID + " connected");
    }

    public event Action<EPlayerID> PLAYERS_PlayerDisconnected;
    public void Invoke_PLAYERS_PlayerDisconnected(EPlayerID playerID)
    {
        if (PLAYERS_PlayerDisconnected != null)
        {
            PLAYERS_PlayerDisconnected.Invoke(playerID);
        }
        DebugLog(playerID + " disconnected");
    }

    public event Action<EPlayerID> PLAYERS_PlayerSpawned;
    public void Invoke_PLAYERS_PlayerSpawned(EPlayerID playerID)
    {
        if (PLAYERS_PlayerSpawned != null)
        {
            PLAYERS_PlayerSpawned.Invoke(playerID);
        }
        DebugLog(playerID + " spawned");
    }

    public event Action<EPlayerID> PLAYERS_PlayerDied;
    public void Invoke_PLAYERS_PlayerDied(EPlayerID playerID)
    {
        if (PLAYERS_PlayerDied != null)
        {
            PLAYERS_PlayerDied.Invoke(playerID);
        }
        DebugLog(playerID + " died");
    }
    #endregion

    #region SPELLS
    public event Action<ISpell, EPlayerID> SPELLS_SpellSpawned;
    public void Invoke_SPELLS_SpellSpawned(ISpell castedSpell, EPlayerID castingPlayerID)
    {

        if (SPELLS_SpellSpawned != null)
        {
            SPELLS_SpellSpawned.Invoke(castedSpell, castingPlayerID);
        }
        DebugLog(castedSpell + " casted by " + castingPlayerID);
    }

    public event Action<HitInfo> SPELLS_SpellHitPlayer;
    public void Invoke_SPELLS_SpellHitPlayer(HitInfo hitInfo)
    {

        if (SPELLS_SpellHitPlayer != null)
        {
            SPELLS_SpellHitPlayer.Invoke(hitInfo);
        }
        Debug.Log(hitInfo.CastedSpell.SpellName + " from player " + hitInfo.CastingPlayerID + " hit player " + hitInfo.HitPlayerID);
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
        DebugLog("Menu state changed from " + lastState + " to " + newState);
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
        DebugLog(buttonType + " pressed by " + playerID);
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

    private void DebugLog(string messageLog)
    {
        if (isDebugLogEvents == true)
        {
            Debug.Log(messageLog);
        }
    }
}
