[System.Serializable]
public class Net_OnCreateAccount : NetMsg
{
    public Net_OnCreateAccount()
    {
        OP = NetOP.OnCreateAccount;
    }
    public byte Success { set; get; }
    public string Information { set; get; }
    
    public int ConnectionId { set; get; }
    public string Username { set; get; }
    public string Discriminator { set; get; }
}
