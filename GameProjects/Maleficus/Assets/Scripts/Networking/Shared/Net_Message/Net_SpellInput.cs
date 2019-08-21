using System.Collections.Generic;

[System.Serializable]
public class Net_SpellInput : AbstractNetMessage
{
    public Net_SpellInput()
    {
        ID = NetID.SpellInput;
    }

    public string Token { set; get; }
    public EInputButton spellId { set; get; }
    public EPlayerID ePlayerID { set; get; }
}