using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_FFA_Lives : AbstractGameMode<PlayerStats_Lives>
{
    // Should only be called directly after object construction (used in Start method)
    public int TotalLives { get { return totalLives; } set { totalLives = value; } }

    private int totalLives = 5;


    protected override void Awake()
    {
        base.Awake();

        // Define in child class correct game mode!
        gameMode = EGameMode.SINGLE_LIVES_5;
    }


    protected override void Start()
    {
        base.Start();

        // Initialize player stats correctly/
        Dictionary<EPlayerID, bool> connectedPlayers = PlayerManager.Instance.ConnectedPlayers;             // TODO: Find a way to use playerStats from AbstractGameMode instead of reusing ConnectedPlayers
        foreach (EPlayerID playerID in connectedPlayers.Keys)
        {
            if (connectedPlayers[playerID] == true)
            {
                playerStats[playerID] = new PlayerStats_Lives(playerID, TotalLives);                  
            }
        }

        EventManager.Instance.PLAYERS_PlayerDied    += On_PLAYERS_PlayerDied;
        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;
    }


    private void Update()
    {
        foreach(PlayerStats_Lives playerStat in PlayerStats.Values)
        {
            DebugManager.Instance.Log(103, "* " + playerStat.PlayerID + " *"
                + "\nFrags : " + playerStat.NumberOfKilledPlayers
                + "\nRemaining lives :" + playerStat.RemainingLives
                + "\nNumber of hit players :" + playerStat.NumberOfHitPlayers
                + "\nLast hit by : " + playerStat.LastHitBy
                + "\n-------------------------\n"
                );
        }
    }

    private void On_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {
        EPlayerID hitPlayerID = hitInfo.HitPlayerID;
        EPlayerID hitByPlayerID = hitInfo.CastingPlayerID;
        PlayerStats[hitPlayerID].SetLastHitBy(hitByPlayerID);
        PlayerStats[hitByPlayerID].IncrementNumberOfHitPlayers();
    }

    private void On_PLAYERS_PlayerDied(EPlayerID diedPlayerID)
    {
        // if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_RUNNING)          // TODO: check for right state

        // Update remaining lives and kills counter
        PlayerStats_Lives killedPlayer = PlayerStats[diedPlayerID];
        PlayerStats_Lives killingPlayer = PlayerStats[killedPlayer.LastHitBy];
        killedPlayer.DecrementPlayerLives();
        killingPlayer.IncrementNumberOfKilledPlayers();

        // Check if game over (only one player still alive)
        int deadPlayersCounter = 0;
        EPlayerID winnerPlayerID = EPlayerID.NONE;
        foreach (EPlayerID playerID in PlayerStats.Keys)
        {
            if (PlayerStats[playerID].IsGameOver == true)
            {
                deadPlayersCounter++;
            }
            else
            {
                winnerPlayerID = playerID;
            }
        }
        if (deadPlayersCounter == PlayerStats.Count - 1)
        {
            //
            ETeamID winnerTeamID = PlayerManager.Instance.GetPlayerTeamID(winnerPlayerID);
            EventManager.Instance.Invoke_GAME_PlayerWon(winnerTeamID, gameMode);
        }
    }
}
