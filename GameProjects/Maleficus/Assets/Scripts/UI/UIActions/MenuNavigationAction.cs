using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuNavigationAction : AbstractUIAction {

    public event Action<EMenuState> MenuNavigationActionPressed;

    [SerializeField] private EMenuState fromState = EMenuState.NONE;
    [SerializeField] private EMenuState toState = EMenuState.NONE;

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (MenuNavigationActionPressed != null)
        {
            Delegate[] delegates = MenuNavigationActionPressed.GetInvocationList();
            foreach (Delegate myDelegate in delegates)
            {
                MenuNavigationActionPressed -= (myDelegate as Action<EMenuState>);
            }
        }
    }

    public override void Execute()
    {
        base.Execute();

        if (MenuNavigationActionPressed != null)
        {
            MenuNavigationActionPressed.Invoke(toState);
        }
    }
}
