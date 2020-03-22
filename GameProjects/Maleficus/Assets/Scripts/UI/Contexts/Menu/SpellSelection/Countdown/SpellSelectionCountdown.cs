using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpellSelectionCountdown : AbstractSingletonManager<SpellSelectionCountdown>
{
    public event Action<int> CountdownProgressed;

    [SerializeField] private int countdownSeconds = 5;

    private Text myText;
    private IEnumerator StartCountdownEnumerator;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myText = GetComponentWithCheck<Text>();
        myText.text = "";
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.PLAYERS_AllPlayersReady += On_PLAYERS_AllPlayersReady;
        EventManager.Instance.PLAYERS_PlayerJoined += On_PLAYERS_PlayerJoined;
        EventManager.Instance.PLAYERS_PlayerCanceledReady += On_PLAYERS_PlayerCanceledReady;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.PLAYERS_AllPlayersReady -= On_PLAYERS_AllPlayersReady;
            EventManager.Instance.PLAYERS_PlayerJoined -= On_PLAYERS_PlayerJoined;
            EventManager.Instance.PLAYERS_PlayerCanceledReady -= On_PLAYERS_PlayerCanceledReady;
        }
    }

    private void On_PLAYERS_AllPlayersReady()
    {
        StartNewCoroutine(ref StartCountdownEnumerator, StartCountdownCoroutine());
    }

    private void On_PLAYERS_PlayerJoined(EPlayerID playerID, EControllerID controllerID)
    {
        
        StopCoroutineIfRunning(StartCountdownEnumerator);

        myText.text = "";
    }

    private void On_PLAYERS_PlayerCanceledReady(EPlayerID playerID)
    {
        StopCoroutineIfRunning(StartCountdownEnumerator);

        myText.text = "";
    }

    private IEnumerator StartCountdownCoroutine()
    {
        for (int i = countdownSeconds; i > 0; i--)
        {
            myText.text = "Starting in " + i + "...";
            UISoundManager.Instance.SpawnSound_Countdown();
            InvokeEventIfBound(CountdownProgressed, i);
            yield return new WaitForSeconds(1.0f);
        }
        myText.text = "Game starting...";
        UISoundManager.Instance.SpawnSound_CountdownFinished();
        EventManager.Instance.Invoke_GAME_CountdownFinished();
    }
}
