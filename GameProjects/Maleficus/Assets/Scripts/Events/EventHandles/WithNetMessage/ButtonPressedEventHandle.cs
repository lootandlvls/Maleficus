using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonPressedEventHandle : AbstractEventHandle
{
    public EInputButton InputButton { get; }

    public ButtonPressedEventHandle(EClientID senderID, EInputButton inputButton)
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

    //public override AbstractNetMessage GetNetMessage()
    //{
    //    return new Net_SpellInput(PlayerID, InputButton, true);
    //}
}
