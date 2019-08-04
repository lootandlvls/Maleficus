using System.Collections.Generic;

[System.Serializable]
public class Net_MovementInput : NetMsg
{
    public Net_MovementInput()
    {
        OP = NetOP.MovementInput;
    }

    public string Token { set; get; }

    public EInputAxis axis {set; get; }

    public double axisValue { set; get; }
}