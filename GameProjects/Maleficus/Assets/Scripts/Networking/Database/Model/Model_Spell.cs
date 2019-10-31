using MongoDB.Bson;
public class Model_Spell
{
    // identification
    public ObjectId _id { set; get; }
    public byte spell_id { set; get; }
    public ObjectId account_id { set; get; } // account

    // other values
    public byte spell_level { set; get; }
    public int spell_xp { set; get; }
    public bool selected { set; get; }
}