using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the rules for a specific game mode.
/// Don't forget to define the correct game mode inside the awake of the extending children classes!
/// </summary>
/// <typeparam name="T"> Type of information that defines the player's stats (e.g. Reamining lives)</typeparam>
public abstract class AbstractGameMode<T> : MonoBehaviour where T : AbstractPlayerStats, new()
{
    public EGameMode GameMode                       { get { return gameMode; } }
    public Dictionary<EPlayerID, T> PlayerStats     { get { return playerStats; } }

    protected EGameMode gameMode;
    protected Dictionary<EPlayerID, T> playerStats;


    protected virtual void Awake()
    {
        // Define in child class correct game mode!
        gameMode = EGameMode.NONE;
    }

    protected virtual void Start()
    {
        // Get all connected players and initialize lives
        Dictionary<EPlayerID, bool> connectedPlayers = PlayerManager.Instance.ConnectedPlayers;
        foreach (EPlayerID playerID in connectedPlayers.Keys)
        {
            if (connectedPlayers[playerID] == true)
            {
                playerStats.Add(playerID, new T());
            }
        }
        Debug.Log(connectedPlayers.Count + " players playings");
    }

    public static implicit operator AbstractGameMode<T>(GM_Single_Lives<T> v)
    {
        throw new NotImplementedException();
    }
}
