using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : AbstractSingletonManager<CoinManager>

{

    public int numberOfCoins = 0;
    private int lastSpawnIndex = 0;
    private Dictionary<int, Coin> AllCoins = new Dictionary<int, Coin>();


    private void Start()
    {
        EventManager.Instance.PLAYERS_PlayerCollectedCoin += On_PLAYERS_PlayerCollectedCoin;

        Coin[] coinList = FindObjectsOfType<Coin>();
        foreach (Coin coin in coinList)
        {

            AllCoins.Add(numberOfCoins, coin);
            numberOfCoins++;
            Debug.Log("counter = " + numberOfCoins);
            coin.gameObject.SetActive(false);
        }
        AllCoins[0].gameObject.SetActive(true);
    }
    public override void Initialize()
    {
        base.Initialize();
       

        Coin[] coinList = FindObjectsOfType<Coin>();
        foreach (Coin coin in coinList)
        {

            AllCoins.Add(numberOfCoins, coin);
            numberOfCoins++;
            Debug.Log("counter = " + numberOfCoins);
            coin.gameObject.SetActive(false);
        }
        AllCoins[0].gameObject.SetActive(true);


    }

    private void On_PLAYERS_PlayerCollectedCoin()
    {
        Debug.Log("Player Collected coin number " + lastSpawnIndex);
        lastSpawnIndex++;
        SpawnCoin(lastSpawnIndex);
       

    }
    private void SpawnCoin(int coinToSpawnID)
    {
        Debug.Log("Coin to spawn : " + coinToSpawnID);
        if (coinToSpawnID < numberOfCoins)
        {
            AllCoins[coinToSpawnID].gameObject.SetActive(true);
        }
       
    }
}
