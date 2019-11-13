[System.Serializable]
public class Net_UpdateFollow : AbstractNetMessage
{
    public Net_UpdateFollow()
    {
        MessageType = ENetMessageType.UPDATE_FOLLOW;
    }
    public Local_Account Follow { set; get; }
}