using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class EventManager : AbstractSingletonManager<EventManager>
{

    public override void OnSceneStartReinitialize()
    {

    }

    public MaleficusEvent<TestEventHandle> TEST_TestEvent 
        = new MaleficusEvent<TestEventHandle>("TEST_TestEvent");


    #region APP

    public MaleficusEvent<Event_StateUpdated<EAppState>> APP_AppStateUpdated 
        = new MaleficusEvent<Event_StateUpdated<EAppState>>("APP_AppStateUpdated");

    public MaleficusEvent<Event_GenericHandle<EScene>> APP_SceneWillChange 
        = new MaleficusEvent<Event_GenericHandle<EScene>>("APP_SceneWillChange");

    public MaleficusEvent<Event_GenericHandle<EScene>> APP_SceneChanged 
        = new MaleficusEvent<Event_GenericHandle<EScene>>("APP_SceneChanged");


    #endregion

    #region GAME

    public event Action<EGameMode> GAME_GameStarted;
    public void Invoke_GAME_GameStarted(EGameMode gameModeStarted)
    {
        if (GAME_GameStarted != null)
        {
            GAME_GameStarted.Invoke(gameModeStarted);
        }
        DebugLog("Game started : " + gameModeStarted);
    }

    public event Action<EGameMode> GAME_GamePaused;
    public void Invoke_GAME_GamePaused(EGameMode gameModePaused)
    {
        if (GAME_GamePaused != null)
        {
            GAME_GamePaused.Invoke(gameModePaused);
        }
        DebugLog("Game paused : " + gameModePaused);
    }

    public event Action<EGameMode> GAME_GameUnPaused;
    public void Invoke_GAME_GameUnPaused(EGameMode gameModeUnPaused)
    {
        if (GAME_GameUnPaused != null)
        {
            GAME_GameUnPaused.Invoke(gameModeUnPaused);
        }
        DebugLog("Game unpaused : " + gameModeUnPaused);
    }

    public event Action<EGameMode, bool> GAME_GameEnded;
    public void Invoke_GAME_GameEnded(EGameMode gameModeEnded, bool wasAborted = false)
    {
        if (GAME_GameEnded != null)
        {
            GAME_GameEnded.Invoke(gameModeEnded, wasAborted);
        }
        DebugLog("Game ended : " + gameModeEnded + ". Aborted : " + wasAborted.ToString());
    }

    public event Action<AbstractPlayerStats, EGameMode> GAME_PlayerStatsUpdated;
    public void Invoke_GAME_PlayerStatsUpdated(AbstractPlayerStats updatedPlayerStats, EGameMode fromGameMode)
    {
        if (GAME_PlayerStatsUpdated != null)
        {
            GAME_PlayerStatsUpdated.Invoke(updatedPlayerStats, fromGameMode);
        }
        DebugLog("Player stats " + updatedPlayerStats.ToString() + " updated for " + fromGameMode);
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
        Debug.Log("Player collected a coin");
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

    
    public event Action<SHitInfo> SPELLS_SpellHitPlayer;
    public void Invoke_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {

        if (SPELLS_SpellHitPlayer != null)
        {
            SPELLS_SpellHitPlayer.Invoke(hitInfo);
        }
        Debug.Log(hitInfo.CastedSpell.SpellName + " from player " + hitInfo.CastingPlayerID + " hit player " + hitInfo.HitPlayerID);
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
    public event Action<ISpell, EPlayerID> SPELLS_Teleport;
    public void Invoke_SPELLS_Teleport(ISpell castedSpell , EPlayerID castingPlayerID)
    {
        if (SPELLS_Teleport != null)
        {
            SPELLS_Teleport.Invoke(castedSpell, castingPlayerID);
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
    public MaleficusEvent<NetEvent_GameStateReplicate> NETWORK_GameStateReplicate 
        = new MaleficusEvent<NetEvent_GameStateReplicate>("NETWORK_GameStateReplicate") ;

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

    #region AR

    public MaleficusEvent<Event_StateUpdated<EARState>> AR_ARStateUpdated 
        = new MaleficusEvent<Event_StateUpdated<EARState>>("AR_ARStateUpdated");


    public MaleficusEvent<NetEvent_ARStagePlaced> AR_ARStagePlaced 
        = new MaleficusEvent<NetEvent_ARStagePlaced>("AR_ARStagePlayed");

    #endregion

    private void DebugLog(string messageLog)
    {
        if (MotherOfManagers.Instance.IsDebugLogEvents == true)
        {
            Debug.Log("[EVENT (old)] " + messageLog);
        }
    }


}
