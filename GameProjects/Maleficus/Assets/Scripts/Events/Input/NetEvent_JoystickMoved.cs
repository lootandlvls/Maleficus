﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.Utils;

[System.Serializable]
public class NetEvent_JoystickMoved : AbstractEventHandle
{
    public EControllerID ControllerID { get; }
    public EJoystickType JoystickType { get; }
    public float Joystick_X { get; }
    public float Joystick_Y { get; }


    public NetEvent_JoystickMoved(EClientID senderID, EControllerID controllerID, EJoystickType joystickType, float joystick_X, float joystick_Y)
    {
        MessageType = ENetMessageType.JOYSTICK_MOVED;
        SenderID = senderID;

        ControllerID = controllerID;
        JoystickType = joystickType;
        Joystick_X = joystick_X;
        Joystick_Y = joystick_Y;
    }


    public override string GetDebugMessage()
    {
        EPlayerID playerID = GetPlayerIDFrom(SenderID);
        return ControllerID + " moved " + JoystickType + " | X : " + Joystick_X + ", Y : " + Joystick_Y + " as : " + playerID;
    }
}
