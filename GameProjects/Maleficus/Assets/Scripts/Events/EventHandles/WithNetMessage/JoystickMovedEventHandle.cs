using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickMovedEventHandle : AbstractEventHandle
{
    public EJoystickType JoystickType { get; }
    public float Joystick_X { get; }
    public float Joystick_Y { get; }
    public EPlayerID PlayerID { get; }


    public JoystickMovedEventHandle(EJoystickType joystickType, float joystick_X, float joystick_Y, EPlayerID playerID)
    {
        JoystickType = joystickType;
        Joystick_X = joystick_X;
        Joystick_Y = joystick_Y;
        PlayerID = playerID;
    }


    public override string GetDebugMessage()
    {
        return PlayerID + " moved " + JoystickType + " | X : " + Joystick_X + ", Y : " + Joystick_Y;
    }

    public override AbstractNetMessage GetNetMessage()
    {
        return new Net_JoystickInput(JoystickType, Joystick_X, Joystick_Y, PlayerID);
    }
}
