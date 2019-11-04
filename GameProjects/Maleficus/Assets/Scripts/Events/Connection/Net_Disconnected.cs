[System.Serializable]
public class Net_Disonnected : AbstractNetMessage
{
    public Net_Disonnected()
    {
        MessageType = ENetMessageType.DISCONNECTED;
    }
}