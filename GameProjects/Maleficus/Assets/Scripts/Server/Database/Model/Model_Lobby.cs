using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

public class Model_Lobby
{
    public ObjectId _id { set; get; }

    public int LobbyID { set; get; }

    public ObjectId initialiserId { set; get; }

    public EGameMode GameMode { set; get; }

    public List<ObjectId> Team1 { set; get; }
    public List<ObjectId> Team2 { set; get; }
    public List<ObjectId> Team3 { set; get; }
    public List<ObjectId> Team4 { set; get; }
}
