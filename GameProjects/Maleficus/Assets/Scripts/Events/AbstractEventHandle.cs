using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEventHandle
{

    /// <summary>
    /// Gets the corresponding Net Message to this event handle.
    /// If returned object is null, this means there is no Net Message associated to this Event Handle (No need to broadcast event)
    /// </summary>
    public abstract AbstractNetMessage GetNetMessage();

    public abstract string GetEventDebugMessage();


}
