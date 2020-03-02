﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.Consts;

public class GM_FFA_Lives : ConcreteGameMode<PlayerStats_Lives>
{
    public int TotalLives { get; private set; }
    public int GameLenght { get; private set; } = 180;
    public int RemainingTime { get; private set; } = 100;

    protected override void Awake()
    {
        base.Awake();

        // Define in child class correct game mode!
        GameModeType = EGameMode.FFA_LIVES;
        GameLenght = FFA_MODE_GAME_LENGHT;
        TotalLives = FFA_MODE_PLAYER_LIVES;
    }

    protected override void Start()
    {
        base.Start();

        EventManager.Instance.PLAYERS_PlayerDied            += On_PLAYERS_PlayerDied;
        EventManager.Instance.SPELLS_SpellHitPlayer         += On_SPELLS_SpellHitPlayer;
        EventManager.Instance.GAME_GameTimeUpdated += On_GAME_GameTimeUpdated;
    }

    protected override void Update()
    {
        base.Update();

        // Debug Player stats
        if (IsRunning)
        {
            foreach (PlayerStats_Lives playerStat in PlayerStats.Values)
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
    }

    protected override void InitializePlayerStats()
    {
        PlayerStats.Clear();
        foreach (EPlayerID playerID in PlayerManager.Instance.GetJoinedPlayers())
        {
            PlayerStats.Add(playerID, new PlayerStats_Lives(playerID, TotalLives));
        }
    }

    private void On_GAME_GameTimeUpdated(int newTime)
    {
        RemainingTime = newTime;      
    }

    private void On_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {
        if (IsRunning == false)
        {
            return;
        }
        EPlayerID hitPlayerID = hitInfo.HitPlayerID;
        EPlayerID hitByPlayerID = hitInfo.CastingPlayerID;
        if (IS_KEY_CONTAINED(PlayerStats, hitPlayerID))
        {
            PlayerStats[hitPlayerID].SetLastHitBy(hitByPlayerID);
        }
        if (IS_KEY_CONTAINED(PlayerStats, hitByPlayerID))
        {
            PlayerStats[hitByPlayerID].IncrementNumberOfHitPlayers();
        }

        EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(PlayerStats[hitPlayerID], GameModeType);
    }

    private void On_PLAYERS_PlayerDied(EPlayerID diedPlayerID)
    {
        if (IsRunning == false)
        {
            return;
        }
        // Update Killed Player stats
        PlayerStats_Lives killedPlayerStats = PlayerStats[diedPlayerID];
        killedPlayerStats.DecrementPlayerLives();
        EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(killedPlayerStats, GameModeType);

        // Update Killer stats
        if (killedPlayerStats.LastHitBy != EPlayerID.NONE)
        {
            PlayerStats_Lives killingPlayer = PlayerStats[killedPlayerStats.LastHitBy];
            killingPlayer.IncrementNumberOfKilledPlayers();
            EventManager.Instance.Invoke_GAME_PlayerStatsUpdated(killingPlayer, GameModeType);
        }
      
        // Respawn Killed Player
        if (killedPlayerStats.RemainingLives > 0)
        {
            PlayerManager.Instance.RespawnPlayer(diedPlayerID);
        }
        else
        {
            killedPlayerStats.SetTimeOfDeath(RemainingTime);
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

        if (gameOverPlayerCounter == PlayerStats.Count - 1)
        {
            ETeamID winnerTeamID = PlayerManager.Instance.PlayersTeam[winnerPlayerID];
            ETeamID teamID = ETeamID.NONE;


            //Itirate through the the players time of death and set the ranking
           foreach (PlayerStats_Lives playerStats in PlayerStats.Values)
            {
                int counter = 0;
                foreach (PlayerStats_Lives otherPlayerStats in PlayerStats.Values )
                {
                    if (playerStats != otherPlayerStats && playerStats.TimeOfDeath > otherPlayerStats.TimeOfDeath)
                    {
                        counter++;
                      //  Debug.Log(" Player " + Maleficus.Utils.GetIntFrom(playerStats.PlayerID)  + " Time of Death = " + playerStats.TimeOfDeath + " died before Player " + Maleficus.Utils.GetIntFrom(otherPlayerStats.PlayerID) + " with a Time of Death = " + otherPlayerStats.TimeOfDeath);
                    }
                }
                playerStats.SetRank(counter + 1);
            }

           if ((NetworkManager.Instance.HasAuthority == true)
                || (MotherOfManagers.Instance.ConnectionMode == EConnectionMode.PLAY_OFFLINE))
            {
                NetEvent_GameOver gameOverEventHandle = new NetEvent_GameOver(EClientID.SERVER, teamID);
                EventManager.Instance.GAME_GameOver.Invoke(gameOverEventHandle, EEventInvocationType.TO_ALL);
            }
        }
    }
}
