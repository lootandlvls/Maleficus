using MongoDB.Bson;
[System.Serializable]
public class Local_Account
{
    public string user_name { set; get; }
    public string password { set; get; }
    public string email { set; get; }
    public byte status { set; get; }
    public int coins { set; get; }
    public byte level { set; get; }
    public int xp { set; get; }
    public byte spent_spell_points { set; get; }
    public BsonDateTime account_created { set; get; }
    public BsonDateTime last_login { set; get; }
}