using System.Collections.Generic;

[System.Serializable]
public class Net_MovementInput : AbstractNetMessage
{
    public Net_MovementInput()
    {
        ID = NetID.MovementInput;
    }

    public string Token { set; get; }

    public EInputAxis axis {set; get; }

    public double axisValue { set; get; }
}