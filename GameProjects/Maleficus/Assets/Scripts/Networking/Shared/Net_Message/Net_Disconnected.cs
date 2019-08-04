[System.Serializable]
public class Net_Disonnected : NetMsg
{
    public Net_Disonnected()
    {
        OP = NetOP.Disconnected;
    }
}