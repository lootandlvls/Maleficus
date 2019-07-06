using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OpenLoginPopUpAction : AbstractUIAction
{
    public event Action OpenLoginPopUpActionPressed;

    public override void Execute()
    {
        if (OpenLoginPopUpActionPressed != null)
        {
            OpenLoginPopUpActionPressed.Invoke();
        }
    }
}