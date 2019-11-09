[System.Serializable]
public class Net_LoginRequest : AbstractNetMessage
{
    public Net_LoginRequest()
    {
        MessageType = ENetMessageType.LOGIN_REQUEST;
    }
    public string UsernameOrEmail { set; get; }
    public string Password { set; get; }
}