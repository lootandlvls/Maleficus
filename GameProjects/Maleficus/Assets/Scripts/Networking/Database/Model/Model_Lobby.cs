using MongoDB.Bson;
using System.Collections.Generic;
public class Model_Lobby
{
    // identification
    public ObjectId _id { set; get; }
    public ObjectId initialiser { set; get; } // account

    // other values
    public List<participant> participants { set; get; }
    public byte game_mode { set; get; }
    public int queue { set; get; }

    public struct participant
    {
        public ObjectId participant_id; // account
        public bool ready;
    }
}