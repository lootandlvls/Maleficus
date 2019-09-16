[System.Serializable]
public class Net_OnAddFollow : AbstractNetMessage
{
    public Net_OnAddFollow()
    {
        ID = ENetMessageID.ON_ADD_FOLLOW;
    }
    public byte Success { set; get; }
    public Account Follow { set; get; }
}