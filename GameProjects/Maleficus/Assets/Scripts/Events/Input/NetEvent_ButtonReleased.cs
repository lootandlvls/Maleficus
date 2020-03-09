using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.Utils;

[System.Serializable]
public class NetEvent_ButtonReleased : AbstractEventHandle
{
    public EControllerID ControllerID { get; }
    public EInputButton InputButton { get; }

    public NetEvent_ButtonReleased(EClientID senderID, EControllerID controllerID, EInputButton inputButton)
    {
        MessageType = ENetMessageType.BUTTON_RELEASEED;
        SenderID = senderID;

        ControllerID = controllerID;
        InputButton = inputButton;
    }

    public override string GetDebugMessage()
    {
        EPlayerID playerID = GetPlayerIDFrom(SenderID);
        return ControllerID + " released " + InputButton + " as : " + playerID;
    }
}