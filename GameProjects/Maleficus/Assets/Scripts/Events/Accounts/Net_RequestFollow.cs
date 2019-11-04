[System.Serializable]
public class Net_RequestFollow : AbstractNetMessage
{
    public Net_RequestFollow()
    {
        MessageType = ENetMessageType.REQUEST_FOLLOW;
    }

}