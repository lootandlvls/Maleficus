using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Dictionary<EPlayerID, int> playerScores;

    private AbstractGameMode currentGameMode;

    private bool isCanStartGame;


    protected override void Awake()
    {
        base.Awake();

        isCanStartGame = false;
    }




    #region Game Commands
    public void StartGame(EGameMode gameModeToStart)
    {
        switch (gameModeToStart)
        {
            case EGameMode.LIVES_3:
                currentGameMode = new Lives3GameMode();
                break;

            case EGameMode.TIME_2_MINUTES:

                break;

            case EGameMode.INSANE:

                break;
        }
    }

    public void Start3LivesGame()
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_NOT_STARTED)
        {
            Debug.Log("Starting 3 lives game");
            currentGameMode = new Lives3GameMode();

            EventManager.Instance.Invoke_GAME_GameAboutToStart(currentGameMode.GameMode);
        }

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


}
