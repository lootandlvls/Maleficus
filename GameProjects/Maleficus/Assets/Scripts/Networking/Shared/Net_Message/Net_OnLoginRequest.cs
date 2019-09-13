[System.Serializable]
public class Net_OnLoginRequest : AbstractNetMessage
{
    public Net_OnLoginRequest()
    {
        ID = ENetMessageID.ON_LOGIN_REQUEST;
    }
    public byte Success { set; get; }
    public string Information { set; get; }

    public int ConnectionId { set; get; }
    public string Username { set; get; }
    public string Discriminator { set; get; }
}
