using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverEventHandle : AbstractEventHandle
{
    public ETeamID TeamID { get; }

    public GameOverEventHandle(ETeamID teamID)
    {
        TeamID = teamID;
    }

    public override string GetDebugMessage()
    {
        return "Game has ended! Winner is Team: " + TeamID;
    }

    public override AbstractNetMessage GetNetMessage()
    {
        return new Net_GameOver(TeamID);
    }
}
