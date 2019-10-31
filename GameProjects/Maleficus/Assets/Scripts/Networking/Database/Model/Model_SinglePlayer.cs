using MongoDB.Bson;
public class Model_SinglePlayer
{
    // identification
    public ObjectId _id { set; get; }
    public byte level_id { set; get; }
    public ObjectId account_id { set; get; } // account

    // other values
    public bool unlocked { set; get; }
    public bool finished { set; get; }
}