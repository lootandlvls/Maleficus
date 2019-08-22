using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInputEventHandle : AbstractEventHandle
{
    public EInputButton InputButton { get; }
    public EPlayerID PlayerID { get; }
    public bool IsPressed { get; }  // true : Pressed | false : Released

    public SpellInputEventHandle(EInputButton inputButton, EPlayerID playerID, bool isPressed)
    {
        InputButton = inputButton;
        PlayerID = playerID;
        IsPressed = IsPressed;
    }

    public override string GetDebugMessage()
    {
        return PlayerID + " - " + InputButton + " - IsPressed : " + IsPressed;
    }

    public override AbstractNetMessage GetNetMessage()
    {
        return new Net_SpellInput(InputButton, PlayerID, IsPressed);
    }
}
