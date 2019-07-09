using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoBackToLoginAction : AbstractUIAction
{
    public event Action GoBackToLoginActionPressed;

    public override void Execute()
    {
        if (GoBackToLoginActionPressed != null)
        {
            GoBackToLoginActionPressed.Invoke();
        }
    }
}