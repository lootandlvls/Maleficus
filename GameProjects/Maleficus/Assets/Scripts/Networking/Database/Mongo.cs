using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.MaleficusUtilities;

public class Mongo
{
    private const string MONGO_URI = "mongodb://lnsandn:MongosMitBongos123@lobbydb-shard-00-00-fiwkz.mongodb.net:27017,lobbydb-shard-00-01-fiwkz.mongodb.net:27017,lobbydb-shard-00-02-fiwkz.mongodb.net:27017/test?ssl=true&replicaSet=lobbydb-shard-0&authSource=admin&retryWrites=true";
    //private const string MONGO_URI = "mongodb://lnsandn:MongosMitBongos123@lobbydb-fiwkz.mongodb.net:27017?ssl=true";
    private const string DATABASE_NAME = "lobbydb";
    private MongoClient client;
    private IMongoDatabase db;


    private IMongoCollection<Model_Account> collection_accounts;
    private IMongoCollection<Model_Friend> collection_friends;
    private IMongoCollection<Model_FriendRequest> collection_friend_requests;
    private IMongoCollection<Model_Spell> collection_spells;
    private IMongoCollection<Model_Skin> collection_skins;
    private IMongoCollection<Model_OtherBuyable> collection_other_buyables;
    private IMongoCollection<Model_Lobby> collection_lobbies;
    private IMongoCollection<Model_Game> collection_games;
    private IMongoCollection<Model_Instance> collection_instances;
    private IMongoCollection<Model_InstanceManager> collection_instance_managers;
    private IMongoCollection<Model_SinglePlayer> collection_single_players;
    private IMongoCollection<Model_DailyMission> collection_daily_missions;

    private IMongoCollection<Model_Lobby> lobbys;
    //private IMongoCollection<Model_Account> accounts;
    private IMongoCollection<Model_Follow> follows;

    // initialize the connection to the database
    public bool Init() { 
        
        client = new MongoClient(MONGO_URI);
        db = client.GetDatabase(DATABASE_NAME);
        // initialize collections here
        collection_accounts = db.GetCollection<Model_Account>("accounts");
        collection_friends = db.GetCollection<Model_Friend>("friends");
        collection_friend_requests = db.GetCollection<Model_FriendRequest>("friend_requests");
        collection_spells = db.GetCollection<Model_Spell>("spells");
        collection_skins = db.GetCollection<Model_Skin>("skins");
        collection_other_buyables = db.GetCollection<Model_OtherBuyable>("other_buyables");
        collection_lobbies = db.GetCollection<Model_Lobby>("lobbies");
        collection_games = db.GetCollection<Model_Game>("games");
        collection_instances = db.GetCollection<Model_Instance>("instances");
        collection_instance_managers = db.GetCollection<Model_InstanceManager>("instance_managers");
        collection_single_players = db.GetCollection<Model_SinglePlayer>("single_players");
        collection_daily_missions = db.GetCollection<Model_DailyMission>("daily_missions");


        //accounts = db.GetCollection<Model_Account>("account");
        //follows = db.GetCollection<Model_Follow>("follow");
        //lobbys = db.GetCollection<Model_Lobby>("lobby");

        if (db != null)
        {
            Debug.Log("Database has been initialized");
            return true;
        }
        return false;
    }

    // set the connection to the database to null
    public void Shutdown()
    {
        client = null;
        db = null;
    }

    #region Fetch
    public Model_Account FindAccount(ObjectId _id=default(ObjectId), string user_name="", string email="", string token="", ObjectId lobby_id=default(ObjectId), ObjectId game_id=default(ObjectId))
    {
        if (_id != default(ObjectId)){
            return collection_accounts.Find(u => u._id == _id).FirstOrDefault<Model_Account>();
        }else if (user_name != "")
        {
            return collection_accounts.Find(u => u.user_name == user_name).FirstOrDefault<Model_Account>();
        }else if(email != "")
        {
            return collection_accounts.Find(u => u.email == email).FirstOrDefault<Model_Account>();
        }else if(token != "")
        {
            return collection_accounts.Find(u => u.token == token).FirstOrDefault<Model_Account>();
        }else if(lobby_id != default(ObjectId))
        {
            return collection_accounts.Find(u => u.lobby_id == lobby_id).FirstOrDefault<Model_Account>();
        }else if(game_id != default(ObjectId))
        {
            return collection_accounts.Find(u => u.game_id == game_id).FirstOrDefault<Model_Account>();
        }
        Debug.Log("FindAccount: No parameter given!");
        return null;
    }

    public List<Model_Friend> FindFriends(ObjectId friend=default(ObjectId))
    {
        if(friend != default(ObjectId))
        {
            return collection_friends.Find(u => u.friend_one == friend || u.friend_two == friend).ToList<Model_Friend>();
        }
        Debug.Log("FindFriends: No parameter given!");
        return null;
    }

    public List<Model_FriendRequest> FindFriendRequests(ObjectId asker=default(ObjectId), ObjectId possible_friend=default(ObjectId))
    {
        if(asker != default(ObjectId))
        {
            return collection_friend_requests.Find(u => u.asker == asker).ToList<Model_FriendRequest>();
        }else if(possible_friend != default(ObjectId))
        {
            return collection_friend_requests.Find(u => u.possible_friend == possible_friend).ToList<Model_FriendRequest>();
        }
        Debug.Log("FindFriendRequests: No parameter given!");
        return null;
    }

    public List<Model_Spell> FindSpells(ObjectId account_id=default(ObjectId))
    {
        if(account_id != default(ObjectId))
        {
            return collection_spells.Find(u => u.account_id == account_id).ToList<Model_Spell>();
        }

        Debug.Log("FindSpells: No parameter given!");
        return null;
    }

    public List<Model_Skin> FindSkins(ObjectId account_id=default(ObjectId))
    {
        if(account_id != default(ObjectId))
        {
            return collection_skins.Find(u => u.account_id == account_id).ToList<Model_Skin>();
        }

        Debug.Log("FindSkins: No parameter given!");
        return null;
    }

    public List<Model_OtherBuyable> FindOtherBuyables(ObjectId account_id = default(ObjectId))
    {
        if(account_id != default(ObjectId))
        {
            return collection_other_buyables.Find(u => u.account_id == account_id).ToList<Model_OtherBuyable>();
        }
        Debug.Log("FindOtherBuyables: No parameter given!");
        return null;
    }

    public Model_Lobby FindLobby(ObjectId _id = default(ObjectId), ObjectId initialiser=default(ObjectId), ObjectId participant=default(ObjectId))
    {
        if (_id != default(ObjectId))
        {
            return collection_lobbies.Find(u => u._id == _id).FirstOrDefault<Model_Lobby>();
        }else if(initialiser != default(ObjectId))
        {
            return collection_lobbies.Find(u => u.initialiser == initialiser).FirstOrDefault<Model_Lobby>();
        }else if(participant != default(ObjectId)){
            List<Model_Lobby> lobbies = collection_lobbies.Find(u => true).ToList<Model_Lobby>();
            foreach(var lobby in lobbies)
            {
                foreach(var lobby_participant in lobby.participants)
                {
                    if(lobby_participant.participant_id == participant)
                    {
                        return lobby;
                    }
                }
            }
        }
        Debug.Log("FindLobbies: No parameter given!");
        return null;
    }

    public Model_Game FindGame(ObjectId _id = default(ObjectId), ObjectId instance_id = default(ObjectId), ObjectId player = default(ObjectId))
    {
        if (_id != default(ObjectId))
        {
            return collection_games.Find(u => u._id == _id).FirstOrDefault<Model_Game>();
        }
        else if (instance_id != default(ObjectId))
        {
            return collection_games.Find(u => u.instance_id == instance_id).FirstOrDefault<Model_Game>();
        }
        else if (player != default(ObjectId))
        {
            List<Model_Game> games = collection_games.Find(u => true).ToList<Model_Game>();
            foreach (var game in games)
            {
                foreach (var game_player in game.players)
                {
                    if (game_player.player_id == player)
                    {
                        return game;
                    }
                }
            }
        }
        Debug.Log("FindGames: No parameter given!");
        return null;
    }

    public Model_Instance FindInstance(ObjectId _id = default(ObjectId), ObjectId game_id = default(ObjectId), ObjectId manager = default(ObjectId), ObjectId participant = default(ObjectId))
    {
        if (_id != default(ObjectId))
        {
            return collection_instances.Find(u => u._id == _id).FirstOrDefault<Model_Instance>();
        }
        else if (game_id != default(ObjectId))
        {
            return collection_instances.Find(u => u.game_id == game_id).FirstOrDefault<Model_Instance>();
        }
        else if(manager != default(ObjectId))
        {
            return collection_instances.Find(u => u.manager == manager).FirstOrDefault<Model_Instance>();
        }
        else if (participant != default(ObjectId))
        {
            List<Model_Instance> instances = collection_instances.Find(u => true).ToList<Model_Instance>();
            foreach (var instance in instances)
            {
                foreach (var instance_participant in instance.instance_participants)
                {
                    if (instance_participant.participant_id == participant)
                    {
                        return instance;
                    }
                }
            }
        }
        Debug.Log("FindInstance: No parameter given!");
        return null;
    }

    public Model_InstanceManager FindInstanceManager(ObjectId _id = default(ObjectId), string ip = "", string region = "")
    {
        if (_id != default(ObjectId))
        {
            return collection_instance_managers.Find(u => u._id == _id).FirstOrDefault<Model_InstanceManager>();
        }
        else if (ip != "")
        {
            return collection_instance_managers.Find(u => u.ip == ip).FirstOrDefault<Model_InstanceManager>();
        }
        else if (region != "")
        {
            return collection_instance_managers.Find(u => u.region == region).FirstOrDefault<Model_InstanceManager>();
        }
        Debug.Log("FindInstanceManager: No parameter given!");
        return null;
    }

    public List<Model_SinglePlayer> FindSinglePlayers(ObjectId account_id = default(ObjectId))
    {
        if (account_id != default(ObjectId))
        {
            return collection_single_players.Find(u => u.account_id == account_id).ToList<Model_SinglePlayer>();
        }

        Debug.Log("FindSinglePlayers: No parameter given!");
        return null;
    }

    public List<Model_DailyMission> FindDailyMissions(ObjectId account_id = default(ObjectId))
    {
        if (account_id != default(ObjectId))
        {
            return collection_daily_missions.Find(u => u.account_id == account_id).ToList<Model_DailyMission>();
        }

        Debug.Log("FindDailyMissions: No parameter given!");
        return null;
    }
    #endregion

    #region Insert
    public bool InsertAccount()
    {
        /*
        Model_Account new_account = new Model_Account();
        new_account.password = "" + UnityEngine.Random.Range(0, 99999).ToString("00000");
        new_account.user_name = "player";
        List<Model_Account> users_with_standard_name = new List<Model_Account>();
        users_with_standard_name = collection_accounts.Find ToList<Model_Account>();


        accounts.InsertOne(newAccount);*/
        return true;
    }
    #endregion

    #region Update
    public bool UpdateAccount(ObjectId _id, string user_name = "", string password = "", string email = "", int main_connection = -1, int instance_connection = -1, string token = "", ObjectId lobby_id = default(ObjectId), ObjectId game_id = default(ObjectId), byte status = 255, int coins = -1, byte level = 255, int xp = -1, byte spent_spell_points = 255, BsonDateTime last_login = default(BsonDateTime))
    {
        // check if user_name, password, email are valid
        if (user_name != "" && !IsUsername(user_name))
        {
            Debug.Log("User name not valid");
            return false;
        }
        if (password != "" && !IsPassword(password))
        {
            Debug.Log("User name not valid");
            return false;
        }
        if (email != "" && !IsEmail(email))
        {
            Debug.Log("User name not valid");
            return false;
        }

        // update by the given values
        if (password != "")
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("password", password));
        }

        if (user_name != "")
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("user_name", user_name));
        }
        if (password != "")
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("password", password));
        }
        if (main_connection != -1)
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("main_connection", main_connection));
        }
        if (instance_connection != -1)
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("instance_connection", instance_connection));
        }
        if (token != "")
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("token", token));
        }
        if (lobby_id != default(ObjectId))
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("lobby_id", lobby_id));
        }
        if (game_id != default(ObjectId))
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("game_id", game_id));
        }
        if (status != 255)
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("status", status));
        }
        if (coins != -1)
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("coins", coins));
        }
        if (level != 255)
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("level", level));
        }
        if (xp != -1)
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("xp", xp));
        }
        if (spent_spell_points != 255)
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("spent_spell_points", spent_spell_points));
        }
        if (last_login != default(ObjectId))
        {
            collection_accounts.UpdateOne(u => u._id == _id || u.user_name == user_name, Builders<Model_Account>.Update.Set("last_login", last_login));
        }
        return true;
    }
    public void UpdateSpell(ObjectId _id, byte spell_id, bool selected, ObjectId account_id=default(ObjectId), byte spell_level=255, int spell_xp = -1)
    {
        // update by the given values
        collection_spells.UpdateOne(u => (u._id == _id || u.account_id == account_id) && u.spell_id == spell_id, Builders<Model_Spell>.Update.Set("selected", selected));

        if (spell_level != 255)
        {
            collection_spells.UpdateOne(u => (u._id == _id || u.account_id == account_id) && u.spell_id == spell_id, Builders<Model_Spell>.Update.Set("spell_level", spell_level));
        }
        if (spell_xp != -1)
        {
            collection_spells.UpdateOne(u => (u._id == _id || u.account_id == account_id) && u.spell_id == spell_id, Builders<Model_Spell>.Update.Set("spell_xp", spell_xp));
        }

    }
    public void UpdateSkin(ObjectId _id, byte skin_id, ObjectId account_id, bool in_use)
    {
        // update by the given values
        collection_skins.UpdateOne(u => (u._id == _id || u.account_id == account_id) && u.skin_id == skin_id, Builders<Model_Skin>.Update.Set("in_use", in_use));

    }
    public void UpdateOtherBuyable(ObjectId _id, byte buyable_id, ObjectId account_id, bool used, bool expired, BsonDateTime usage_date=default(BsonDateTime), BsonDateTime expiration_date=default(BsonDateTime))
    {
        // update by the given values
        collection_other_buyables.UpdateOne(u => (u._id == _id || u.account_id == account_id) && u.buyable_id == buyable_id, Builders<Model_OtherBuyable>.Update.Set("used", used)
                                                                                                                                                                .Set("expired", expired));
        if(usage_date != default(BsonDateTime))
        {
            collection_other_buyables.UpdateOne(u => (u._id == _id || u.account_id == account_id) && u.buyable_id == buyable_id, Builders<Model_OtherBuyable>.Update.Set("usage_date", usage_date));
        }
        if (expiration_date != default(BsonDateTime))
        {
            collection_other_buyables.UpdateOne(u => (u._id == _id || u.account_id == account_id) && u.buyable_id == buyable_id, Builders<Model_OtherBuyable>.Update.Set("expiration_date", expiration_date));
        }

    }
    public void UpdateLobby(ObjectId _id, ObjectId initialiser=default(ObjectId), List<Model_Lobby.participant> participants=null, byte game_mode=255, int queue=-1)
    {
        // update by the given values
        if (participants != null)
        {
            collection_lobbies.UpdateOne(u => u._id == _id || u.initialiser == initialiser, Builders<Model_Lobby>.Update.Set("participants", participants));
        }
        if (game_mode != 255)
        {
            collection_lobbies.UpdateOne(u => u._id == _id || u.initialiser == initialiser, Builders<Model_Lobby>.Update.Set("game_mode", game_mode));
        }
        if(queue != -1)
        {
            collection_lobbies.UpdateOne(u => u._id == _id || u.initialiser == initialiser, Builders<Model_Lobby>.Update.Set("queue", queue));
        }

    }
    public void UpdateGame(ObjectId _id, byte game_mode=255, ObjectId instance_id = default(ObjectId), byte game_state=255, List<Model_Game.player> players=null)
    {
        // update by the given values
        if (game_mode != 255)
        {
            collection_games.UpdateOne(u => u._id == _id || u.instance_id == instance_id, Builders<Model_Game>.Update.Set("game_mode", game_mode));
        }
        if (game_state != 255)
        {
            collection_games.UpdateOne(u => u._id == _id || u.instance_id == instance_id, Builders<Model_Game>.Update.Set("game_state", game_state));
        }
        if (players != null)
        {
            collection_games.UpdateOne(u => u._id == _id || u.instance_id == instance_id, Builders<Model_Game>.Update.Set("players", players));
        }

    }
    public void UpdateInstance(ObjectId _id, bool occupied, ObjectId game_id, ObjectId manager = default(ObjectId), int port = -1, List<Model_Instance.instance_participant> instance_participants = null)
    {
        // update by the given values
        if (manager != default(ObjectId))
        {
            collection_instances.UpdateOne(u => u._id == _id || u.game_id == game_id, Builders<Model_Instance>.Update.Set("manager", manager));
        }
        if(port != -1)
        {
            collection_instances.UpdateOne(u => u._id == _id || u.game_id == game_id, Builders<Model_Instance>.Update.Set("port", port));
        }
        if(instance_participants != null)
        {
            collection_instances.UpdateOne(u => u._id == _id || u.game_id == game_id, Builders<Model_Instance>.Update.Set("instance_participants", instance_participants));
        }
        collection_instances.UpdateOne(u => u._id == _id || u.game_id == game_id, Builders<Model_Instance>.Update.Set("occupied", occupied)
                                                                                                                    .Set("game_id", game_id));

    }
    public void UpdateInstanceManager(ObjectId _id, string ip="", string region="")
    {
        // update by the given values
        if (ip != "")
        {
            collection_instance_managers.UpdateOne(u => u._id == _id, Builders<Model_InstanceManager>.Update.Set("ip", ip));
        }
        if(region != "")
        {
            collection_instance_managers.UpdateOne(u => u._id == _id, Builders<Model_InstanceManager>.Update.Set("region", region));
        }
    }
    public void UpdateSinglePlayer(ObjectId _id, byte level_id, bool unlocked, bool finished, ObjectId account_id = default(ObjectId))
    {
        // update by the given values
        collection_single_players.UpdateOne(u => (u._id == _id || u.account_id == account_id) && u.level_id == level_id, Builders<Model_SinglePlayer>.Update.Set("unlocked", unlocked)
                                                                                                                                                            .Set("finished", finished));

    }
    public void UpdateDailyMission(ObjectId _id, byte mission_id, bool finished, ObjectId account_id = default(ObjectId), BsonDateTime start=default(BsonDateTime), BsonDateTime end=default(BsonDateTime))
    {
        // update by the given values
        collection_daily_missions.UpdateOne(u => (u._id == _id || u.account_id == account_id) && u.mission_id == mission_id, Builders<Model_DailyMission>.Update.Set("finished", finished));

    }
    #endregion

    /*
        #region Insert
        public bool InsertAccount(string username, string password, string email)
        {
            // check if email is valid
            if (!IsEmail(email))
            {
                Debug.Log(email + " is not an email!");
                return false;
            }

            // check if username is valid
            if (!IsUsername(username))
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
                    Debug.Log("We rolled to many times, suggest username change!");
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
            if (IsEmail(usernameOrEmail))
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
                if(IsUsernameAndDiscriminator(usernameOrEmail))
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

            if (!IsEmail(emailOrUsername))
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

        public int InitLobby(ObjectId initialiserId)
        {
            Model_Account self = FindAccountByObjectId(initialiserId);
            if (self != null)
            {
                Model_Lobby newLobby = new Model_Lobby();
                int lobbyID = (int) lobbys.CountDocuments(u => u.LobbyID != -1);
                newLobby.LobbyID = lobbyID;
                newLobby.initialiserId = initialiserId;
                newLobby.Team1 = new List<ObjectId>();
                newLobby.Team1.Add(initialiserId);

                // add friends to game lobby
                //Todo replace with inviting to lobby
                List<Model_Account> yourFriends = FindAllFollowFromGetModelAccount(self.Token);

                int friendListSize = yourFriends.Count;

                if (friendListSize >= 1)
                {
                    newLobby.Team2 = new List<ObjectId>();
                    newLobby.Team2.Add(yourFriends[0]._id);
                }

                if(friendListSize >= 2)
                {
                    newLobby.Team3 = new List<ObjectId>();
                    newLobby.Team3.Add(yourFriends[1]._id);
                }

                if (friendListSize >= 3)
                {
                    newLobby.Team4 = new List<ObjectId>();
                    newLobby.Team4.Add(yourFriends[2]._id);
                }

                // insert lobby into mongodb
                lobbys.InsertOne(newLobby);
                return lobbyID;
            }

            return -1;
        }
        #endregion

        #region Lobby
        public Model_Lobby FindLobbyByObjectId(ObjectId objectId)
        {
            return lobbys.Find(u => u._id == objectId).FirstOrDefault<Model_Lobby>();
        }
        public Model_Lobby FindLobbyByLobbyID(int lobbyID)
        {
            return lobbys.Find(u => u.LobbyID == lobbyID).FirstOrDefault<Model_Lobby>();
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

        public void UpdateAccountInLobby(ObjectId accountid, ObjectId lobbyId)
        {
            accounts.UpdateOne((a => a._id == accountid), Builders<Model_Account>.Update.Set("inLobby", lobbyId));
        }
        #endregion

        #region Delete
        public void RemoveFollow(string token, string usernameDiscriminator)
        {
            ObjectId id = FindFollowByUsernameAndDiscriminator(token, usernameDiscriminator)._id;
            follows.DeleteOne(f => f._id == id);
        }
        #endregion
        */
}