using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.Utils;

[System.Serializable]
public class NetEvent_ButtonPressed : AbstractEventHandle
{
    public EControllerID ControllerID { get; }
    public EInputButton InputButton { get; }

    public NetEvent_ButtonPressed(EClientID senderID, EControllerID controllerID, EInputButton inputButton)
    {
        MessageType = ENetMessageType.BUTTON_PRESSED;
        SenderID = senderID;

        ControllerID = controllerID;
        InputButton = inputButton;
    }

    public override string GetDebugMessage()
    {
        EPlayerID playerID = GetPlayerIDFrom(SenderID);
        return ControllerID + " pressed " + InputButton + " as : " + playerID;
    }
}
