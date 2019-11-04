[System.Serializable]
public class Net_AddFollow : AbstractNetMessage
{
    public Net_AddFollow()
    {
        MessageType = ENetMessageType.ADD_FOLLOW;
    }

    public string UsernameDiscriminatorOrEmail { set; get; }
}