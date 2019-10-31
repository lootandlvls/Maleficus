using MongoDB.Bson;
using MongoDB.Driver;
public class Model_Friend
{
    // identification
    public ObjectId _id { set; get; }
    public ObjectId asker { set; get; } // account
    public ObjectId possible_friend { set; get; } // account
}