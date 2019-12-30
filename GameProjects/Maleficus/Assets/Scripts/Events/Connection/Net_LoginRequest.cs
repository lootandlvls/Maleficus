[System.Serializable]
public class Net_LoginRequest : AbstractNetMessage
{
    public Net_LoginRequest()
    {
        MessageType = ENetMessageType.LOGIN_REQUEST;
    }
    public string user_name { set; get; }
    public string password { set; get; }
    public string email { set; get; }
}