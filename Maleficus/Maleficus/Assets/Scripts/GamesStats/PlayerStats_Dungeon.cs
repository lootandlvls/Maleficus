using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats_Dungeon : AbstractPlayerStats
{
    private EPlayerID playerID;
    private int remainingLives;
    private int numberOfKilledEnemies;
    private int remainingNumberOfCollectedItems;

    public PlayerStats_Dungeon()
    {

    }
    public PlayerStats_Dungeon(EPlayerID playerStatID, int maximumNumberOfLives, int maximumNumberOfItemsToCollet)
    {
        playerID = playerStatID;
        remainingLives = maximumNumberOfLives;
        remainingNumberOfCollectedItems = maximumNumberOfItemsToCollet;
        numberOfKilledEnemies = 0;
    }

    public EPlayerID PlayerID { get { return playerID; } }
    public int RemainingLives { get { return remainingLives; } }
    public int NumberOfKilledEnemies { get { return numberOfKilledEnemies; } }
    public int RemainingNumberOfCollectedItems { get { return remainingNumberOfCollectedItems; } }
    public bool IsGameOver { get { return remainingLives == 0; } }
    public bool IsGameWon { get { return remainingNumberOfCollectedItems == 0; } }

    /// <summary>
    /// Decrement by 1 a player's lives and tell if he died.
    /// </summary>
    /// <returns> are reamining lives = 0 </returns>
    public bool DecrementPlayerLives()
    {
        remainingLives--;
        return remainingLives == 0;
    }

    /// <summary>
    /// Decrements number of remaining items to collect by 1
    /// </summary>
    public void IncrementNumberOfCollectedItems()
    {
        remainingNumberOfCollectedItems--;
    }

    /// <summary>
    /// Icrements the number of killed enemies by 1
    /// </summary>
    public void IncrementNumberOfKilledEnemies()
    {
        numberOfKilledEnemies++;
    }

}
