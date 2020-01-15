using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.Utils;

[System.Serializable]
public class NetEvent_ButtonPressed : AbstractEventHandle
{
    public EInputButton InputButton { get; }

    public NetEvent_ButtonPressed(EClientID senderID, EInputButton inputButton)
    {
        MessageType = ENetMessageType.BUTTON_PRESSED;
        SenderID = senderID;

        InputButton = inputButton;
    }

    public override string GetDebugMessage()
    {
        EPlayerID playerID = GetPlayerIDFrom(SenderID);
        return playerID + " pressed " + InputButton;
    }
}
