[System.Serializable]
public class Net_OnAddFollow : AbstractNetMessage
{
    public Net_OnAddFollow()
    {
        ID = NetID.OnAddFollow;
    }
    public byte Success { set; get; }
    public Account Follow { set; get; }
}