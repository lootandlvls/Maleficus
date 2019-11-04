[System.Serializable]
public class Net_RemoveFollow : AbstractNetMessage
{
    public Net_RemoveFollow()
    {
        MessageType = ENetMessageType.REMOVE_FOLLOW;
    }

    public string UsernameDiscriminator { set; get; }
}