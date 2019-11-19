using MongoDB.Bson;
using System;
public class Model_InstanceManager
{
    // identification
    public ObjectId _id { set; get; }
    public string ip { set; get; }
    public string region { set; get; }
}