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
        EventManager.Instance.GAME_TeamWon += On_GAME_TeamWon;
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
                    gameObject.AddComponent<GM_Single_Lives<PlayerStats_Lives>>();          // TODO: Test if it works
                    break;

                case EGameMode.SINGLE_TIME_2:

                    break;

                case EGameMode.INSANE:

                    break;
            }

            EventManager.Instance.Invoke_GAME_GameStarted(currentGameMode);
        }
    }



    // Test function
    public void Start3LivesGame()
    {
        StartGame(EGameMode.SINGLE_LIVES_5);

    }

    private void PauseOrUnpauseGame()
    {

    }

    private void EndGame()
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
