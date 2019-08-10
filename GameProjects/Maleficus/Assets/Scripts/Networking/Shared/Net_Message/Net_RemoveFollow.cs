[System.Serializable]
public class Net_RemoveFollow : AbstractNetMessage
{
    public Net_RemoveFollow()
    {
        ID = NetID.RemoveFollow;
    }

    public string Token { set; get; }
    public string UsernameDiscriminator { set; get; }
}