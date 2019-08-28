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

    private void Start()
    {
        EventManager.Instance.GAME_GameOver += ON_GAME_GameOver;
        EventManager.Instance.AR_ARStateUpdated.AddListener(On_AR_ARStateUpdated);
        EventManager.Instance.AR_ARStagePlaced.AddListener(On_AR_ARStagePlayerd);
        // EventManager.Instance.AR_StagePlaced += On_AR_StagePlaced;
    }

    private void On_AR_ARStagePlayerd(ARStagePlacedEventHandle obj)
    {
        // TODO Implement
    }

    public override void OnSceneStartReinitialize()
    {
        FindAndBindButtonActions();

        isCanStartGame = false;
    }



    #region Game Actions
    private void StartGame(EGameMode gameModeToStart)
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_NOT_STARTED)
        {
            currentGameMode = gameModeToStart;

            switch (gameModeToStart)
            {
                case EGameMode.FFA_LIVES:
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
    private void ON_GAME_GameOver(EGameMode gameMode)
    {
        EndGame();

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

    private void On_AR_ARStateUpdated(StateUpdatedEventHandle<EARState> eventHandle)
    {
        switch (eventHandle.NewState)
        {
            case EARState.NO_POSE:
                if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
                {
                    PauseOrUnpauseGame();
                }
                break;
        }
    }

    //private void On_AR_StagePlaced()
    //{
    //    switch (AppStateManager.Instance.CurrentState)
    //    {
    //        case EAppState.IN_GAME_IN_NOT_STARTED:
    //            StartGame(EGameMode.DUNGEON);
    //            break;

    //        case EAppState.IN_GAME_IN_PAUSED:
    //            PauseOrUnpauseGame();
    //            break;
    //    }
    //}
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
