[System.Serializable]
public class Net_AddFollow : AbstractNetMessage
{
    public Net_AddFollow()
    {
        ID = NetID.AddFollow;
    }

    public string UsernameDiscriminatorOrEmail { set; get; }
}