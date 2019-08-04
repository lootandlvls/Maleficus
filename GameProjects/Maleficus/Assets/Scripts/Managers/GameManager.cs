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
        EventManager.Instance.AR_ARStateUpdated += On_AR_ARStateUpdated;
        // EventManager.Instance.AR_StagePlaced += On_AR_StagePlaced;
    }

    public override void Initialize()
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
                case EGameMode.SINGLE_LIVES_5:
                    gameObject.AddComponent<GM_FFA_Lives>();          
                    break; 

                case EGameMode.SINGLE_TIME_2:

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
        switch(gameMode)
        {
            case EGameMode.SINGLE_LIVES_5:

                break;

            case EGameMode.DUNGEON:
                GM_Single_Dungeon gameModeInstance = GetComponent<GM_Single_Dungeon>();
                Dictionary<EPlayerID, PlayerStats_Dungeon> playerStats = gameModeInstance.PlayerStats;

                // TODO

                Destroy(gameModeInstance);
                break;
        }
    }

    private void On_AR_ARStateUpdated(EARState newState, EARState lastState)
    {
        switch (newState)
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
                StartGame(EGameMode.SINGLE_LIVES_5);
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
