[System.Serializable]
public class Net_RequestFollow : AbstractNetMessage
{
    public Net_RequestFollow()
    {
        ID = ENetMessageID.REQUEST_FOLLOW;
    }

}