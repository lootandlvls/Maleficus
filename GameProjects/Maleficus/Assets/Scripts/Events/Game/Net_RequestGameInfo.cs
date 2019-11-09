[System.Serializable]
public class Net_RequestGameInfo : AbstractNetMessage
{
    public Net_RequestGameInfo()
    {
        MessageType = ENetMessageType.REQUEST_GAME_SESSION_INFO;
    }

    public int lobbyID { set; get; }
}