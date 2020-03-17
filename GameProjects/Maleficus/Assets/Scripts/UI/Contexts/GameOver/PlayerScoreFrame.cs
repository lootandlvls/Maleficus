using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreFrame : BNJMOBehaviour

{
   [SerializeField] private EPlayerID PlayerID;
   [SerializeField] private Text NumberOfKilledPlayers;
   [SerializeField] private Text TimeSurvived;
   [SerializeField] private Text Rank;
   [SerializeField] private GameObject Crown;
   [SerializeField] private GameObject PlayerScoreFrameView;

    private bool isPlayerActive;

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

         //EventManager.Instance.GAME_GameEnded += On_Game_GameEnded;
        EventManager.Instance.GAME_SetPlayersScores += On_Game_SetPlayersScores;
    }
    protected override void InitializeComponents()
    {
        base.InitializeComponents();


        //if (PlayerManager.Instance.HasPlayerJoined(PlayerID))
        //{
        //    isPlayerActive = true;
           
        //}
        //else
        //{
        //    PlayerScoreFrameView.SetActive(false);
        //}
    }

    protected override void Start()
    {
        base.Start();

        if (PlayerManager.Instance.HasPlayerJoined(PlayerID))
        {
            isPlayerActive = true;

        }
        else
        {
            PlayerScoreFrameView.SetActive(false);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.GAME_SetPlayersScores -= On_Game_SetPlayersScores;
        }
    }

    private void On_Game_SetPlayersScores(AbstractGameMode GameMode)
    {
        
        GM_FFA_Lives gameMode = (GM_FFA_Lives)GameMode;

       
        if (isPlayerActive)
        {
            if (gameMode.PlayerStats[PlayerID] != null)
            {
                if (gameMode.PlayerStats[PlayerID].Rank == 1 )
                {
                    Crown.SetActive(true);
                }
                else
                {
                    Rank.text =  "" + gameMode.PlayerStats[PlayerID].Rank;
                }
                
                NumberOfKilledPlayers.text = "" + gameMode.PlayerStats[PlayerID].NumberOfKilledPlayers;
                TimeSurvived.text = "" + gameMode.PlayerStats[PlayerID].TimeOfDeath;
            }
        }
       
    }

    //private void On_Game_GameEnded(AbstractGameMode GameMode, bool wasAborted)
    //{
    //    Debug.Log("PLAYER STATS ASSIGNED");
    //    GM_FFA_Lives gameMode = (GM_FFA_Lives)GameMode;
    //    if (gameMode.PlayerStats[PlayerID] != null)
    //    {
    //        NumberOfKilledPlayers.text = "aaaaaa" + gameMode.PlayerStats[PlayerID].NumberOfKilledPlayers;
    //        TimeSurvived.text = "aaaa" + gameMode.PlayerStats[PlayerID].TimeSurvived;
    //    }


    //}

}
