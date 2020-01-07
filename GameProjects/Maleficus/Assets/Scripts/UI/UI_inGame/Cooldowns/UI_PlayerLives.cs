using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerLives : MaleficusMonoBehaviour
{

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.GAME_PlayerStatsUpdated += On_GAME_PlayerStatsUpdated;
    }

    private void On_GAME_PlayerStatsUpdated(AbstractPlayerStats playerStats, EGameMode gameMode)
    {
        switch (gameMode)
        {
            case EGameMode.FFA_LIVES:
                PlayerStats_Lives playerStatsFFA = (PlayerStats_Lives)playerStats;

                //if playerStatsFFA.PlayerID

                break;
        }
    }
}
