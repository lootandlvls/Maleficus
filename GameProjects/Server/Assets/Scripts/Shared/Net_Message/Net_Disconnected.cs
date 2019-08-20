[System.Serializable]
public class Net_Disonnected : AbstractNetMessage
{
    public Net_Disonnected()
    {
        ID = NetID.Disconnected;
    }
}