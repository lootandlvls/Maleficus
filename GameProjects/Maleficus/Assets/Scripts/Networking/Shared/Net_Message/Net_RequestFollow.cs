[System.Serializable]
public class Net_RequestFollow : AbstractNetMessage
{
    public Net_RequestFollow()
    {
        ID = NetID.RequestFollow;
    }

    public string Token { set; get; }
}