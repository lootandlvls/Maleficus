using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : BNJMOBehaviour
{
    [SerializeField] GameObject GameOverView;
    [SerializeField] GameObject InGameView;
    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();
        EventManager.Instance.GAME_GameEnded += On_Game_GameEnded;
    }

    private void On_Game_GameEnded(AbstractGameMode gameMode, bool wasAborted)
    {
        InGameView.SetActive(false);
        GameOverView.SetActive(true);
        StartCoroutine(wait(gameMode));
    }


    IEnumerator wait(AbstractGameMode gameMode)
    {
        yield return new WaitForSeconds(0.2f);
        EventManager.Instance.Invoke_GAME_SetPlayersScores(gameMode);
    }
}
