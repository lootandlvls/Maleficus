using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARStagePlacedEventHandle : AbstractEventHandle
{
    public EPlayerID PlayerID { get; }

    public float X_Imagerotation;
    public float Y_Imagerotation;
    public float Z_Imagerotation;

    public float X_ImageToStage;
    public float Y_ImageToStage;
    public float Z_ImageToStage;


    public ARStagePlacedEventHandle(EPlayerID playerID, float x_Imagerotation, float y_Imagerotation, float z_Imagerotation, float x_ImageToStage, float y_ImageToStage, float z_ImageToStage)
    {
        PlayerID = playerID;
        X_Imagerotation = x_Imagerotation;
        Y_Imagerotation = y_Imagerotation;
        Z_Imagerotation = z_Imagerotation;
        X_ImageToStage = x_ImageToStage;
        Y_ImageToStage = y_ImageToStage;
        Z_ImageToStage = z_ImageToStage;
    }

    public override string GetDebugMessage()
    {
        return "";
    }

    public override AbstractNetMessage GetNetMessage()
    {
        return new Net_ARStagePlaced(
            PlayerID, 
            X_Imagerotation, 
            Y_Imagerotation, 
            Z_Imagerotation, 
            X_ImageToStage, 
            Y_ImageToStage, 
            Z_ImageToStage);
    }
}
