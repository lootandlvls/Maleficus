[System.Serializable]
public class Net_OnCreateAccount : AbstractNetMessage
{
    public Net_OnCreateAccount()
    {
        ID = NetID.OnCreateAccount;
    }
    public byte Success { set; get; }
    public string Information { set; get; }
    
    public int ConnectionId { set; get; }
    public string Username { set; get; }
    public string Discriminator { set; get; }
}