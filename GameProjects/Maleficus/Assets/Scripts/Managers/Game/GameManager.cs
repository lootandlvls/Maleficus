using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : AbstractSingletonManager<GameManager>
{                    
    private EGameMode currentGameMode;

    private bool isCanStartGame = false;


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        EventManager.Instance.GAME_GameOver.AddListener(ON_GAME_GameOver);
        EventManager.Instance.NETWORK_GameStarted.AddListener(On_NETWORK_GameStarted);
        
    }

    private void On_NETWORK_GameStarted(NetEvent_GameStarted obj)
    {
        Debug.Log("[GAME_LOOP_FIX] Game started event");

        StartGame(EGameMode.FFA_LIVES);
    }

   

    public override void OnSceneStartReinitialize()
    {
        FindAndBindButtonActions();

        isCanStartGame = false;
    }



    #region Game Actions
    private void StartGame(EGameMode gameModeToStart)
    {
        currentGameMode = gameModeToStart;

        switch (gameModeToStart)
        {
            case EGameMode.FFA_LIVES:
                Debug.Log("[GAME_LOOP_FIX] Starting GM_FFA_Lives");
                gameObject.AddComponent<GM_FFA_Lives>();          
                break; 

            case EGameMode.FFA_TIME:

                break;

            case EGameMode.INSANE:

                break;

            case EGameMode.DUNGEON:
                gameObject.AddComponent<GM_Single_Dungeon>();          

                break;
        }

        EventManager.Instance.Invoke_GAME_GameStarted(currentGameMode);
    }


    private void PauseOrUnpauseGame()
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {
            EventManager.Instance.Invoke_GAME_GamePaused(currentGameMode);
        }
        else if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_PAUSED)
        {
            EventManager.Instance.Invoke_GAME_GameUnPaused(currentGameMode);
        }
    }

    private void EndGame(bool wasAborted = false)
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {
            EventManager.Instance.Invoke_GAME_GameEnded(currentGameMode, wasAborted);
        }
    }

    #endregion


#region Event Callbacks 
    private void ON_GAME_GameOver(NetEvent_GameOver eventHandle)
    {
        EndGame();
        //Todo change back to enable other game modes
        EGameMode gameMode = EGameMode.FFA_LIVES;
                                                                                    // TODO Show player stats according to game mode
        switch (gameMode)
        {
            case EGameMode.FFA_LIVES:
                GM_FFA_Lives gameModeInstanceFFA = GetComponent<GM_FFA_Lives>();
                Dictionary<EPlayerID, PlayerStats_Lives> playerStatsFFA = gameModeInstanceFFA.PlayerStats;


                Destroy(gameModeInstanceFFA);
                break;

            case EGameMode.DUNGEON:
                GM_Single_Dungeon gameModeInstanceDungeon = GetComponent<GM_Single_Dungeon>();
                Dictionary<EPlayerID, PlayerStats_Dungeon> playerStatsDungeon = gameModeInstanceDungeon.PlayerStats;


                Destroy(gameModeInstanceDungeon);
                break;
        }
    }
    #endregion

    private void FindAndBindButtonActions()
    {
        /* In GAME */
        StartTestGameAction[] startTestGameActions = FindObjectsOfType<StartTestGameAction>();
        foreach (StartTestGameAction action in startTestGameActions)
        {
            action.ActionButtonPressed += () =>
            {
                StartGame(EGameMode.FFA_LIVES);
            };
        }

        StartDungeonSingleGame[] startDungeonSingleGameActions = FindObjectsOfType<StartDungeonSingleGame>();
        foreach (StartDungeonSingleGame action in startDungeonSingleGameActions)
        {
            action.ActionButtonPressed += () =>
            {
                StartGame(EGameMode.DUNGEON);
            };
        }

        //StartGameAction[] startGameActions = FindObjectsOfType<StartGameAction>();
        //foreach (StartGameAction action in startGameActions)
        //{
        //    action.ActionButtonPressed += () =>
        //    {
        //       // StartGame(EGameMode.FFA_LIVES);
        //    };
        //}



        PauseGameAction[] pauseGameActions = FindObjectsOfType<PauseGameAction>();
        foreach (PauseGameAction action in pauseGameActions)
        {
            action.ActionButtonPressed += () =>
            {
                PauseOrUnpauseGame();
            };
        }

        AbortGameAction[] abortGameActions = FindObjectsOfType<AbortGameAction>();
        foreach (AbortGameAction action in abortGameActions)
        {
            action.ActionButtonPressed += () =>
            {
                EndGame(true);
            };
        }
    }


}
