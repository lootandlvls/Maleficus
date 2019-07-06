using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoginRequestAction : AbstractUIAction
{
    public event Action LoginRequestActionPressed;

    public override void Execute()
    {
        if (LoginRequestActionPressed != null)
        {
            LoginRequestActionPressed.Invoke();
        }
    }
}