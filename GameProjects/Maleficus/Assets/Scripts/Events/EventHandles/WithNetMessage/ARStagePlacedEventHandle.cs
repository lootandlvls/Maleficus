using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ARStagePlacedEventHandle : AbstractEventHandle
{
    public float X_TrackerRotation;
    public float Y_TrackerRotation;
    public float Z_TrackerRotation;

    public float X_TrackerToStage;
    public float Y_TrackerToStage;
    public float Z_TrackerToStage;


    public ARStagePlacedEventHandle(EClientID senderID, float x_Imagerotation, float y_Imagerotation, float z_Imagerotation, float x_ImageToStage, float y_ImageToStage, float z_ImageToStage)
    {
        ID = ENetMessageID.AR_STAGE_PLACED;
        SenderID = senderID;

        X_TrackerRotation = x_Imagerotation;
        Y_TrackerRotation = y_Imagerotation;
        Z_TrackerRotation = z_Imagerotation;
        X_TrackerToStage = x_ImageToStage;
        Y_TrackerToStage = y_ImageToStage;
        Z_TrackerToStage = z_ImageToStage;
    }

    public override string GetDebugMessage()
    {
        return "";
    }

    //public override AbstractNetMessage GetNetMessage()
    //{
    //    return new Net_ARStagePlaced(
    //        PlayerID, 
    //        X_TrackerRotation, 
    //        Y_TrackerRotation, 
    //        Z_TrackerRotation, 
    //        X_TrackerToStage, 
    //        Y_TrackerToStage, 
    //        Z_TrackerToStage);
    //}
}
