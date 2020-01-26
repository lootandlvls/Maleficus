using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats_Lives : AbstractPlayerStats
{
    private EPlayerID playerID;
    private int remainingLives;
    private int numberOfHitPlayers;
    private int numberOfKilledPlayers;
    private EPlayerID lastHitBy;
    private int timeOfDeath;
    private int rank = 0;

    public PlayerStats_Lives()
    {

    }
    public PlayerStats_Lives(EPlayerID playerStatID, int maximumNumberOfLives)
    {
        playerID = playerStatID;
        remainingLives = maximumNumberOfLives;
        numberOfKilledPlayers = 0;
        lastHitBy = EPlayerID.NONE;
    }

    public EPlayerID PlayerID { get { return playerID; } }
    public int RemainingLives { get { return remainingLives; } }
    public int NumberOfHitPlayers { get { return numberOfHitPlayers; } }
    public int NumberOfKilledPlayers { get { return numberOfKilledPlayers; } }
    public int TimeOfDeath { get { return timeOfDeath; } }
    public bool IsGameOver { get { return remainingLives == 0; } }
    public int Rank { get { return rank; } }
    public EPlayerID LastHitBy { get { return lastHitBy; } }

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
    /// Icrements the number of killed players by 1
    /// </summary>
    public void IncrementNumberOfHitPlayers()
    {
        numberOfHitPlayers++;
    }
    /// <summary>
    ///  Set the time when the player is killed
    /// </summary>
    public void SetTimeOfDeath(int time)
    {
        timeOfDeath = time;
    }
    /// <summary>
    /// Set the player rank
    /// </summary>
    /// <param name="playerRank"></param>
    public void SetRank(int playerRank)
    {
        rank = playerRank;
    }

    /// <summary>
    /// Icrements the number of killed players by 1
    /// </summary>
    public void IncrementNumberOfKilledPlayers()
    {
        numberOfKilledPlayers++;
    }

    /// <summary>
    /// Sets the Player ID of the last player that hit this player.
    /// Used to determine who finally killed this player.
    /// </summary>
    public void SetLastHitBy(EPlayerID hitByPlayerID)
    {
        lastHitBy = hitByPlayerID;
    }
}