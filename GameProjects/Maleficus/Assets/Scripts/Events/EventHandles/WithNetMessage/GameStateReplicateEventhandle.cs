using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameStateReplicateEventhandle : AbstractEventHandle
{   
    public EPlayerID UpdatedPlayerID    { get; set; }
    public float[] playerPosition = new float[3];
    public float[] playerRotation = new float[3];

    public GameStateReplicateEventhandle(EClientID senderID, EPlayerID updatedPlayerID, float[] playerPosition, float[] playerRotation)
    {
        ID = ENetMessageID.GAME_STATE_REPLICATION;
        SenderID = senderID;

        this.UpdatedPlayerID = updatedPlayerID;
        this.playerPosition = playerPosition;
        this.playerRotation = playerRotation;
    }

    public override string GetDebugMessage()
    {
        return "";
    }

    //public override AbstractNetMessage GetNetMessage()
    //{
    //    return new Net_GameStateReplicate(playerID, playerPosition, playerRotation);
    //}

}
