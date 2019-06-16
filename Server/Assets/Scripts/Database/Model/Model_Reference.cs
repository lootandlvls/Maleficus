using MongoDB.Bson;

public class Model_Reference
{ 
    public string reference { set; get; }
    public ObjectId id { set; get; }
    public Model_Reference(string reference, ObjectId id)
    {
        this.reference = reference;
        this.id = id;
    }
}