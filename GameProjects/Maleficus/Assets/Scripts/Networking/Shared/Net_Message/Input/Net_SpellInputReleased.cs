using System.Collections.Generic;

[System.Serializable]
public class Net_SpellInputReleased : AbstractNetMessage
{
    public EPlayerID PlayerID { get; }
    public EInputButton InputButton { get; }

    public Net_SpellInputReleased(EPlayerID playerID, EInputButton inputButton)
    {
        ID = NetID.SpellInput;
        PlayerID = playerID;
        InputButton = inputButton;
    }
}