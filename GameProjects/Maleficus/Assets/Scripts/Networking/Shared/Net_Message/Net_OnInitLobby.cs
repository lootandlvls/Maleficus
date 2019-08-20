[System.Serializable]
public class Net_OnInitLobby : AbstractNetMessage
{
    public Net_OnInitLobby()
    {
        ID = NetID.OnInitLobby;
    }

    public byte Success { set; get; }
    public string Information { set; get; }
    public int lobbyID { set; get; }
}