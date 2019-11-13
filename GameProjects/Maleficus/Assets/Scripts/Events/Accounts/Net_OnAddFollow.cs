[System.Serializable]
public class Net_OnAddFollow : AbstractNetMessage
{
    public Net_OnAddFollow()
    {
        MessageType = ENetMessageType.ON_ADD_FOLLOW;
    }
    public byte Success { set; get; }
    public Local_Account Follow { set; get; }
}