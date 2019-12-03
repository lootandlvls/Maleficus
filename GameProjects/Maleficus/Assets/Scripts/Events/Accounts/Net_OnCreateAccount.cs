using MongoDB.Bson;
[System.Serializable]
public class Net_OnCreateAccount : AbstractNetMessage
{
    public Net_OnCreateAccount()
    {
        MessageType = ENetMessageType.ON_CREATE_ACCOUNT;
    }
    public byte success { set; get; }
    
    public string token { set; get; }
    public bool random { set; get; }
    public int main_connection { set; get; }
    public string user_name { set; get; }
    public string password { set; get; }
    public string email { set; get; }
    public BsonDateTime account_created { set; get; }
}