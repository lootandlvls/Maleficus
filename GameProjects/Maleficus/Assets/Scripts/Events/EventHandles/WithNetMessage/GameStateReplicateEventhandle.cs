using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateReplicateEventhandle : AbstractEventHandle
{   
    public EPlayerID  playerID { get; }
    public float[] playerPosition = new float[3];
    public float[] playerRotation = new float[3];

    public GameStateReplicateEventhandle(EPlayerID playerID, float[] playerPosition, float[] playerRotation)
    {
        this.playerID = playerID;
        this.playerPosition = playerPosition;
        this.playerRotation = playerRotation;
    }

    public override string GetDebugMessage()
    {
        return "";
    }

    public override AbstractNetMessage GetNetMessage()
    {
        return new Net_GameStateReplicate(playerID, playerPosition, playerRotation);
    }

}
