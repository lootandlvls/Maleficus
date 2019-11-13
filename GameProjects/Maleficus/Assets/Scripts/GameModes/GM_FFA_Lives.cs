using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.MaleficusConsts;

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

        totalLives = PLAYER_LIVES_IN_FFA_MODE;
    }

    protected override void Start()
    {
        base.Start();

        EventManager.Instance.PLAYERS_PlayerDied            += On_PLAYERS_PlayerDied;
        EventManager.Instance.SPELLS_SpellHitPlayer         += On_SPELLS_SpellHitPlayer;
    }

    private void Update()
    {
        foreach(PlayerStats_Lives playerStat in PlayerStats.Values)
        {
            DebugManager.Instance.Log(103, "* " + playerStat.PlayerID + " *"
                + "\nFrags : " + playerStat.NumberOfKilledPlayers
                + "\nRemaining lives :" + playerStat.RemainingLives
                + "\nNumber of hit players :" + playerStat.NumberOfHitPlayers
                + "\nNumber of killed players :" + playerStat.NumberOfKilledPlayers
                + "\nLast hit by : " + playerStat.LastHitBy
                + "\n-------------------------\n"
                );
        }
    }

    protected override void InitializePlayerStats()
    {
        Debug.Log("[GAME_LOOP_FIX] - Connected players number : " + PlayerManager.Instance.GetConnectedPlayers().Length);

        foreach (EPlayerID playerID in PlayerManager.Instance.GetConnectedPlayers())
        {
            Debug.Log("[GAME_LOOP_FIX] - Adding playeyr : " + playerID);
            playerStats.Add(playerID, new PlayerStats_Lives(playerID, TotalLives));
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


        // Update Killed Player stats
        PlayerStats_Lives killedPlayerStats = PlayerStats[diedPlayerID];
        killedPlayerStats.DecrementPlayerLives();
        EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(killedPlayerStats, GameMode);

        // Update Killer stats
        if (killedPlayerStats.LastHitBy != EPlayerID.NONE)
        {
            PlayerStats_Lives killingPlayer = PlayerStats[killedPlayerStats.LastHitBy];
            killingPlayer.IncrementNumberOfKilledPlayers();
            EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(killingPlayer, GameMode);
        }

        // Respawn Killed Player
        if (killedPlayerStats.RemainingLives > 0)
        {
            PlayerManager.Instance.SpawnPlayer(diedPlayerID);
        }

        // Check if game over (only one player still alive)
        int gameOverPlayerCounter = 0;
        EPlayerID winnerPlayerID = EPlayerID.NONE;
        foreach (EPlayerID playerID in PlayerStats.Keys)
        {
            if (PlayerStats[playerID].IsGameOver == true)
            {
                gameOverPlayerCounter++;
            }
            else
            {
                winnerPlayerID = playerID;
            }
        }
        Debug.Log("[GAME_LOOP_FIX] - Game Over Player Counter : " + gameOverPlayerCounter + " | Player counts : " + playerStats.Count);

        //TODO[BNJMO] fix this when only one player is connected
        if (gameOverPlayerCounter == PlayerStats.Count - 1)
        {
            
            ETeamID winnerTeamID = PlayerManager.Instance.PlayersTeam[winnerPlayerID];
            ETeamID teamID = ETeamID.NONE;

            if ((NetworkManager.Instance.HasAuthority == true)
                || (MotherOfManagers.Instance.ConnectionMode == EConnectionMode.PLAY_OFFLINE))
            {
                NetEvent_GameOver gameOverEventHandle = new NetEvent_GameOver(EClientID.SERVER, teamID);
                EventManager.Instance.GAME_GameOver.Invoke(gameOverEventHandle, EEventInvocationType.TO_ALL);
            }
            
        }

    }


}
