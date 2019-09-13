using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_NONE : AbstractNetMessage
{
    public Net_NONE()
    {
        ID = ENetMessageID.NONE;
    }
}
