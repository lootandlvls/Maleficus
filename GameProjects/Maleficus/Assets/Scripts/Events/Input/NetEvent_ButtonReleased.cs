using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.Utils;

[System.Serializable]
public class NetEvent_ButtonReleased : AbstractEventHandle
{
    public EInputButton InputButton { get; }

    public NetEvent_ButtonReleased(EClientID senderID, EInputButton inputButton)
    {
        MessageType = ENetMessageType.BUTTON_RELEASEED;
        SenderID = senderID;

        InputButton = inputButton;
    }

    public override string GetDebugMessage()
    {
        EPlayerID playerID = GetPlayerIDFrom(SenderID);
        return playerID + " released " + InputButton;
    }
}