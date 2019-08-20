[System.Serializable]
public class Net_LoginRequest : AbstractNetMessage
{
    public Net_LoginRequest()
    {
        ID = NetID.LoginRequest;
    }
    public string UsernameOrEmail { set; get; }
    public string Password { set; get; }
}
