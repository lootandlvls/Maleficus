[System.Serializable]
public class Net_InitLobby : NetMsg
{
    public Net_InitLobby()
    {
        OP = NetOP.InitLobby;
    }

    public string Token { set; get; }
}