using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// UI element that executes a Action when interacted with.
/// Simply drag component on GameObject with a Button and it will automatically binds to event.
/// </summary>

[RequireComponent(typeof (Button))]
[RequireComponent(typeof (MaleficusButton))]
public abstract class AbstractUIAction : BNJMOBehaviour
{
    [SerializeField] private bool delayedEventExecution = false;

    public event Action ActionButtonPressed;
    //public event Action ActionButtonReleased; // Todo
    public event Action ActionButtonHighlighted;

    /// <summary>
    /// Trigger Execute() whenever the button is pressed.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        GetComponent<Button>().onClick.AddListener(Execute);
    }

    /// <summary>
    /// Unsubscribe to ActionButtonPressed event when object is destroyed.
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        ClearEventCallbakcs(ActionButtonPressed);
    }

    /// <summary>
    /// Action that should be triggered when Button is pressed. 
    /// Extend it in child class when needed.
    /// </summary>
    public virtual void Execute()
    {
        if (delayedEventExecution == false)
        {
            InvokeActionPressedEvent();
        }
        else
        {
            Invoke(nameof(DelayedExecuteCoroutine), 0.1f);
        }
    }

    private void DelayedExecuteCoroutine()
    {
        InvokeActionPressedEvent();
    }

    /// <summary>
    /// Action that should be triggered when Button is highlighted (selected). 
    /// Extend it in child class when needed.
    /// </summary>
    public virtual void OnHighlighted()
    {
        InvokeActionHighlightedEvent();
    }

    protected void InvokeActionPressedEvent()
    {
        if (ActionButtonPressed != null)
        {
            ActionButtonPressed.Invoke();
            LogConsole("Pressed");
        }
    }

    protected void InvokeActionHighlightedEvent()
    {
        if (ActionButtonHighlighted != null)
        {
            ActionButtonHighlighted.Invoke();
        }
    }



}
