using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using MongoDB.Bson;
using System.IO;
using static Maleficus.MaleficusUtilities;
using static Maleficus.MaleficusConsts;
using System.Runtime.Serialization;

public class UserManager : AbstractSingletonManager<UserManager>
{
    // accessable variables after deserialization
    public static Local_Account user;
    public static List<Local_Spell> spells;
    public static List<Local_SinglePlayer> singleplayers;
    public static Local_Achievement achievements;

    #region Monobehaviour
    public override void OnSceneStartReinitialize()
    {

    }

    protected override void Awake()
    {
        base.Awake();
        /*CreateLocalData();
        LoadSavedData();
        UpdateSavedAccountData("", "", "", 1);
        LoadSavedAccountData();
        print(user.ToJson());*/
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public static void CreateLocalData()
    {
        CreateLocalUserAccount(false);
    }

    public static void CreateLocalUserAccount(bool forcedCreate)
    {
        if (!File.Exists(Application.persistentDataPath + "/savedAccountData.gd") || forcedCreate)
        {
            user = new Local_Account();
            user.user_name = "player_1";
            user.account_created = new BsonDateTime(System.DateTime.Now);
            user.last_login = new BsonDateTime(System.DateTime.Now);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/savedAccountData.gd");
            bf.Serialize(file, user);
            file.Close();
        }
    }

    public static void LoadSavedData()
    {
        if(user == null)
        {
            LoadSavedAccountData();
        }
    }

    public static void LoadSavedAccountData()
    {
        if (File.Exists(Application.persistentDataPath + "/savedAccountData.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedAccountData.gd", FileMode.Open);
            try
            {
                user = (Local_Account)bf.Deserialize(file);
            }
            catch(SerializationException ex)
            {
                print(ex.Message);
                CreateLocalUserAccount(true);
            }

            file.Close();
        }
    }

    public static void UpdateSavedAccountData(string user_name="", string password="", string email="", byte status=255, int coins=-1, byte level=255, int xp=-1, byte spent_spell_points=255, BsonDateTime account_created=default(BsonDateTime), BsonDateTime last_login=default(BsonDateTime))
    {
        // check if user is already loaded;
        if(user == null)
        {
            LoadSavedAccountData();
        }

        // update values if not null / default
        if(user_name != "")
        {
            user.user_name = user_name;
        }
        if (IsPassword(password))
        {
            user.password = password;
        }
        if (IsEmail(email))
        {
            user.email = email;
        }
        if(status != 255)
        {
            user.status = status;
        }
        if(coins != -1)
        {
            user.coins = coins;
        }
        if(level != 255)
        {
            user.level = level;
        }
        if(xp != -1)
        {
            user.xp = xp;
        }
        if(spent_spell_points != 255)
        {
            user.spent_spell_points = spent_spell_points;
        }
        if(account_created != default(BsonDateTime))
        {
            user.account_created = account_created;
        }
        if(last_login != default(BsonDateTime))
        {
            user.last_login = last_login;
        }

        // actually update the file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedAccountData.gd");
        bf.Serialize(file, user);
        file.Close();
    }
    #endregion
}