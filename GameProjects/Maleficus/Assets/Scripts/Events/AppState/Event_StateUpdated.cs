using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_StateUpdated<E> : AbstractEventHandle
{
    public E NewState { get; }
    public E LastState { get; }

    public Event_StateUpdated(E newState, E lastState)
    {
        MessageType = ENetMessageType.NONE;

        NewState = newState;
        LastState = lastState;
    }

    public override string GetDebugMessage()
    {
        return "State changed from " + LastState + " to " + NewState;
    }
}

