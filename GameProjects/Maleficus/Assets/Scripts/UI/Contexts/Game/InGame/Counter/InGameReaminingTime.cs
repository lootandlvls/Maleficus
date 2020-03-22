using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameReaminingTime : BNJMOBehaviour
{
    private Text myText;
    private IEnumerator StartCountDownEnumerator;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myText = GetComponentWithCheck<Text>();
        myText.text = "";
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.GAME_GameStarted += On_GAME_GameStarted;
        EventManager.Instance.GAME_GameEnded += On_Game_GameEnded;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.GAME_GameStarted -= On_GAME_GameStarted;
            EventManager.Instance.GAME_GameEnded -= On_Game_GameEnded;
        }
    }

    private void On_Game_GameEnded(AbstractGameMode gameMode, bool wasAborted)
    {
        StopCoroutine(StartCountDownEnumerator);
    }

    private void On_GAME_GameStarted(AbstractGameMode gameMode)
    {
        switch(gameMode.GameModeType)
        {
            case EGameMode.FFA_LIVES:
                GM_FFA_Lives FFA_Lives = (GM_FFA_Lives)gameMode;
                StartNewCoroutine(ref StartCountDownEnumerator , startCountDownCoroutine(FFA_Lives.GameLenght));
                break;
        }
    }

    private IEnumerator startCountDownCoroutine(int countDown)
    {
        for (int i = countDown; i >= 0; i--)
        {
            myText.text = "" + i ;
            EventManager.Instance.Invoke_GAME_GameTimeUpdated(i);
           
            yield return new WaitForSeconds(1.0f);
        }
    }
}
