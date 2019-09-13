using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameStartedEventHandle : AbstractEventHandle
{
    public  GameStartedEventHandle(EClientID senderID)
    {
        ID = ENetMessageID.GAME_STARTED;
        SenderID = senderID;
    }

    public override string GetDebugMessage()
    {
        return "Game Started";
    }

    //public override AbstractNetMessage GetNetMessage()
    //{
    //    return new Net_GameStarted(PlayerID);
    //}
}
