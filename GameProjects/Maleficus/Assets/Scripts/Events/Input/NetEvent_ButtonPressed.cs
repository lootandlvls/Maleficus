using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetEvent_ButtonPressed : AbstractEventHandle
{
    public EInputButton InputButton { get; }

    public NetEvent_ButtonPressed(EClientID senderID, EInputButton inputButton)
    {
        ID = ENetMessageID.BUTTON_PRESSED;
        SenderID = senderID;

        InputButton = inputButton;
    }

    public override string GetDebugMessage()
    {
        EPlayerID playerID = MaleficusUtilities.GetPlayerIDFrom(SenderID);
        return playerID + " pressed " + InputButton;
    }
}
