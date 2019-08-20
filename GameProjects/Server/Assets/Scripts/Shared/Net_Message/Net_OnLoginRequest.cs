[System.Serializable]
public class Net_OnLoginRequest : AbstractNetMessage
{
    public Net_OnLoginRequest()
    {
        ID = NetID.OnLoginRequest;
    }
    public byte Success { set; get; }
    public string Information { set; get; }

    public int ConnectionId { set; get; }
    public string Username { set; get; }
    public string Discriminator { set; get; }
    public string Token { set; get; }
}
