[System.Serializable]
public class Net_UpdateFollow : NetMsg
{
    public Net_UpdateFollow()
    {
        OP = NetOP.UpdateFollow;
    }
    public Account Follow { set; get; }
}