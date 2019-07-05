using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RegisterRequestCommand : AbstractUICommand
{
    public event Action RegisterRequestCommandPressed;

    public override void Execute()
    {
        if (RegisterRequestCommandPressed != null)
        {
            RegisterRequestCommandPressed.Invoke();
        }
    }
}