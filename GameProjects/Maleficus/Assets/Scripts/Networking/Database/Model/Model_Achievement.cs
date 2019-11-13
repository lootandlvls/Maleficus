using MongoDB.Bson;
public class Model_Achievement
{
    // identification
    public ObjectId _id { set; get; }
    public ObjectId account_id { set; get; } // account

    // other values
    public int wins { set; get; }
    public int losses { set; get; }
}