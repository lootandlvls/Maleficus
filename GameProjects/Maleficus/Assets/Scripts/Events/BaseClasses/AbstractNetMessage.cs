[System.Serializable]
public abstract class AbstractNetMessage
{
    /// <summary> Differentiate between the different messages </summary>
    public ENetMessageType MessageType         { get; set; }

    /// <summary> Differentiate between the different messages </summary>
    public float TimeStamp         { get; set; }

    /// <summary> Client ID of the message sender </summary>
    public EClientID SenderID       { get; set; }

    /// <summary> #define# </summary>
    public string Token             { get; set; }

    public AbstractNetMessage()
    {
        MessageType = ENetMessageType.NONE;
    }
}