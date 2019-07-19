using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsUI : MonoBehaviour
{

    int numberOfCoins = 0;
    Text UI_numberOFCoins;
    // Start is called before the first frame update
    void Start()
    {
        UI_numberOFCoins = GetComponent<Text>();
        EventManager.Instance.PLAYERS_PlayerCollectedCoin += On_PLAYERS_PlayerCollectedCoin;
    }

    private void On_PLAYERS_PlayerCollectedCoin()
    {
        numberOfCoins++;

        UI_numberOFCoins.text = "Coins = " + numberOfCoins;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
