using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameOverEventHandle : AbstractEventHandle
{
    public ETeamID TeamID { get; }

    public GameOverEventHandle(EClientID senderID, ETeamID teamID)
    {
        ID = ENetMessageID.GAME_OVER;
        SenderID = SenderID;

        TeamID = teamID;
    }

    public override string GetDebugMessage()
    {
        return "Game has ended! Winner is Team: " + TeamID;
    }

    //public override AbstractNetMessage GetNetMessage()
    //{
    //    return new Net_GameOver(TeamID);
    //}
}
