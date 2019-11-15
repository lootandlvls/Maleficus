using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetEvent_Offline : AbstractEventHandle
{
    public NetEvent_Offline()
    {

    }

    public override string GetDebugMessage()
    {
        return "Playing Offline";
    }
}
