[System.Serializable]
public class Net_GameStarted : AbstractNetMessage
{
   public EPlayerID PlayerID;

    public Net_GameStarted(EPlayerID playerID)
    {
        ID = NetID.GameStarted;
       PlayerID = playerID;
    }
}
