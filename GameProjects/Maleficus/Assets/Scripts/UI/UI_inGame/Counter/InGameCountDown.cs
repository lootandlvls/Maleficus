using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameCountDown : MaleficusMonoBehaviour
{
    [SerializeField] private int countdownSeconds;

    private Text myText;
    private bool isGameRunning;

    
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
                countdownSeconds = FFA_Lives.GameLenght;
               StartNewCoroutine(ref StartCountDownEnumerator , startCountDownCoroutine(countdownSeconds));
                break;
        }
    }

    private IEnumerator startCountDownCoroutine(int countDown)
    {

        for (int i = countdownSeconds; i > 0; i--)
        {
            myText.text = "Remainging Time : " + i ;
            yield return new WaitForSeconds(1.0f);
        }
    }
}
