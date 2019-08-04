[System.Serializable]
public class Net_OnInitLobby : NetMsg
{
    public Net_OnInitLobby()
    {
        OP = NetOP.OnInitLobby;
    }

    public byte Success { set; get; }
    public string Information { set; get; }
    public int playerID { set; get; }
}