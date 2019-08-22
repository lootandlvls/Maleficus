using System.Collections.Generic;

[System.Serializable]
public class Net_SpellInput : AbstractNetMessage
{
    public EInputButton InputButton { get; }
    public EPlayerID PlayerID { get; }
    public bool IsPressed { get; }  // true : Pressed | false : Released

    public Net_SpellInput(EInputButton inputButton, EPlayerID playerID, bool isPressed)
    {
        ID = NetID.SpellInput;
        InputButton = inputButton;
        PlayerID = playerID;
        IsPressed = IsPressed;
    }
}