using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateUpdatedEventHandle<E> : AbstractEventHandle
{
    public E NewState { get; }
    public E LastState { get; }

    public StateUpdatedEventHandle(E newState, E lastState)
    {
        NewState = newState;
        lastState = LastState;
    }

    public override AbstractNetMessage GetNetMessage()
    {
        return new Net_NONE();
    }

    public override string GetDebugMessage()
    {
        return "App state changed from " + LastState + " to " + NewState;
    }
}

