using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetEvent_ButtonReleased : AbstractEventHandle
{
    public EInputButton InputButton { get; }

    public NetEvent_ButtonReleased(EClientID senderID, EInputButton inputButton)
    {
        ID = ENetMessageID.BUTTON_RELEASEED;
        SenderID = senderID;

        InputButton = inputButton;
    }

    public override string GetDebugMessage()
    {
        EPlayerID playerID = MaleficusUtilities.GetPlayerIDFrom(SenderID);
        return playerID + " released " + InputButton;
    }
}