using System.Collections.Generic;

[System.Serializable]
public class Net_OnRequestGameInfo : AbstractNetMessage
{
    public Net_OnRequestGameInfo()
    {
        ID = NetID.OnRequestGameInfo;
    }

    public string Token { set; get; }
    public byte ownPlayerId { set; get; }
    public Account initialiser { set; get; }

    public EGameMode GameMode { set; get; }

    // TODO [Leon]: add array with connected players


    public Account[] Team1 { set; get; }
    public Account[] Team2 { set; get; }
    public Account[] Team3 { set; get; }
    public Account[] Team4 { set; get; }
}