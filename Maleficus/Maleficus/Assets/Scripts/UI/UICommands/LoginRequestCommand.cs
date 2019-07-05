using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoginRequestCommand : AbstractUICommand
{
    public event Action LoginRequestCommandPressed;

    public override void Execute()
    {
        if (LoginRequestCommandPressed != null)
        {
            LoginRequestCommandPressed.Invoke();
        }
    }
}