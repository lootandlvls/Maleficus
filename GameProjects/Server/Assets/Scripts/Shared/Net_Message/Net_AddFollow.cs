[System.Serializable]
public class Net_AddFollow : AbstractNetMessage
{
    public Net_AddFollow()
    {
        ID = NetID.AddFollow;
    }

    public string Token { set; get; }
    public string UsernameDiscriminatorOrEmail { set; get; }
}