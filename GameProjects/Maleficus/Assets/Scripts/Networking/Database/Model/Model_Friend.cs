using MongoDB.Bson;
using MongoDB.Driver;
public class Model_Friend
{
    // identification
    public ObjectId _id { set; get; }
    public ObjectId friend_one { set; get; } // account
    public ObjectId friend_two { set; get; } // account
}