using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Single_Lives : AbstractGameMode
{
    public int TotalLives { get { return totalLives; } set { totalLives = value; } }

    private int totalLives = 5;

    private Dictionary<EPlayerID, int> playerLives;

    private void Awake()
    {
        gameMode = EGameMode.SINGLE_LIVES_5;

        // Get all connected players and initialize lives
        Dictionary<EPlayerID, bool> connectedPlayers = PlayerManager.Instance.ConnectedPlayers;
        foreach (EPlayerID playerID in connectedPlayers.Keys)
        {
            if (connectedPlayers[playerID] == true)
            {
                playerLives.Add(playerID, totalLives);
            }
        }
        Debug.Log(connectedPlayers.Count + " players playings");
    }


    private void Start()
    {
        EventManager.Instance.PLAYERS_PlayerDied += On_PLAYERS_PlayerDied;
    }

    private void On_PLAYERS_PlayerDied(EPlayerID diedPlayerID)
    {
        // if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_RUNNING)          // TODO: check for right state
        playerLives[diedPlayerID] -= 1;

        // Check if game over
        int deadPlayersCounter = 0;
        EPlayerID winnerPlayerID = EPlayerID.NONE;
        foreach (EPlayerID playerID in playerLives.Keys)
        {
            if (playerLives[playerID] == 0)
            {
                deadPlayersCounter++;
            }
            else
            {
                winnerPlayerID = playerID;
            }
        }

        if (deadPlayersCounter == playerLives.Count - 1)
        {
            ETeamID winnerTeamID = PlayerManager.Instance.GetPlayerTeamID(winnerPlayerID);
            EventManager.Instance.Invoke_GAME_PlayerWon(winnerTeamID, gameMode);
        }
    }
}
