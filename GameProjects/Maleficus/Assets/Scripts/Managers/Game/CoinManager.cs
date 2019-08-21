using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : AbstractSingletonManager<CoinManager>

{

    public int NumberOfCoins { get { return numberOfCoins; } }

    private int numberOfCoins = 0;
    private int lastSpawnIndex = 0;
    private Dictionary<int, Coin> AllCoins = new Dictionary<int, Coin>();


    private void Start()
    {
        EventManager.Instance.PLAYERS_PlayerCollectedCoin += On_PLAYERS_PlayerCollectedCoin;
        EventManager.Instance.APP_AppStateUpdated.AddListener(On_APP_AppStateUpdated);
        EventManager.Instance.ENEMIES_WaveCompleted += On_ENEMIES_WaveCompleted;
    }

    public override void OnSceneStartReinitialize()
    {
        numberOfCoins = 0;
        AllCoins.Clear();
        Coin[] coinList = FindObjectsOfType<Coin>();
        foreach (Coin coin in coinList)
        {
            AllCoins.Add(numberOfCoins, coin);
            numberOfCoins++;
            coin.gameObject.SetActive(false);
        }
    }

    private void SpawnCoin(int coinToSpawnID)
    {
        Debug.Log("Coin to spawn : " + coinToSpawnID);
        if (coinToSpawnID < numberOfCoins)
        {
            AllCoins[coinToSpawnID].gameObject.SetActive(true);
        }

    }

    private void On_APP_AppStateUpdated(StateUpdatedEventHandle<EAppState> eventHandle)
    {
        if (eventHandle.NewState == EAppState.IN_GAME_IN_RUNNING)
        {
            if (AllCoins.Count != 0)
            {
                AllCoins[0].gameObject.SetActive(true);
            }
        }
    }

    private void On_ENEMIES_WaveCompleted(int waveIndex)
    {
        SpawnCoin(lastSpawnIndex);
    }

    private void On_PLAYERS_PlayerCollectedCoin()
    {
        Debug.Log("Player Collected coin number " + lastSpawnIndex);
        lastSpawnIndex++;
    }

}
