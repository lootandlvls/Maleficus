using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OpenLoginPopUpCommand : AbstractUICommand
{
    public event Action OpenLoginPopUpCommandPressed;

    public override void Execute()
    {
        if (OpenLoginPopUpCommandPressed != null)
        {
            OpenLoginPopUpCommandPressed.Invoke();
        }
    }
}