using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StartConnectingPlayersCommand : AbstractUICommand
{
    public event Action ConnectPlayersCommandPressed;

    public override void Execute()
    {

        if (ConnectPlayersCommandPressed != null)
        {
            ConnectPlayersCommandPressed.Invoke();
        }
    }
}
