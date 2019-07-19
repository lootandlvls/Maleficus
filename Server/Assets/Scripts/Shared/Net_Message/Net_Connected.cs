[System.Serializable]
public class Net_Connected : NetMsg
{
    public Net_Connected()
    {
        OP = NetOP.Connected;
    }
}