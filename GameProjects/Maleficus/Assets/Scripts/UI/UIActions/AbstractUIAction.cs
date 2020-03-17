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
    public event Action ActionButtonExecuted;

    [SerializeField] private bool delayedEventExecution = false;

    protected MaleficusButton maleficusButton;

    /// <summary>
    /// Trigger Execute() whenever the button is pressed.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        GetComponent<Button>().onClick.AddListener(Execute);

        // Bind events from MaleficusButton
        maleficusButton = GetComponent<MaleficusButton>();
        maleficusButton.ButtonSuccessfullyReleased += On_MaleficusButton_ButtonPressed;
    }

    /// <summary>
    /// Unsubscribe to ActionButtonPressed event when object is destroyed.
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        ClearEventCallbakcs(ActionButtonExecuted);
    }

    private void On_MaleficusButton_ButtonPressed(MaleficusButton maleficusButton)
    {
        Execute();
    }

    /// <summary>
    /// Action that should be triggered when Button is pressed. 
    /// Extend it in child class when needed.
    /// </summary>
    protected virtual void Execute()
    {
        if (delayedEventExecution == false)
        {
            InvokeActionExecutedEvent();
        }
        else
        {
            Invoke(nameof(DelayedExecuteCoroutine), 0.1f);
        }
    }

    private void DelayedExecuteCoroutine()
    {
        InvokeActionExecutedEvent();
    }

    protected void InvokeActionExecutedEvent()
    {
        if (ActionButtonExecuted != null)
        {
            ActionButtonExecuted.Invoke();
            LogConsole("Executed");
        }
    }
}
