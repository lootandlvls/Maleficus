using MongoDB.Bson;
using System.Collections.Generic;
public class Model_Game
{
    // identification
    public ObjectId _id { set; get; }
    public byte game_mode { set; get; }
    public ObjectId instance_id { set; get; } // instance

    // other values
    public byte game_state { set; get; }
    public List<player> players { set; get; } // {account, ready}

    public struct player
    {
        public ObjectId player_id;
        public byte team_id;
        public bool connected;
    }
}