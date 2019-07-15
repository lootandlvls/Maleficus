using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

public class Model_Lobby
{
    public ObjectId _id { set; get; }

    public string Token { set; get; }

    public EGameMode GameMode { set; get; }

    public List<string> Team1 { set; get; }
    public List<string> Team2 { set; get; }
    public List<string> Team3 { set; get; }
    public List<string> Team4 { set; get; }
}
