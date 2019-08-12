using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class EventManager : AbstractSingletonManager<EventManager>
{

    public override void Initialize()
    {

    }

    public MaleficusEvent<TestEventHandle> TEST_TestEvent = new MaleficusEvent<TestEventHandle>("TEST_TestEvent");


    #region APP
    //public event Action<EAppState, EAppState> APP_AppStateUpdated_old;
    //public void Invoke_APP_AppStateUpdated(EAppState newAppState, EAppState lastAppState)
    //{
    //    if (APP_AppStateUpdated_old != null)
    //    {
    //        APP_AppStateUpdated_old.Invoke(newAppState, lastAppState);
    //    }
    //    DebugLog("App state changed from " + lastAppState + " to " + newAppState);
    //}
    public MaleficusEvent<StateUpdatedEventHandle<EAppState>> APP_AppStateUpdated = new MaleficusEvent<StateUpdatedEventHandle<EAppState>>("APP_AppStateUpdated");


    //public event Action<EScene> APP_SceneWillChange_old;
    //public void Invoke_APP_SceneWillChange(EScene newScene)
    //{
    //    if (APP_SceneWillChange_old != null)
    //    {
    //        APP_SceneWillChange_old.Invoke(newScene);
    //    }
    //    DebugLog("Scene will change : " + newScene);
    //}
    public MaleficusEvent<BasicEventHandle<EScene>> APP_SceneWillChange = new MaleficusEvent<BasicEventHandle<EScene>>("APP_SceneWillChange");



    //public event Action<EScene> APP_SceneChanged_old;
    //public void Invoke_APP_SceneChanged(EScene newScene)
    //{
    //    if (APP_SceneChanged_old != null)
    //    {
    //        APP_SceneChanged_old.Invoke(newScene);
    //    }
    //    DebugLog("Scene changed : " + newScene);
    //}
    public MaleficusEvent<BasicEventHandle<EScene>> APP_SceneChanged = new MaleficusEvent<BasicEventHandle<EScene>>("APP_SceneChanged");


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


    public event Action<EGameMode> GAME_GameOver;
    public void Invoke_GAME_GameOver(EGameMode gameMode)
    {
        if (GAME_GameOver != null)
        {
            GAME_GameOver.Invoke(gameMode);
        }
        DebugLog("Game over : " + gameMode);
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
    public MaleficusEvent<StateUpdatedEventHandle<EMenuState>> UI_MenuStateUpdated = new MaleficusEvent<StateUpdatedEventHandle<EMenuState>>("UI_MenuStateUpdated");

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

    public event Action<EInputButton, EPlayerID> INPUT_ButtonReleased;
    public void Invoke_INPUT_ButtonReleased(EInputButton buttonType, EPlayerID playerID)
    {
        if (INPUT_ButtonReleased != null)
        {
            INPUT_ButtonReleased.Invoke(buttonType, playerID);
        }
        DebugLog(buttonType + " released by " + playerID);
    }

    public event Action<EInputAxis, float, EPlayerID> INPUT_JoystickMoved;
    public void Invoke_INPUT_JoystickMoved(EInputAxis axisType, float axisValue, EPlayerID playerID)
    {
        if (INPUT_JoystickMoved != null)
        {
            INPUT_JoystickMoved.Invoke(axisType, axisValue, playerID);
        }
    }

   /* public event Action<EInputButton, EPlayerID> INPUT_ButtonStillBeingPressed;
    public void Invoke_INPUT_ButtonStillBeingPressed(EInputButton buttonType, EPlayerID playerID)
    {
        if (INPUT_ButtonStillBeingPressed != null)
        {
            INPUT_ButtonStillBeingPressed.Invoke(buttonType, playerID);
        }
    }*/
    #endregion

    #region NETWORK
    public event Action<ENetworkMessage> NETWORK_ReceivedMessageUpdated;
    public void Invoke_NETWORK_ReceivedMessageUpdated(ENetworkMessage receivedMsg)
    {
        if (NETWORK_ReceivedMessageUpdated != null)
        {
            NETWORK_ReceivedMessageUpdated.Invoke(receivedMsg);
        }
        DebugLog("Client received Message: " + receivedMsg);
    }
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
    //public event Action<EARState, EARState> AR_ARStateUpdated_old;                                       
    //public void Invoke_AR_TrackingStateUpdated(EARState newARState, EARState lastState)
    //{
    //    if (AR_ARStateUpdated_old != null)
    //    {
    //        AR_ARStateUpdated_old.Invoke(newARState, lastState);
    //    }
    //    DebugLog("AR State updated : " + newARState);
    //}
    public MaleficusEvent<StateUpdatedEventHandle<EARState>> AR_ARStateUpdated = new MaleficusEvent<StateUpdatedEventHandle<EARState>>("AR_ARStateUpdated");


    public event Action AR_StagePlaced;                                       
    public void Invoke_AR_StagePlaced()
    {
        if (AR_StagePlaced != null)
        {
            AR_StagePlaced.Invoke();
        }
        DebugLog("Stage placed");
    }


    #endregion

    private void DebugLog(string messageLog)
    {
        if (MotherOfManagers.Instance.IsDebugLogEvents == true)
        {
            Debug.Log("[EVENT (old)] " + messageLog);
        }
    }


}
