using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARStagePlacedEventHandle : AbstractEventHandle
{
    public float X_position;
    public float Y_position;
    public float Z_position;
    public float X_rotation;
    public float Y_rotation;
    public float Z_rotation;

    public ARStagePlacedEventHandle()
    {
       
    }

    public ARStagePlacedEventHandle(float x_position, float y_position, float z_position, float x_rotation, float y_rotation, float z_rotation)
    {
        X_position = x_position;
        Y_position = y_position;
        Z_position = z_position;
        X_rotation = x_rotation;
        Y_rotation = y_rotation;
        Z_rotation = z_rotation;
    }


    public override string GetDebugMessage()
    {
        throw new System.NotImplementedException();
    }

    public override AbstractNetMessage GetNetMessage()
    {
        throw new System.NotImplementedException();
    }
}
