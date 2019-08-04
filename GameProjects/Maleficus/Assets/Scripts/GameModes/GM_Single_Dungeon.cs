using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Single_Dungeon : AbstractGameMode<PlayerStats_Dungeon>
{
    // Should only be called directly after object construction (used in Start method)
    public int TotalLives { get { return totalLives; } }
    public int TotalItemsToCollect { get { return totalItemsToCollect; } }

    private int totalLives = 10;
    private int totalItemsToCollect = 3;


    protected override void Awake()
    {
        base.Awake();

        // Define in child class correct game mode!
        gameMode = EGameMode.DUNGEON;
    }


    protected override void Start()
    {
        base.Start();

        totalItemsToCollect = CoinManager.Instance.NumberOfCoins;
        totalLives = MaleficusTypes.PLAYER_LIVES_IN_DUNGEON_MODE;

        // Initialize player stats correctly/
        Dictionary<EPlayerID, bool> connectedPlayers = PlayerManager.Instance.ConnectedPlayers;             // TODO: Find a way to use playerStats from AbstractGameMode instead of reusing ConnectedPlayers
        foreach (EPlayerID playerID in connectedPlayers.Keys)
        {
            if (connectedPlayers[playerID] == true)
            {
                playerStats[playerID] = new PlayerStats_Dungeon(playerID, TotalLives, TotalItemsToCollect);
            }
        }

        EventManager.Instance.ENEMIES_EnemyHitPlayer += On_ENEMIES_EnemyAttackedPlayer;
        EventManager.Instance.ENEMIES_EnemyDied += On_ENEMIES_EnemyDied;
        EventManager.Instance.PLAYERS_PlayerCollectedCoin += On_PLAYERS_PlayerCollectedCoin;
        EventManager.Instance.ENEMIES_WaveCompleted += On_ENEMIES_WaveCompleted;
    }

    private void On_ENEMIES_WaveCompleted(int waveIndex)
    {
        if (waveIndex == totalItemsToCollect)
        {
            EventManager.Instance.Invoke_GAME_GameOver(gameMode);

        }
    }

    private void On_PLAYERS_PlayerCollectedCoin()
    {
        foreach (PlayerStats_Dungeon playerStat in playerStats.Values)
        {
            playerStat.DecrementNumberOfRemainingCoinsToCollect();

            EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(playerStat, GameMode);
        }
    }

    private void On_ENEMIES_EnemyDied(IEnemy diedEnemy)
    {
        foreach(PlayerStats_Dungeon playerStat in playerStats.Values)
        {
            playerStat.IncrementNumberOfKilledEnemies();

            EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(playerStat, GameMode);
        }
    }

    private void On_ENEMIES_EnemyAttackedPlayer(IEnemy attackingEnemy)
    {
        foreach (PlayerStats_Dungeon playerStat in playerStats.Values)
        {
            playerStat.DecrementPlayerLives();

            EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(playerStat, GameMode);

            // Check if player is dead
            if (playerStat.IsGameOver == true)
            {
                EventManager.Instance.Invoke_GAME_GameOver(gameMode);
            }
        }
    }

    private void Update()
    {
        foreach (PlayerStats_Dungeon playerStat in PlayerStats.Values)
        {
            DebugManager.Instance.Log(103, "* " + playerStat.PlayerID + " *"
                + "\nFrags : " + playerStat.NumberOfKilledEnemies
                + "\nRemaining lives :" + playerStat.RemainingLives
                + "\nNumber of remaining items :" + playerStat.RemainingNumberOfCollectedItems
                + "\n-------------------------\n"
                );
        }
    }

}
