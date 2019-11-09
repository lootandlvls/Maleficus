using System.Collections.Generic;

[System.Serializable]
public class Net_OnRequestFollow : AbstractNetMessage
{
    public Net_OnRequestFollow()
    {
        MessageType = ENetMessageType.ON_REQUEST_FOLLOW;
    }

    public List<Account> Follows { set; get; }
}