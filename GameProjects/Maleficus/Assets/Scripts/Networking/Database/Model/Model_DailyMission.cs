using MongoDB.Bson;
using MongoDB.Driver;
public class Model_DailyMission
{
    // identification
    public ObjectId _id { set; get; }
    public byte mission_id { set; get; }
    public ObjectId account_id { set; get; } // account

    // other values
    public bool finished { set; get; }
    public BsonDateTime start { set; get; }
    public BsonDateTime end { set; get; }
}