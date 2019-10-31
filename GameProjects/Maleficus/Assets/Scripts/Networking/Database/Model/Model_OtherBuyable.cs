using MongoDB.Bson;
using MongoDB.Driver;
public class Model_OtherBuyable
{
    // identification
    public ObjectId _id { set; get; }
    public byte buyable_id { set; get; }
    public ObjectId account_id { set; get; } // account

    // other values
    public bool used { set; get; }
    public bool expired { set; get; }
    public BsonDateTime usage_date { set; get; }
    public BsonDateTime expiration_date { set; get; }
}