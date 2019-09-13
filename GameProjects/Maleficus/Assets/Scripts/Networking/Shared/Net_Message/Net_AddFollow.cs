[System.Serializable]
public class Net_AddFollow : AbstractNetMessage
{
    public Net_AddFollow()
    {
        ID = ENetMessageID.ADD_FOLLOW;
    }

    public string UsernameDiscriminatorOrEmail { set; get; }
}