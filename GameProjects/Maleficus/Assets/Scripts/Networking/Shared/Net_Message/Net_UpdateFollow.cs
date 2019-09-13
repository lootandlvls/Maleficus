[System.Serializable]
public class Net_UpdateFollow : AbstractNetMessage
{
    public Net_UpdateFollow()
    {
        ID = ENetMessageID.UPDATE_FOLLOW;
    }
    public Account Follow { set; get; }
}