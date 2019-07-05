using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoBackToLoginCommand : AbstractUICommand
{
    public event Action GoBackToLoginCommandPressed;

    public override void Execute()
    {
        if (GoBackToLoginCommandPressed != null)
        {
            GoBackToLoginCommandPressed.Invoke();
        }
    }
}