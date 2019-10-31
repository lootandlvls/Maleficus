using MongoDB.Bson;
using System.Collections.Generic;
public class Model_Instance
{
    // identification
    public ObjectId _id { set; get; }
    public ObjectId game_id { set; get; }
    public string server_ip { set; get; }
    public int port { set; get; }

    // other values
    public bool occupied { set; get; }
    public List<participant> participants { set; get; }

    public struct participant
    {
        public ObjectId participant_id; // account
        public byte connection_id;
    }
}