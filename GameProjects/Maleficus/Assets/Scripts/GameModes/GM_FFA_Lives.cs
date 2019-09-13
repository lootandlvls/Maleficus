using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_FFA_Lives : AbstractGameMode<PlayerStats_Lives>
{
    // Should only be called directly after object construction (used in Start method)
    public int TotalLives { get { return totalLives; } }

    private int totalLives;


    protected override void Awake()
    {
        base.Awake();

        // Define in child class correct game mode!
        gameMode = EGameMode.FFA_LIVES;

        totalLives = MaleficusConsts.PLAYER_LIVES_IN_FFA_MODE;
    }

    protected override void Start()
    {
        base.Start();

        EventManager.Instance.PLAYERS_PlayerDied += On_PLAYERS_PlayerDied;
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

        EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(PlayerStats[hitPlayerID], GameMode);
        EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(PlayerStats[hitByPlayerID], GameMode);
    }

    private void On_PLAYERS_PlayerDied(EPlayerID diedPlayerID)
    {
        // if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_RUNNING)          // TODO: check for right state
        Debug.Log("PLAYER DIED : " + diedPlayerID);
        // Update remaining lives and kills counter
        PlayerStats_Lives killedPlayer = PlayerStats[diedPlayerID];
        killedPlayer.DecrementPlayerLives();
        EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(killedPlayer, GameMode);

        if (killedPlayer.LastHitBy != EPlayerID.NONE)
        {
            PlayerStats_Lives killingPlayer = PlayerStats[killedPlayer.LastHitBy];
            killingPlayer.IncrementNumberOfKilledPlayers();
            EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(killingPlayer, GameMode);
        }


        if (killedPlayer.RemainingLives > 0)
        {
            Debug.Log("$%&$%&$%&b Respawn Player");
            PlayerManager.Instance.SpawnPlayer(diedPlayerID);
        }
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
        //TODO[BNJMO] fix this when only one player is connected
        if (deadPlayersCounter == PlayerStats.Count - 1)
        {
            //
            ETeamID winnerTeamID = PlayerManager.Instance.PlayersTeam[winnerPlayerID];
            ETeamID teamID = ETeamID.NONE;

            if (NetworkManager.Instance.HasAuthority == true)
            {
                GameOverEventHandle gameOverEventHandle = new GameOverEventHandle(EClientID.SERVER, teamID);
                EventManager.Instance.GAME_GameOver.Invoke(gameOverEventHandle, EEventInvocationType.TO_ALL);
            }
            
        }

    }

    protected override void InitializePlayerStats()
    {
        Dictionary<EPlayerID, bool> connectedPlayers = PlayerManager.Instance.ConnectedPlayers;
        foreach (EPlayerID playerID in connectedPlayers.Keys)
        {
            if (connectedPlayers[playerID] == true)
            {
                playerStats.Add(playerID, new PlayerStats_Lives(playerID, TotalLives));
            }
        }
    }
}
