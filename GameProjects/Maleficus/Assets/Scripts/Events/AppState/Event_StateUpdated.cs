﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_StateUpdated<E> : AbstractEventHandle
{
    public E NewState { get; }
    public E LastState { get; }

    public Event_StateUpdated(E newState, E lastState)
    {
        ID = ENetMessageID.NONE;

        NewState = newState;
        lastState = LastState;
    }

    public override string GetDebugMessage()
    {
        return "App state changed from " + LastState + " to " + NewState;
    }
}
