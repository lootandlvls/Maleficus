[System.Serializable]
public class Net_InitLobby : AbstractNetMessage
{
    public Net_InitLobby()
    {
        ID = NetID.InitLobby;
    }

    public string Token { set; get; }
}