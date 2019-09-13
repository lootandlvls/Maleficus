[System.Serializable]
public class Net_OnCreateAccount : AbstractNetMessage
{
    public Net_OnCreateAccount()
    {
        ID = ENetMessageID.ON_CREATE_ACCOUNT;
    }
    public byte Success { set; get; }
    public string Information { set; get; }
    
    public int ConnectionId { set; get; }
    public string Username { set; get; }
    public string Discriminator { set; get; }
}