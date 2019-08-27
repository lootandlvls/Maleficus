using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARStagePlacedEventHandle : AbstractEventHandle
{
    
    public float X_rotation;
    public float Y_rotation;
    public float Z_rotation;
    public EPlayerID PlayerID { get; }

    public ARStagePlacedEventHandle()
    {
       
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
