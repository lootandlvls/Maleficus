using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPressedEventHandle : AbstractEventHandle
{
    public EPlayerID PlayerID { get; }
    public EInputButton InputButton { get; }

    public ButtonPressedEventHandle(EPlayerID playerID, EInputButton inputButton)
    {
        PlayerID = playerID;
        InputButton = inputButton;
    }

    public override string GetDebugMessage()
    {
        return PlayerID + " pressed " + InputButton;
    }

    public override AbstractNetMessage GetNetMessage()
    {
        return new Net_SpellInput(PlayerID, InputButton, true);
    }
}
