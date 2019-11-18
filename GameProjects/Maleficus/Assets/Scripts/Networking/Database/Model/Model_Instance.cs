using MongoDB.Bson;
using System.Collections.Generic;
public class Model_Instance
{
    // identification
    public ObjectId _id { set; get; }
    public ObjectId game_id { set; get; }
    public ObjectId manager { set; get; }
    public int port { set; get; }

    // other values
    public bool occupied { set; get; }
    public List<instance_participant> instance_participants { set; get; }

    public struct instance_participant
    {
        public ObjectId participant_id; // account
        public byte connection_id;
    }
}