using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Mongo_InstanceManager
{
    private const string MONGO_URI = "mongodb://lnsandn:MongosMitBongos123@lobbydb-shard-00-00-fiwkz.mongodb.net:27017,lobbydb-shard-00-01-fiwkz.mongodb.net:27017,lobbydb-shard-00-02-fiwkz.mongodb.net:27017/test?ssl=true&replicaSet=lobbydb-shard-0&authSource=admin&retryWrites=true";
    private const string DATABASE_NAME = "lobbydb";
    private MongoClient client;
    private IMongoDatabase db;

    private IMongoCollection<Model_Account> accounts;
    private IMongoCollection<Model_Lobby> lobbys;

    // initialize the connection to the database
    public void Init()
    {
        client = new MongoClient(MONGO_URI);
        db = client.GetDatabase(DATABASE_NAME);
        // initialize collections here
        accounts = db.GetCollection<Model_Account>("account");
        lobbys = db.GetCollection<Model_Lobby>("lobby");

        if (db != null)
        {
            Debug.Log("Database has been initialized");
        }
    }

    // set the connection to the database to null
    public void Shutdown()
    {
        client = null;
        db = null;
    }

    #region Fetch

    #region Account

    #endregion

    #region Lobby
    public Model_Lobby FindLobbyByToken(string token)
    {
        return lobbys.Find(u => u.Token == token).FirstOrDefault<Model_Lobby>();
    }
    #endregion

    #endregion
}
