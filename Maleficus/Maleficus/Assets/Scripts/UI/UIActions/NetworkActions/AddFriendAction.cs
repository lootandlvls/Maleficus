using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AddFriendAction : AbstractUIAction
{
    public event Action AddFriendActionPressed;

    public override void Execute()
    {
        if (AddFriendActionPressed != null)
        {
            AddFriendActionPressed.Invoke();
        }
    }
}