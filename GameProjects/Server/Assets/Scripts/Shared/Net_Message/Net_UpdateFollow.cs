[System.Serializable]
public class Net_UpdateFollow : AbstractNetMessage
{
    public Net_UpdateFollow()
    {
        ID = NetID.UpdateFollow;
    }
    public Account Follow { set; get; }
}