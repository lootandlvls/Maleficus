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

    protected override void Execute()
    {
        if (fromState == UIManager.Instance.CurrentState)
        {
            base.Execute();

            if (MenuNavigationActionPressed != null)
            {
                LogConsole("pressed");
                MenuNavigationActionPressed.Invoke(toState);
            }
        }
    }
}
