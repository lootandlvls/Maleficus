﻿[System.Serializable]
public class Net_Connected : AbstractNetMessage
{
    public Net_Connected()
    {
        ID = ENetMessageID.CONNECTED;
    }
}