public static class NetID
{
    public const int None=0;

    public const int CreateAccount = 1;
    public const int LoginRequest = 2;

    public const int OnCreateAccount = 3;
    public const int OnLoginRequest = 4;


    public const int AddFollow = 5;
    public const int RemoveFollow = 6;
    public const int RequestFollow = 7;

    public const int OnAddFollow = 8;
    public const int OnRequestFollow = 9;
    public const int UpdateFollow = 10;


    public const int InitLobby = 11;

    public const int OnInitLobby = 12;

    public const int Connected = 13;
    public const int Disconnected = 14;

    public const int SpellInput = 15;
    public const int MovementInput = 16;

    public const int RequestGameInfo = 17;
    public const int OnRequestGameInfo = 18;



    public const int Test = 255;  // Maximum
}
[System.Serializable]
public abstract class AbstractNetMessage
{
    /// <summary> ID = const id of the NET_ class, to differentiate between the different messages </summary>
    public byte ID { set; get; }

    public AbstractNetMessage()
    {
        ID = NetID.None;
    }
}