using MongoDB.Bson;
using MongoDB.Driver;
public class Model_Skin
{
    // identification
    public ObjectId _id { set; get; }
    public byte skin_id { set; get; }
    public ObjectId account_id { set; get; } // account

    // other values
    public bool in_use { set; get; }
}