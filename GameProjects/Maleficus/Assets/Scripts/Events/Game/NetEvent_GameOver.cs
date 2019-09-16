using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetEvent_GameOver : AbstractEventHandle
{
    public ETeamID TeamID { get; }

    public NetEvent_GameOver(EClientID senderID, ETeamID teamID)
    {
        ID = ENetMessageID.GAME_OVER;
        SenderID = SenderID;

        TeamID = teamID;
    }

    public override string GetDebugMessage()
    {
        return "Game has ended! Winner is Team: " + TeamID;
    }
}
