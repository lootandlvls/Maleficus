using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetEvent_ARStagePlaced : AbstractEventHandle
{
    public float X_TrackerRotation;
    public float Y_TrackerRotation;
    public float Z_TrackerRotation;

    public float X_TrackerToStage;
    public float Y_TrackerToStage;
    public float Z_TrackerToStage;


    public NetEvent_ARStagePlaced(EClientID senderID, float x_Imagerotation, float y_Imagerotation, float z_Imagerotation, float x_ImageToStage, float y_ImageToStage, float z_ImageToStage)
    {
        MessageType = ENetMessageType.AR_STAGE_PLACED;
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
        return "Ar Stage placed";
    }
}
