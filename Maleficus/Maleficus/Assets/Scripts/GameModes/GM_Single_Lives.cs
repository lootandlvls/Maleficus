using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Single_Lives<T> : AbstractGameMode<PlayerStats_Lives> where T : AbstractPlayerStats, new()
{
    // Should only be called directly after object construction (used in Start method)
    public int TotalLives { get { return totalLives; } set { totalLives = value; } }

    private int totalLives = 5;


    protected override void Awake()
    {
        base.Awake();

        // Define in child class correct game mode!
        gameMode = EGameMode.NONE;
    }


    protected override void Start()
    {
        base.Start();

        // Initialize player stats correctly
        foreach (EPlayerID playerID in PlayerStats.Keys)
        {
            PlayerStats[playerID] = new PlayerStats_Lives(TotalLives);
        }

        EventManager.Instance.PLAYERS_PlayerDied    += On_PLAYERS_PlayerDied;
        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;
    }

    private void On_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {
        EPlayerID hitPlayerID = hitInfo.HitPlayerID;
        EPlayerID hitByPlayerID = hitInfo.CastingPlayerID;
        PlayerStats[hitPlayerID].SetLastHitBy(hitByPlayerID);
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
