[System.Serializable]
public class Net_RequestGameInfo : AbstractNetMessage
{
    public Net_RequestGameInfo()
    {
        ID = NetID.RequestGameInfo;
    }

    public string Token { set; get; }
    public int lobbyID { set; get; }
}