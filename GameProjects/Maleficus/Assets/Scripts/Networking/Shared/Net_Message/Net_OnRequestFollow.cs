using System.Collections.Generic;

[System.Serializable]
public class Net_OnRequestFollow : AbstractNetMessage
{
    public Net_OnRequestFollow()
    {
        ID = NetID.OnRequestFollow;
    }

    public List<Account> Follows { set; get; }
}