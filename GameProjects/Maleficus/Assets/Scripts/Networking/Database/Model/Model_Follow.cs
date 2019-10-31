using MongoDB.Bson;
using MongoDB.Driver;
public class Model_Follow
{
    public ObjectId _id { set; get; }

    public Model_Reference Sender { set; get; }
    public Model_Reference Target { set; get; }
}


