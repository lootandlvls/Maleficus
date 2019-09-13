[System.Serializable]
public class Net_InitLobby : AbstractNetMessage
{
    public Net_InitLobby()
    {
        ID = ENetMessageID.INIT_LOBBY;
    }

}