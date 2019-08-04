using System.Collections.Generic;

[System.Serializable]
public class Net_SpellInput : NetMsg
{
    public Net_SpellInput()
    {
        OP = NetOP.SpellInput;
    }

    public string Token { set; get; }

    public EInputButton spellId { set; get; }
    public EPlayerID ePlayerID { set; get; }
}