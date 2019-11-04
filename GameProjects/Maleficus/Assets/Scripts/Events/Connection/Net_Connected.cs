[System.Serializable]
public class Net_Connected : AbstractNetMessage
{
    public Net_Connected()
    {
        MessageType = ENetMessageType.CONNECTED;
    }
}