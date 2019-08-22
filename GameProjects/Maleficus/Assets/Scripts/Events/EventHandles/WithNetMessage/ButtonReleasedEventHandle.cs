using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonReleasedEventHandle : AbstractEventHandle
{
    public EPlayerID PlayerID { get; }
    public EInputButton InputButton { get; }

    public ButtonReleasedEventHandle(EPlayerID playerID, EInputButton inputButton)
    {
        PlayerID = playerID;
        InputButton = inputButton;
    }

    public override string GetDebugMessage()
    {
        return PlayerID + " released " + InputButton;
    }

    public override AbstractNetMessage GetNetMessage()
    {
        return new Net_SpellInput(PlayerID, InputButton, false);
    }
}