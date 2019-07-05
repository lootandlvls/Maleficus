using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OpenRegisterPopUpCommand : AbstractUICommand
{
    public event Action OpenRegisterPopUpCommandPressed;

    public override void Execute()
    {
        if (OpenRegisterPopUpCommandPressed != null)
        {
            OpenRegisterPopUpCommandPressed.Invoke();
        }
    }
}