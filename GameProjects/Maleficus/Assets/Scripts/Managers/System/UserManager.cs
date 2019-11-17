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
        //CreateLocalData();
        //LoadSavedData();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    #endregion

    #region Create
    public static void CreateLocalData()
    {
        CreateLocalUserAccount(false);
        CreateLocalSpells(false);
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
            FileStream file = null;
            try
            {
                file = File.Create(Application.persistentDataPath + "/savedAccountData.gd");
            }
            catch(IOException e)
            {
                print(e.Message);
                File.Delete(Application.persistentDataPath + "/savedAccountData.gd");
                file = File.Create(Application.persistentDataPath + "/savedAccountData.gd");
            }
            if(file != null)
            {
                bf.Serialize(file, user);
                file.Close();
            }
        }
    }

    public static void CreateLocalSpells(bool forcedCreate)
    {
        if(!File.Exists(Application.persistentDataPath + "/savedSpells.gd") || forcedCreate)
        {
            FileStream file = null;
            try
            {
                file = File.Create(Application.persistentDataPath + "/savedSpells.gd");
            }
            catch(IOException e)
            {
                print(e.Message);
                File.Delete(Application.persistentDataPath + "/savedSpells.gd");
                file = File.Create(Application.persistentDataPath + "/savedSpells.gd");
            }
            if(file != null)
            {
                file.Close();
            }
        }
    }
    #endregion

    #region Load

    public static void LoadSavedData()
    {
        if(user == null)
        {
            LoadSavedAccount();
        }
        if(spells == null)
        {
            LoadSavedSpells();
        }
    }

    public static void LoadSavedAccount()
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
                file.Close();
                CreateLocalUserAccount(true);
                return;
            }

            file.Close();
        }
    }

    public static void LoadSavedSpells()
    {
        if (File.Exists(Application.persistentDataPath + "/savedSpells.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedSpells.gd", FileMode.Open);
            try
            {
                spells = (List<Local_Spell>)bf.Deserialize(file);
            }
            catch (SerializationException ex)
            {
                print(ex.Message);
                file.Close();
                CreateLocalSpells(true);
                return;
            }

            file.Close();
        }
    }

    #endregion

    #region Update

    public static void UpdateSavedAccountData(string user_name="", string password="", string email="", byte status=255, int coins=-1, byte level=255, int xp=-1, byte spent_spell_points=255, BsonDateTime account_created=default(BsonDateTime), BsonDateTime last_login=default(BsonDateTime))
    {
        // check if user is already loaded;
        if(user == null)
        {
            LoadSavedAccount();
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
        FileStream file = null;
        try
        {
            file = File.Create(Application.persistentDataPath + "/savedAccountData.gd");
        }
        catch(IOException e)
        {
            print(e.Message);
            bf.Serialize(file, user);
            file.Close();
            return;
        }

        bf.Serialize(file, user);
        file.Close();
    }

    public static void UpdateSavedSpells(bool new_spell, byte spell_id=255, bool selected = false, byte spell_level=255, int spell_xp=-1)
    {
        // check if spells are already loaded;
        if (spells == null)
        {
            LoadSavedSpells();
        }

        // update values if not null / default

        if (new_spell)
        {
            Local_Spell spell = new Local_Spell();
            spell.selected = selected;
            if (spell_id != 255)
            {
                spell.spell_id = spell_id;
            }
            if(spell_level != 255)
            {
                spell.spell_level = spell_level;
            }
            if(spell_xp != -1)
            {
                spell.spell_xp = spell_xp;
            }
            spells.Add(spell);
        }
        else
        {
            spells
        }

        // actually update the file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = null;
        try
        {
            file = File.Create(Application.persistentDataPath + "/savedSpells.gd");
        }
        catch (IOException e)
        {
            print(e.Message);
            bf.Serialize(file, spells);
            file.Close();
            return;
        }

        bf.Serialize(file, spells);
        file.Close();
    }
    #endregion
}