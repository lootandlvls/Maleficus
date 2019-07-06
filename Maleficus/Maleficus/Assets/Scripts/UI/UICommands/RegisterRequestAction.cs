using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RegisterRequestAction : AbstractUIAction
{
    public event Action RegisterRequestActionPressed;

    public override void Execute()
    {
        if (RegisterRequestActionPressed != null)
        {
            RegisterRequestActionPressed.Invoke();
        }
    }
}