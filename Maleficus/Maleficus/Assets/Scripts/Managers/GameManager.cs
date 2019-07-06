using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private AbstractGameMode currentGameMode;

    private bool isCanStartGame;


    protected override void Awake()
    {
        base.Awake();

        isCanStartGame = false;
    }

    private void Start()
    {
        EventManager.Instance.GAME_TeamWon += On_GAME_TeamWon;
    }

    

    #region Game Actions
    public void StartGame(EGameMode gameModeToStart)
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_NOT_STARTED)
        {
            switch (gameModeToStart)
            {
                case EGameMode.SINGLE_LIVES_5:
                    currentGameMode = new GM_Single_Lives();
                    break;

                case EGameMode.SINGLE_TIME_2:

                    break;

                case EGameMode.INSANE:

                    break;
            }

            EventManager.Instance.Invoke_GAME_GameAboutToStart(currentGameMode.GameMode);
        }
    }

    // Test function
    public void Start3LivesGame()
    {
        StartGame(EGameMode.SINGLE_LIVES_5);

    }

    public void PauseOrUnpauseGame()
    {

    }

    public void EndGame()
    {

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
}
