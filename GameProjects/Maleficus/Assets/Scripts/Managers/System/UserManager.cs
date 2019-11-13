using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MongoDB.Bson;
using static Maleficus.MaleficusUtilities;
using static Maleficus.MaleficusConsts;

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
        CreateLocalData();
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

        if (!File.Exists(Application.persistentDataPath + "/savedAccountData.gd"))
        {
            user.user_name = "player_1";
            user.account_created = new BsonDateTime(System.DateTime.Now);
            user.last_login = new BsonDateTime(System.DateTime.Now);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/savedAccountData.gd");
            bf.Serialize(file, user);
            file.Close();
            /*
            System.IO.File.WriteAllText(Application.persistentDataPath + "/savedAccountData.gd", 
                user.Aggregate<BsonDocument, string>("",(seed, document)=>seed+document.ToString()+"\n"));*/
        }
        
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedAccountData.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedAccountData.gd", FileMode.Open);
            user = (Local_Account)bf.Deserialize(file);
            file.Close();
        }
    }

    public static void UpdateLocalData()
    {

    }
    #endregion
}