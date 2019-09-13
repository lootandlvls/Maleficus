using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonReleasedEventHandle : AbstractEventHandle
{
    public EInputButton InputButton { get; }

    public ButtonReleasedEventHandle(EClientID senderID, EInputButton inputButton)
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

    //public override AbstractNetMessage GetNetMessage()
    //{
    //    return new Net_SpellInput(PlayerID, InputButton, false);
    //}
}