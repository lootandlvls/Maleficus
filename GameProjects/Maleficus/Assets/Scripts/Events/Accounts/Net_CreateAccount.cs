[System.Serializable]
public class Net_CreateAccount : AbstractNetMessage
{
    public Net_CreateAccount()
    {
        MessageType = ENetMessageType.CREATE_ACCOUNT;
    }
    public bool random { set; get; }
    public string user_name { set; get; }
    public string password { set; get; }
    public string email { set; get; }
}
