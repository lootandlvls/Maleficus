using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetEvent_GameStateReplication : AbstractEventHandle
{   
    public EPlayerID UpdatedPlayerID    { get; set; }
    public float[] playerPosition = new float[3];

    public NetEvent_GameStateReplication(EClientID senderID, EPlayerID updatedPlayerID, float[] playerPosition)
    {
        MessageType = ENetMessageType.GAME_STATE_REPLICATION;
        SenderID = senderID;

        this.UpdatedPlayerID = updatedPlayerID;
        this.playerPosition = playerPosition;
    }

    public override string GetDebugMessage()
    {
        return "Game state replicated for " + UpdatedPlayerID;
    }
}
