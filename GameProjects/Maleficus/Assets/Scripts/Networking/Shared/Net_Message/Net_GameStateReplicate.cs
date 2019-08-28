[System.Serializable]
public class Net_GameStateReplicate : AbstractNetMessage
{
    public EPlayerID playerID { get; }
    public float[] playerPosition = new float[3];
    public float[] playerRotation = new float[3];
   
    public Net_GameStateReplicate(EPlayerID playerID, float[] playerPosition, float[] playerRotation)
    {
        ID = NetID.GameStateReplicate;
        this.playerID = playerID;
        this.playerPosition = playerPosition;
        this.playerRotation = playerRotation;
    }
}
