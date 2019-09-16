using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetEvent_GameStarted : AbstractEventHandle
{
    public  NetEvent_GameStarted(EClientID senderID)
    {
        ID = ENetMessageID.GAME_STARTED;
        SenderID = senderID;
    }

    public override string GetDebugMessage()
    {
        return "Game Started";
    }
}
