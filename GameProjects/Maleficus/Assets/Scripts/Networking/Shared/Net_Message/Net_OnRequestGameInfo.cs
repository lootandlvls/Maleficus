using System.Collections.Generic;

[System.Serializable]
public class Net_OnRequestGameInfo : AbstractNetMessage
{
    public Net_OnRequestGameInfo()
    {
        ID = ENetMessageID.ON_REQUEST_GAME__SESSION_INFO;
    }

    public byte ownPlayerId { set; get; }
    public Account initialiser { set; get; }

    public EGameMode GameMode { set; get; }

    // TODO [Leon]: add array with connected players


    public Account Player1 { set; get; }
    public Account Player2 { set; get; }
    public Account Player3 { set; get; }
    public Account Player4 { set; get; }
}