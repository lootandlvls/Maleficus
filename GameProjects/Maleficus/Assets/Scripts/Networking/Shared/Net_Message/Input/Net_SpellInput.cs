using System.Collections.Generic;

[System.Serializable]
public class Net_SpellInput : AbstractNetMessage
{
    public EPlayerID PlayerID { get; }
    public EInputButton InputButton { get; }
    public bool IsPressed { get; }

    public Net_SpellInput(EPlayerID playerID, EInputButton inputButton, bool isPressed)
    {
        ID = NetID.SpellInput;
        PlayerID = playerID;
        InputButton = inputButton;
        IsPressed = isPressed;
    }

}