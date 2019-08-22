using System.Collections.Generic;

[System.Serializable]
public class Net_MovementInput : AbstractNetMessage
{
    public Net_MovementInput(EInputAxis axisType, float axisValue, EPlayerID playerID)
    {
        ID = NetID.MovementInput;
        AxisType = axisType;
        AxisValue = axisValue;
        PlayerID = playerID;
    }

    public EInputAxis AxisType { get; }
    public float AxisValue { get; }
    public EPlayerID PlayerID { get; }
}