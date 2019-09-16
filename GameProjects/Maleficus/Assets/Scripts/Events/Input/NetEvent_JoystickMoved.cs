using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetEvent_JoystickMoved : AbstractEventHandle
{
    public EJoystickType JoystickType { get; }
    public float Joystick_X { get; }
    public float Joystick_Y { get; }


    public NetEvent_JoystickMoved(EClientID senderID, EJoystickType joystickType, float joystick_X, float joystick_Y)
    {
        ID = ENetMessageID.JOYSTICK_MOVED;
        SenderID = senderID;

        JoystickType = joystickType;
        Joystick_X = joystick_X;
        Joystick_Y = joystick_Y;
    }


    public override string GetDebugMessage()
    {
        EPlayerID playerID = MaleficusUtilities.GetPlayerIDFrom(SenderID);
        return playerID + " moved " + JoystickType + " | X : " + Joystick_X + ", Y : " + Joystick_Y;
    }
}
