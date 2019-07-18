using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : AbstractSingletonManager<GameManager>
{                    
    private EGameMode currentGameMode;

    private bool isCanStartGame;


    protected override void Awake()
    {
        base.Awake();

        isCanStartGame = false;
    }

    private void Start()
    {
        EventManager.Instance.GAME_GameOver += On_GAME_TeamWon;

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
                    gameObject.AddComponent<GM_FFA_Lives>();          // TODO: Test if it works
                    Debug.Log("Starting " + EGameMode.SINGLE_LIVES_5);
                    break;

                case EGameMode.SINGLE_TIME_2:

                    break;

                case EGameMode.INSANE:

                    break;

                case EGameMode.DUNGEON:
                    gameObject.AddComponent<GM_Single_Dungeon>();          // TODO: Test if it works

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

    private IEnumerator WaitingForGameStartCoroutine()
    {
        while (isCanStartGame == false)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private void On_GAME_TeamWon(ETeamID winnerTeamID, EGameMode gameMode)
    {
        EndGame();
    }



    protected override void FindAndBindButtonActions()
    {
        base.FindAndBindButtonActions();

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
