using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Connect players with controllers
/// </summary>
public class StartConnectingPlayersAction : AbstractUIAction
{
    public event Action ConnectPlayersActionPressed;

    public override void Execute()
    {

        if (ConnectPlayersActionPressed != null)
        {
            ConnectPlayersActionPressed.Invoke();
        }
    }
}
