[System.Serializable]
public class Net_UpdateAccount : AbstractNetMessage
{
    public Net_UpdateAccount()
    {
        MessageType = ENetMessageType.UPDATE_ACCOUNT;
    }
    public string token { set; get; }
    public bool update_random { set; get; }
    public string old_password { set; get; }
    public string user_name { set; get; }
    public string password { set; get; }
    public string email { set; get; }
}