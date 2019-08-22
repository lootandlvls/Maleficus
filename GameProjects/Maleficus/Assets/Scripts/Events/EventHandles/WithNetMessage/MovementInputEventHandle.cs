using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInputEventHandle : AbstractEventHandle
{
    public EInputAxis AxisType { get; }
    public float AxisValue { get; }
    public EPlayerID PlayerID { get; }


    public MovementInputEventHandle(EInputAxis axisType, float axisValue, EPlayerID playerID)
    {
        AxisType = axisType;
        AxisValue = axisValue;
        PlayerID = playerID;
    }


    public override string GetDebugMessage()
    {
        return "";
    }

    public override AbstractNetMessage GetNetMessage()
    {
        return new Net_MovementInput(AxisType, AxisValue, PlayerID);
    }
}
