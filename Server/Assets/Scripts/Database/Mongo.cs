using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Mongo
{
    private const string MONGO_URI = "mongodb://lnsandn:MongosMitBongos123@lobbydb-shard-00-00-fiwkz.mongodb.net:27017,lobbydb-shard-00-01-fiwkz.mongodb.net:27017,lobbydb-shard-00-02-fiwkz.mongodb.net:27017/test?ssl=true&replicaSet=lobbydb-shard-0&authSource=admin&retryWrites=true";
    //private const string MONGO_URI = "mongodb://lnsandn:MongosMitBongos123@lobbydb-fiwkz.mongodb.net:27017?ssl=true";
    private const string DATABASE_NAME = "lobbydb";
    private MongoClient client;
    private IMongoDatabase db;

    private IMongoCollection<Model_Account> accounts;
    private IMongoCollection<Model_Follow> follows;

    public void Init() { 
        
        client = new MongoClient(MONGO_URI);
        db = client.GetDatabase(DATABASE_NAME);
        // initialize collections here
        accounts = db.GetCollection<Model_Account>("account");
        follows = db.GetCollection<Model_Follow>("follow");
        
        Debug.Log("Database has been initialized");
    }
    public void Shutdown()
    {
        client = null;
        db = null;
    }

    #region Insert
    public bool InsertAccount(string username, string password, string email)
    {
        // check if email is valid
        if (!Utility.IsEmail(email))
        {
            Debug.Log(email + " is not an email!");
            return false;
        }

        // check if username is valid
        if (!Utility.IsUsername(username))
        {
            Debug.Log(username + " is not a username!");
            return false;
        }
        
        // check if Account already exists
        if (FindAccountByEmail(email) != null)
        {
            Debug.Log(email + " is already being used!");
            return false;
        }

        Model_Account newAccount = new Model_Account();
        newAccount.Username = username;
        newAccount.ShaPassword = password;
        newAccount.Email = email;
        newAccount.Discriminator = "0000";

        // TODO replace discrimintator generate part with increment
        // roll for unique Discriminator 
        int rollCount = 0;
        while(FindAccountByUsernameAndDiscriminator(newAccount.Username, newAccount.Discriminator) != null)
        {
            newAccount.Discriminator = UnityEngine.Random.Range(0, 9999).ToString("0000");

            rollCount++;
            if(rollCount > 1000)
            {
                Debug.Log("We rolled to many times, suggest usernae change!");
                return false;
            }
        }
        accounts.InsertOne(newAccount);
        return true;
    }
    public Model_Account LoginAccount(string usernameOrEmail, string password, int cnnId, string token)
    {
        Model_Account myAccount = null;
        // soll eig leer sein
        // find my account
        if (Utility.IsEmail(usernameOrEmail))
        {
            // if i logged in using an email
            myAccount = accounts.Find(u => u.Email == usernameOrEmail && u.ShaPassword == password).FirstOrDefault<Model_Account>();

            //Todo find way to store filters
            if (myAccount != null)
            {
                // we found the account lets login
                myAccount.ActiveConnection = cnnId;
                myAccount.Token = token;
                myAccount.Status = 1;
                myAccount.LastLogin = System.DateTime.Now;
                //Todo get convert myAccount to bson
                accounts.UpdateOne(u => u.Email == usernameOrEmail && u.ShaPassword == password, Builders<Model_Account>.Update.Set("ActiveConnection", myAccount.ActiveConnection)
                                                                                                          .Set("Token", myAccount.Token)
                                                                                                          .Set("Status", myAccount.Status)
                                                                                                          .Set("LastLogin", myAccount.LastLogin));
            }
            else
            {
                // we found no account
            }
        }
        else
        {
            // if i logged in using an username#discriminator
            if(Utility.IsUsernameAndDiscriminator(usernameOrEmail))
            {
                string[] data = usernameOrEmail.Split('#');
                if (data[1] != null)
                {

                    myAccount = accounts.Find(u => u.Username == data[0] &&
                                                   u.Discriminator == data[1] &&
                                                   u.ShaPassword == password).FirstOrDefault<Model_Account>();

                    if (myAccount != null)
                    {
                        // we found the account lets login
                        myAccount.ActiveConnection = cnnId;
                        myAccount.Token = token;
                        myAccount.Status = 1;
                        myAccount.LastLogin = System.DateTime.Now;
                        accounts.UpdateOne(u => u.Username == data[0] &&
                                                     u.Discriminator == data[1] &&
                                                     u.ShaPassword == password, Builders<Model_Account>.Update.Set("ActiveConnection", myAccount.ActiveConnection)
                                                                                                              .Set("Token", myAccount.Token)
                                                                                                              .Set("Status", myAccount.Status)
                                                                                                              .Set("LastLogin", myAccount.LastLogin));
                    }
                    else
                    {
                        // we found no account
                    }
                }
            }
        }


        return myAccount;
    }
    public bool InsertFollow(string token, string emailOrUsername)
    {
        Model_Follow newFollow = new Model_Follow();
        newFollow.Sender = new Model_Reference("account", FindAccountByToken(token)._id);

        // start by getting the refenrence to our follow

        if (!Utility.IsEmail(emailOrUsername))
        {
            // if its username#discriminator
            string[] data = emailOrUsername.Split('#');
            if (data[1] != null)
            {
                Model_Account follow = FindAccountByUsernameAndDiscriminator(data[0], data[1]);
                if (follow != null)
                {
                    newFollow.Target = new Model_Reference("account", follow._id);
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            // if its email
            Model_Account follow = FindAccountByEmail(emailOrUsername);
            if (follow != null)
            {
                newFollow.Target = new Model_Reference("account", follow._id);
            }
            else
            {
                return false;
            }
        }

        if(newFollow.Target != newFollow.Sender)
        {
            // does the follow already exist
            if(follows.Find(u => u.Sender == newFollow.Sender && u.Target == newFollow.Target).FirstOrDefault<Model_Follow>() == null)
            {               
                // if not create one
                follows.InsertOne(newFollow);
                return true;
            }
        }
        
        return false;
    }
    #endregion

    #region Fetch
    public Model_Account FindAccountByEmail(string email)
    {
        return accounts.Find(u => u.Email == email).FirstOrDefault<Model_Account>();
    }
    public Model_Account FindAccountByUsernameAndDiscriminator(string username, string discriminator)
    {
        return accounts.Find(u => u.Username == username && u.Discriminator == discriminator).FirstOrDefault<Model_Account>();
    }
    public Model_Account FindAccountByToken(string token)
    {
        return accounts.Find(u => u.Token == token).FirstOrDefault<Model_Account>();
    }
    public Model_Account FindAccountByObjectId(ObjectId id)
    {
        return accounts.Find(u => u._id == id).FirstOrDefault<Model_Account>();
    }
    public Model_Account FindAccountByConnectionId(int connectionId)
    {
        return accounts.Find(u => u.ActiveConnection == connectionId).FirstOrDefault<Model_Account>();
    }
    public List<Account> FindAllFollowFrom(string token)
    {
        Model_Reference self = new Model_Reference("account", FindAccountByToken(token)._id);

        List<Account> followsResponse = new List<Account>();
        foreach(var f in follows.Find(f => f.Sender == self).ToList())
        {
            //maybe cast as objectId at f.target.id
            followsResponse.Add(FindAccountByObjectId(f.Target.id).GetAccount());
        }
        return followsResponse;
    }
    public List<Account> FindAllFollowBy(string email)
    {
        Model_Reference self = new Model_Reference("account", FindAccountByEmail(email)._id);

        List<Account> followsResponse = new List<Account>();
        foreach (var f in follows.Find(f => f.Target == self).ToList())
        {
            //maybe cast as objectId at f.target.id
            followsResponse.Add(FindAccountByObjectId(f.Sender.id).GetAccount());
        }
        return followsResponse;
    }
    public Model_Follow FindFollowByUsernameAndDiscriminator(string token, string usernameAndDiscriminator)
    {
        string[] data = usernameAndDiscriminator.Split('#');

        if(data[1] != null)
        {
            var sender = new Model_Reference("account", FindAccountByToken(token)._id);
            var follow = new Model_Reference("account", FindAccountByUsernameAndDiscriminator(data[0], data[1])._id);

            return follows.Find(f => f.Sender == sender && f.Target == follow).FirstOrDefault<Model_Follow>();
        }
        return null;
    }
    #endregion

    #region Update
    public void UpdateAccountAfterDisconnection(string email)
    {
        var account = accounts.Find(a => a.Email == email).FirstOrDefault<Model_Account>();

        account.Token = null;
        account.ActiveConnection = 0;
        account.Status = 0;

        accounts.UpdateOne((a => a.Email == email), Builders<Model_Account>.Update.Set("Token", account.Token)
                                                                                  .Set("ActiveConnection", account.ActiveConnection)
                                                                                  .Set("Status", account.Status));
    }
    #endregion

    #region Delete
    public void RemoveFollow(string token, string usernameDiscriminator)
    {
        ObjectId id = FindFollowByUsernameAndDiscriminator(token, usernameDiscriminator)._id;
        follows.DeleteOne(f => f._id == id);
    }
    #endregion
}