[System.Serializable]
public class Net_ARStagePlaced : AbstractNetMessage
{
    public EPlayerID PlayerID { get; }

    public float X_Imagerotation;
    public float Y_Imagerotation;
    public float Z_Imagerotation;

    public float X_ImageToStage;
    public float Y_ImageToStage;
    public float Z_ImageToStage;


    public Net_ARStagePlaced(EPlayerID playerID, float x_Imagerotation, float y_Imagerotation, float z_Imagerotation, float x_ImageToStage, float y_ImageToStage, float z_ImageToStage)
    {
        ID = NetID.AddFollow;
        PlayerID = playerID;
        X_Imagerotation = x_Imagerotation;
        Y_Imagerotation = y_Imagerotation;
        Z_Imagerotation = z_Imagerotation;
        X_ImageToStage = x_ImageToStage;
        Y_ImageToStage = y_ImageToStage;
        Z_ImageToStage = z_ImageToStage;
    }
}
