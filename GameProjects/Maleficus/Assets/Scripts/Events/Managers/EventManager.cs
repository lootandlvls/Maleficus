using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class EventManager : AbstractSingletonManager<EventManager>
{
    public MaleficusEvent<TestEventHandle> TEST_TestEvent 
        = new MaleficusEvent<TestEventHandle>("TEST_TestEvent");

    #region APP

    public MaleficusEvent<Event_StateUpdated<EAppState>> APP_AppStateUpdated 
        = new MaleficusEvent<Event_StateUpdated<EAppState>>("APP_AppStateUpdated");

    public MaleficusEvent<Event_GenericHandle<EScene>> APP_SceneWillChange 
        = new MaleficusEvent<Event_GenericHandle<EScene>>("APP_SceneWillChange");

    public MaleficusEvent<Event_GenericHandle<EScene>> APP_SceneChanged 
        = new MaleficusEvent<Event_GenericHandle<EScene>>("APP_SceneChanged");


    public event Action GAME_CountdownFinished;
    public void Invoke_GAME_CountdownFinished()
    {
        if (GAME_CountdownFinished != null)
        {
            GAME_CountdownFinished.Invoke();
        }
        DebugLog("Countdown finished");
    }
    #endregion

    #region GAME
    public event Action<bool> GAME_IntroFinished;
    public void Invoke_GAME_IntroFinished(bool isReady)
    {
        if (GAME_IntroFinished != null)
        {
            GAME_IntroFinished.Invoke(isReady);
        }
        
    }


    public event Action<AbstractGameMode> GAME_GameStarted;
    public void Invoke_GAME_GameStarted(AbstractGameMode gameMode)
    {
        if (GAME_GameStarted != null)
        {
            GAME_GameStarted.Invoke(gameMode);
        }
        DebugLog("Game started : " + gameMode.GameModeType);
    }


    public event Action<AbstractGameMode> GAME_GamePaused;
    public void Invoke_GAME_GamePaused(AbstractGameMode gameMode)
    {
        if (GAME_GamePaused != null)
        {
            GAME_GamePaused.Invoke(gameMode);
        }
        DebugLog("Game paused : " + gameMode.GameModeType);
    }

    public event Action<AbstractGameMode> GAME_GameUnPaused;
    public void Invoke_GAME_GameUnPaused(AbstractGameMode gameMode)
    {
        if (GAME_GameUnPaused != null)
        {
            GAME_GameUnPaused.Invoke(gameMode);
        }
        DebugLog("Game unpaused : " + gameMode.GameModeType);
    }

    public event Action<AbstractGameMode, bool> GAME_GameEnded;
    public void Invoke_GAME_GameEnded(AbstractGameMode gameMode, bool wasAborted = false)
    {
        if (GAME_GameEnded != null)
        {
            GAME_GameEnded.Invoke(gameMode, wasAborted);
        }
        DebugLog("Game ended : " + gameMode.GameModeType + ". Aborted : " + wasAborted.ToString());
    }

    public event Action<AbstractPlayerStats, EGameMode> GAME_PlayerStatsUpdated;
    public void Invoke_GAME_PlayerStatsUpdated(AbstractPlayerStats updatedPlayerStats, EGameMode fromGameMode)
    {
        if (GAME_PlayerStatsUpdated != null)
        {
            GAME_PlayerStatsUpdated.Invoke(updatedPlayerStats, fromGameMode);
        }
       // DebugLog("Player stats " + updatedPlayerStats.ToString() + " updated for " + fromGameMode);
    }

    public event Action<AbstractGameMode> GAME_SetPlayersScores;
    public void Invoke_GAME_SetPlayersScores(AbstractGameMode gameMode)
    {
        if (GAME_SetPlayersScores != null)
        {
            GAME_SetPlayersScores.Invoke(gameMode);
        }
        DebugLog("Set Players Scores");
    }

    public event Action<int> GAME_GameTimeUpdated;
    public void Invoke_GAME_GameTimeUpdated(int time)
    {
         if (GAME_GameTimeUpdated != null)
        {
            GAME_GameTimeUpdated.Invoke(time);
        }
       // DebugLog("Time has been Updated to : " + time);
    }
    public MaleficusEvent<NetEvent_GameOver> GAME_GameOver 
        = new MaleficusEvent<NetEvent_GameOver>("Game_GameOver");

    #endregion

    #region PLAYERS

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


    public event Action PLAYERS_PlayerCollectedCoin;
    public void Invoke_PLAYERS_PlayerCollectedCoin()
    {
        if (PLAYERS_PlayerCollectedCoin != null)
        {
            PLAYERS_PlayerCollectedCoin.Invoke();
        }
        DebugLog("Player collected a coin");
    }
        
    public event Action<EPlayerID> PLAYERS_PlayerJoined;
    public void Invoke_PLAYERS_PlayerJoined(EPlayerID playerID)
    {
        if (PLAYERS_PlayerJoined != null)
        {
            PLAYERS_PlayerJoined.Invoke(playerID);
        }
        DebugLog("Player " + playerID + " joined");
    }
        
    public event Action<EPlayerID> PLAYERS_PlayerLeft;
    public void Invoke_PLAYERS_PlayerLeft(EPlayerID playerID)
    {
        if (PLAYERS_PlayerLeft != null)
        {
            PLAYERS_PlayerLeft.Invoke(playerID);
        }
        DebugLog("Player " + playerID + " left");
    }

    public event Action PLAYERS_AllPlayersReady;
    public void Invoke_PLAYERS_AllPlayersReady()
    {
        if (PLAYERS_AllPlayersReady != null)
        {
            PLAYERS_AllPlayersReady.Invoke();
        }
        DebugLog("All players are ready");
    }

    public event Action<EPlayerID> PLAYERS_PlayerReady;
    public void Invoke_PLAYERS_PlayerReady(EPlayerID playerID)
    {
        if (PLAYERS_PlayerReady != null)
        {
            PLAYERS_PlayerReady.Invoke(playerID);
        }
        DebugLog("Player " + playerID + " ready");
    }

    public event Action<EPlayerID> PLAYERS_PlayerCanceledReady;
    public void Invoke_PLAYERS_PLAYERS_PlayerCanceledReady(EPlayerID playerID)
    {
        if (PLAYERS_PlayerCanceledReady != null)
        {
            PLAYERS_PlayerCanceledReady.Invoke(playerID);
        }
        DebugLog("Player " + playerID + " canceled ready");
    }



    #endregion

    #region SPELLS
    
    public event Action<ISpell, EPlayerID, ESpellSlot> SPELLS_SpellSpawned;
    public void Invoke_SPELLS_SpellSpawned(ISpell castedSpell, EPlayerID castingPlayerID , ESpellSlot spellSlot)
    {

        if (SPELLS_SpellSpawned != null)
        {
            SPELLS_SpellSpawned.Invoke(castedSpell, castingPlayerID, spellSlot);
        }
        DebugLog(castedSpell + " casted by " + castingPlayerID);
    }


    public event Action<ESpellID, EPlayerID> SPELLS_UniqueEffectActivated;
    public void Invoke_SPELLS_UniqueEffectActivated(ESpellID castedSpell, EPlayerID castingPlayerID)
    {

        if (SPELLS_UniqueEffectActivated != null)
        {
            SPELLS_UniqueEffectActivated.Invoke(castedSpell, castingPlayerID);
        }
        DebugLog(castedSpell + " casted by " + castingPlayerID);
    }


    public event Action<SHitInfo> SPELLS_SpellHitPlayer;
    public void Invoke_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {

        if (SPELLS_SpellHitPlayer != null)
        {
            SPELLS_SpellHitPlayer.Invoke(hitInfo);
        }
        DebugLog(hitInfo.CastedSpell.SpellName + " from player " + hitInfo.CastingPlayerID + " hit player " + hitInfo.HitPlayerID);
    }

    public event Action<IEnemy> SPELLS_SpellHitEnemy;
    public void Invoke_SPELLS_SpellHitEnemy(IEnemy hitEnemy)
    {

        if (SPELLS_SpellHitEnemy != null)
        {
            SPELLS_SpellHitEnemy.Invoke(hitEnemy);
        }
        Debug.Log("Payer hit enemy " + hitEnemy.ToString());
    }



    //USED IN SPELL MANAGER TO TELEPORT THE PLAYER
    public event Action<float,EPlayerID> SPELLS_Teleport;
    public void Invoke_SPELLS_Teleport( float distance , EPlayerID castingPlayerID)
    {
        DebugLog("Invoke spell event");
        if (SPELLS_Teleport != null)
        {
            
            SPELLS_Teleport.Invoke( distance ,castingPlayerID);
        }
    }


    #endregion

    #region UI
    //public event Action<EMenuState, EMenuState> UI_MenuStateUpdated_old;
    //public void Invoke_UI_MenuStateUpdated(EMenuState newState, EMenuState lastState)
    //{
    //    if (UI_MenuStateUpdated_old != null)
    //    {
    //        UI_MenuStateUpdated_old.Invoke(newState, lastState);
    //    }
    //    DebugLog("Menu state changed from " + lastState + " to " + newState);
    //}
    public MaleficusEvent<Event_StateUpdated<EMenuState>> UI_MenuStateUpdated = new MaleficusEvent<Event_StateUpdated<EMenuState>>("UI_MenuStateUpdated");

    public event Action<EPlayerID, AbstractSpell , ESpellSlot> UI_SpellChosen;
    public void Invoke_UI_SpellChosen(EPlayerID playerID, AbstractSpell abstractSpell , ESpellSlot spellSlot )
    {

        if (UI_SpellChosen != null)
        {
            UI_SpellChosen.Invoke(playerID,abstractSpell, spellSlot);
        }
        DebugLog("Spell " + abstractSpell.SpellName + " Has been Chosen");
    }
    public event Action<EPlayerID,AbstractSpell> UI_SpellHighlighted;
    public void Invoke_UI_SpellHighlighted(EPlayerID playerID, AbstractSpell abstractSpell)
    {

        if (UI_SpellHighlighted != null)
        {
            UI_SpellHighlighted.Invoke(playerID, abstractSpell);
        }
        DebugLog("Spell " + abstractSpell.SpellName + " Has been Highlighted");
    }

    public event Action<EPlayerID, ESpellSlot> UI_SpellRemoved;
    public void Invoke_UI_SpellRemoved(EPlayerID playerID,ESpellSlot spellSlot)
    {

        if (UI_SpellRemoved != null)
        {
            UI_SpellRemoved.Invoke(playerID, spellSlot);
        }
        DebugLog("Spell Has been REMOVED");
    }
    #endregion

    #region INPUT

    public MaleficusEvent<Event_GenericHandle<EControllerID, EPlayerID>> INPUT_ControllerConnected
        = new MaleficusEvent<Event_GenericHandle<EControllerID, EPlayerID>>("INPUT_ControllerConnected");

    public MaleficusEvent<Event_GenericHandle<EControllerID, EPlayerID>> INPUT_ControllerDisconnected
        = new MaleficusEvent<Event_GenericHandle<EControllerID, EPlayerID>>("INPUT_ControllerDisconnected");

    public MaleficusEvent<NetEvent_ButtonPressed> INPUT_ButtonPressed 
        = new MaleficusEvent<NetEvent_ButtonPressed>("INPUT_ButtonPressed");

    public MaleficusEvent<NetEvent_ButtonReleased> INPUT_ButtonReleased 
        = new MaleficusEvent<NetEvent_ButtonReleased>("INPUT_ButtonReleased");

    public MaleficusEvent<NetEvent_JoystickMoved> INPUT_JoystickMoved 
        = new MaleficusEvent<NetEvent_JoystickMoved>("INPUT_JoystickMoved");


    #endregion

    #region NETWORK
    public event Action<ENetworkMessageType> NETWORK_ReceivedMessageUpdated;
    public void Invoke_NETWORK_ReceivedMessageUpdated(ENetworkMessageType receivedMsg)
    {
        if (NETWORK_ReceivedMessageUpdated != null)
        {
            NETWORK_ReceivedMessageUpdated.Invoke(receivedMsg);
        }
        DebugLog("Client received Message: " + receivedMsg);
    }

    public MaleficusEvent<Event_GenericHandle<List<EPlayerID>, EPlayerID>> NETWORK_ReceivedGameSessionInfo 
        = new MaleficusEvent<Event_GenericHandle<List<EPlayerID>, EPlayerID>>("NETWORK_ReceivedGameSessionInfo");

    /// <summary> Event to restore the game state in case of lost connection </summary>
    public MaleficusEvent<NetEvent_GameStateReplication> NETWORK_GameStateReplication 
        = new MaleficusEvent<NetEvent_GameStateReplication>("NETWORK_GameStateReplication") ;

    public MaleficusEvent<NetEvent_GameStarted> NETWORK_GameStarted 
        = new MaleficusEvent<NetEvent_GameStarted>("NETWORK_GameStarted");

    #endregion

    #region ENEMIES
    public event Action<IEnemy> ENEMIES_EnemyHitPlayer;                                        
    public void Invoke_ENEMIES_EnemyHitPlayer(IEnemy attackingEnemy)
    {
        if (ENEMIES_EnemyHitPlayer != null)
        {
            ENEMIES_EnemyHitPlayer.Invoke(attackingEnemy);
        }
        DebugLog("Enemy " + attackingEnemy.ToString() + " hit player ");
    }


    //public event Action<IEnemy> EnemyDied;
    public event Action<IEnemy> ENEMIES_EnemyDied;
    public void Invoke_ENEMIES_EnemyDied(IEnemy deadEnemy)
    {
        if (ENEMIES_EnemyDied != null)
        {
            ENEMIES_EnemyDied.Invoke(deadEnemy);
        }
        DebugLog("Enemy " + deadEnemy.ToString() + " died ");
    }

    public event Action<int> ENEMIES_WaveCompleted;
    public void Invoke_ENEMIES_WaveCompleted(int waveIndex)
    {
        if (ENEMIES_WaveCompleted != null)
        {
            ENEMIES_WaveCompleted.Invoke(waveIndex);
        }
        DebugLog("Wave " + waveIndex + " completed");
    }


    

    #endregion;

    private void DebugLog(string messageLog)
    {
        if (MotherOfManagers.Instance.IsDebugLogEvents == true)
        {
            Debug.Log("<color=green>[EVENT (old)]</color> " + messageLog);
        }
    }


}
