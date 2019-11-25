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
    public static List<Local_Spell> saved_spells;
    public static List<Local_SinglePlayer> singleplayers;
    public static Local_Achievement achievements;

    #region Monobehaviour
    public override void OnSceneStartReinitialize()
    {

    }

    protected override void Awake()
    {
        base.Awake();
        LoadSavedData();
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
        CreateLocalSinglePlayers(false);
        CreateLocalAchievements(false);
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
            saved_spells = new List<Local_Spell>();
            BinaryFormatter bf = new BinaryFormatter();
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
                bf.Serialize(file, saved_spells);
                file.Close();
            }
        }
    }

    public static void CreateLocalSinglePlayers(bool forcedCreate)
    {
        if (!File.Exists(Application.persistentDataPath + "/savedSinglePlayers.gd") || forcedCreate)
        {
            singleplayers = new List<Local_SinglePlayer>();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = null;
            try
            {
                file = File.Create(Application.persistentDataPath + "/savedSinglePlayers.gd");
            }
            catch (IOException e)
            {
                print(e.Message);
                File.Delete(Application.persistentDataPath + "/savedSinglePlayers.gd");
                file = File.Create(Application.persistentDataPath + "/savedSinglePlayers.gd");
            }
            if (file != null)
            {
                bf.Serialize(file, singleplayers);
                file.Close();
            }
        }
    }

    public static void CreateLocalAchievements(bool forcedCreate)
    {
        if (!File.Exists(Application.persistentDataPath + "/savedAchievements.gd") || forcedCreate)
        {
            Local_Achievement achievement = new Local_Achievement();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = null;
            try
            {
                file = File.Create(Application.persistentDataPath + "/savedAchievements.gd");
            }
            catch (IOException e)
            {
                print(e.Message);
                File.Delete(Application.persistentDataPath + "/savedAchievements.gd");
                file = File.Create(Application.persistentDataPath + "/savedAchievements.gd");
            }
            if (file != null)
            {
                bf.Serialize(file, achievement);
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
        if(saved_spells == null)
        {
            LoadSavedSpells();
        }
        if(singleplayers == null)
        {
            LoadSavedSinglePlayers();
        }
        if(achievements == null)
        {
            LoadSavedAchievements();
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
                saved_spells = (List<Local_Spell>)bf.Deserialize(file);
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

    public static void LoadSavedSinglePlayers()
    {
        if (File.Exists(Application.persistentDataPath + "/savedSinglePlayers.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedSinglePlayers.gd", FileMode.Open);
            try
            {
                singleplayers = (List<Local_SinglePlayer>)bf.Deserialize(file);
            }
            catch (SerializationException ex)
            {
                print(ex.Message);
                file.Close();
                CreateLocalSinglePlayers(true);
                return;
            }

            file.Close();
        }
    }

    public static void LoadSavedAchievements()
    {
        if (File.Exists(Application.persistentDataPath + "/savedAchievements.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedAchievements.gd", FileMode.Open);
            try
            {
                achievements = (Local_Achievement)bf.Deserialize(file);
            }
            catch (SerializationException ex)
            {
                print(ex.Message);
                file.Close();
                CreateLocalAchievements(true);
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
            file.Close();
            return;
        }

        bf.Serialize(file, user);
        file.Close();
    }

    public static void UpdateSavedSpells(bool new_spell, bool selected, byte spell_id, byte spell_level=255, int spell_xp=-1)
    {
        // check if spells are already loaded;
        if (saved_spells == null)
        {
            LoadSavedSpells();
        }

        // update values if not null / default
        if (new_spell)
        {
            foreach(Local_Spell local_spell in saved_spells)
            {
                if(local_spell.spell_id == spell_id)
                {
                    Debug.Log("Tried to add a already existing Spell!");
                    return;
                }
            }
            Local_Spell spell = new Local_Spell();
            spell.selected = selected;
            spell.spell_id = spell_id;
            if(spell_level != 255)
            {
                spell.spell_level = spell_level;
            }
            if(spell_xp != -1)
            {
                spell.spell_xp = spell_xp;
            }
            saved_spells.Add(spell);
        }
        else
        {
            // search spell via spell_id
            foreach(Local_Spell spell in saved_spells)
            {
                if (spell.spell_id == spell_id)
                {
                    spell.selected = selected;
                    if(spell_level != 255)
                    {
                        spell.spell_level = spell_level;
                    }
                    if(spell_xp != -1)
                    {
                        spell.spell_xp = spell_xp;
                    }
                }
            }

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
            file.Close();
            return;
        }

        bf.Serialize(file, saved_spells);
        file.Close();
    }

    public static void UpdateSavedSinglePlayers(bool new_single_player, byte level_id, bool unlocked, bool finished)
    {
        // check if spells are already loaded;
        if (singleplayers == null)
        {
            LoadSavedSinglePlayers();
        }

        // update values if not null / default
        if (new_single_player)
        {
            foreach (Local_SinglePlayer local_single_player in singleplayers)
            {
                if (local_single_player.level_id == level_id)
                {
                    Debug.Log("Tried to add a already existing Level!");
                    return;
                }
            }
            Local_SinglePlayer singleplayer = new Local_SinglePlayer();
            singleplayer.level_id = level_id;
            singleplayer.unlocked = unlocked;
            singleplayer.finished = finished;
            singleplayers.Add(singleplayer);
        }
        else
        {
            // search spell via spell_id
            foreach (Local_SinglePlayer single_player in singleplayers)
            {
                if (single_player.level_id == level_id)
                {
                    single_player.unlocked = unlocked;
                    single_player.finished = finished;
                }
            }

        }

        // actually update the file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = null;
        try
        {
            file = File.Create(Application.persistentDataPath + "/savedSinglePlayers.gd");
        }
        catch (IOException e)
        {
            print(e.Message);
            file.Close();
            return;
        }

        bf.Serialize(file, singleplayers);
        file.Close();
    }

    public static void UpdateSavedAchievements(int wins=-1, int losses=-1)
    {
        // check if user is already loaded;
        if (achievements == null)
        {
            LoadSavedAchievements();
        }

        // update values if not null / default
        if(wins != -1)
        {
            achievements.wins = wins;
        }
        if(losses != -1)
        {
            achievements.losses = losses;
        }
        // actually update the file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = null;
        try
        {
            file = File.Create(Application.persistentDataPath + "/savedAchievements.gd");
        }
        catch (IOException e)
        {
            print(e.Message);
            file.Close();
            return;
        }

        bf.Serialize(file, achievements);
        file.Close();
    }

    #endregion
}