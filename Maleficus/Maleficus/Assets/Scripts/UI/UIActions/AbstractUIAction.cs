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
public abstract class AbstractUIAction : MonoBehaviour
{
    public event Action ActionButtonPressed;

    /// <summary>
    /// Trigger Execute() whenever the button is pressed.
    /// </summary>
    protected virtual void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Execute);
    }

    /// <summary>
    /// Unsubscribe to ActionButtonPressed event when object is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (ActionButtonPressed != null)
        {
            Delegate[] delegates = ActionButtonPressed.GetInvocationList();
            foreach (Delegate myDelegate in delegates)
            {
                ActionButtonPressed -= (myDelegate as Action);
            }
        }
    }

    /// <summary>
    /// Action that should be triggered when Button is pressed. 
    /// Extend it in child class when needed.
    /// </summary>
    public virtual void Execute()
    {
        InvokeActionPressedEvent();
    }

    protected void InvokeActionPressedEvent()
    {
        if (ActionButtonPressed != null)
        {
            ActionButtonPressed.Invoke();
        }
    }

}
