[System.Serializable]
public class Net_CreateAccount : AbstractNetMessage
{
    public Net_CreateAccount()
    {
        ID = ENetMessageID.CREATE_ACCOUNT;
    }
    public string Username { set; get; }
    public string Password { set; get; }
    public string Email { set; get; }
}
