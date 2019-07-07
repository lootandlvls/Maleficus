using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BackAction : AbstractUIAction
{
    public event Action BackActionPressed;

    public override void Execute()
    {
        if (BackActionPressed != null)
        {
            BackActionPressed.Invoke();
        }
    }
}
