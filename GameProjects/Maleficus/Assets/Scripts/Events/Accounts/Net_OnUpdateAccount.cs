[System.Serializable]
public class Net_OnUpdateAccount : AbstractNetMessage
{
    public Net_OnUpdateAccount()
    {
        MessageType = ENetMessageType.ON_UPDATE_ACCOUNT;
    }
    public byte success { set; get; }
    public string user_name { set; get; }
    public string password { set; get; }
    public string email { set; get; }
}