using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OpenRegisterPopUpAction : AbstractUIAction
{
    public event Action OpenRegisterPopUpActionPressed;

    public override void Execute()
    {
        if (OpenRegisterPopUpActionPressed != null)
        {
            OpenRegisterPopUpActionPressed.Invoke();
        }
    }
}