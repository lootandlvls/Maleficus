[System.Serializable]
public class Net_RequestGameInfo : AbstractNetMessage
{
    public Net_RequestGameInfo()
    {
        ID = ENetMessageID.REQUEST_GAME_SESSION_INFO;
    }

    public int lobbyID { set; get; }
}