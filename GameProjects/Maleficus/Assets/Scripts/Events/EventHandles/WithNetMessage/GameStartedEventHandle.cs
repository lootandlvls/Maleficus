using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartedEventHandle : AbstractEventHandle
{
    EPlayerID PlayerID;

    public  GameStartedEventHandle(EPlayerID playerID)
    {
        this.PlayerID = playerID;
    }
    public override string GetDebugMessage()
    {
        return "Game Started";
    }

    public override AbstractNetMessage GetNetMessage()
    {
        return new Net_GameStarted(PlayerID);
    }
}
