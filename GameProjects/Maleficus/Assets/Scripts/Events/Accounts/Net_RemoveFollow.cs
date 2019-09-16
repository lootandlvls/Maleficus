[System.Serializable]
public class Net_RemoveFollow : AbstractNetMessage
{
    public Net_RemoveFollow()
    {
        ID = ENetMessageID.REMOVE_FOLLOW;
    }

    public string UsernameDiscriminator { set; get; }
}