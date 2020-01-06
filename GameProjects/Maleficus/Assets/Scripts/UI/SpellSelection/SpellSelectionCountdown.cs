using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpellSelectionCountdown : MaleficusMonoBehaviour
{
    [SerializeField] private int countdownSeconds = 5;

    private Text myText;

    private bool isPerformingCountdown = false;
    private int counter = 0;
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

    private void On_PLAYERS_AllPlayersReady()
    {
        StartNewCoroutine(ref StartCountdownEnumerator, StartCountdownCoroutine());
    }

    private void On_PLAYERS_PlayerJoined(EPlayerID playerID)
    {
        StopCoroutine(StartCountdownEnumerator);

        myText.text = "";
    }

    private void On_PLAYERS_PlayerCanceledReady(EPlayerID playerID)
    {
        StopCoroutine(StartCountdownEnumerator);

        myText.text = "";
    }

    private IEnumerator StartCountdownCoroutine()
    {
        for (int i = countdownSeconds; i > 0; i--)
        {
            myText.text = "Starting in " + i + "...";
            yield return new WaitForSeconds(1.0f);
        }
        myText.text = "Game starting...";
        EventManager.Instance.Invoke_GAME_CountdownFinished();
    }
}
