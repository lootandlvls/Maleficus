using System.Collections.Generic;

[System.Serializable]
public class Net_SpellInputPressed : AbstractNetMessage
{
    public EPlayerID PlayerID { get; }
    public EInputButton InputButton { get; }

    public Net_SpellInputPressed(EPlayerID playerID, EInputButton inputButton)
    {
        ID = NetID.SpellInput;
        PlayerID = playerID;
        InputButton = inputButton;
    }

}