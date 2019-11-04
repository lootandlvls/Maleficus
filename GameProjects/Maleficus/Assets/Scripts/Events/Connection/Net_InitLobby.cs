[System.Serializable]
public class Net_InitLobby : AbstractNetMessage
{
    public Net_InitLobby()
    {
        MessageType = ENetMessageType.INIT_LOBBY;
    }

}