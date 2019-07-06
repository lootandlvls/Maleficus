using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuNavigationAction : AbstractUIAction {

    public event Action<EMenuState> MenuNavigationActionPressed;

    [SerializeField] private EMenuState fromState;
    [SerializeField] private EMenuState toState;


    public override void Execute()
    {

        if (MenuNavigationActionPressed != null)
        {
            MenuNavigationActionPressed.Invoke(toState);
        }
    }
}
