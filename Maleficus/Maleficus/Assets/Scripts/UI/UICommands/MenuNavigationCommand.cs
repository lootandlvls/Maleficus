using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuNavigationCommand : AbstractUICommand {

    public event Action<EMenuState> MenuNavigationCommandPressed;

    [SerializeField] private EMenuState fromState;
    [SerializeField] private EMenuState toState;


    public override void Execute()
    {

        if (MenuNavigationCommandPressed != null)
        {
            MenuNavigationCommandPressed.Invoke(toState);
        }
    }
}
