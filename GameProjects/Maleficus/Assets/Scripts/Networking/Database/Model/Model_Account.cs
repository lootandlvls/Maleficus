using MongoDB.Bson;
using System;
public class Model_Account
{
    // identification
    public ObjectId _id { set; get; }
    public string user_name { set; get; }
    public string password { set; get; }
    public string email { set; get; }
    public int main_connection { set; get; }
    public int instance_connection { set; get; }
    public string token { set; get; }
    public ObjectId lobby_id { set; get; } // lobby
    public ObjectId game_id { set; get; } // game

    // other values
    public byte status { set; get; }
    public int coins { set; get; }
    public byte level { set; get; }
    public int xp { set; get; }
    public byte spent_spell_points { set; get; }
    public BsonDateTime account_created { set; get; }
    public BsonDateTime last_login { set; get; }

    public Account GetAccount()
    {
        return new Account() { user_name = this.user_name, level = this.level, status = this.status };
    }
}