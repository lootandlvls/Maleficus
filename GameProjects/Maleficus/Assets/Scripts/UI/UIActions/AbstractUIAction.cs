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
[RequireComponent(typeof (BNJMOButton))]
public abstract class AbstractUIAction : BNJMOBehaviour
{
    public event Action ActionButtonExecuted;

    public BNJMOButton Button { get; private set; }

    [SerializeField] private bool delayedEventExecution = false;

    /// <summary>
    /// Trigger Execute() whenever the button is pressed.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        GetComponent<Button>().onClick.AddListener(Execute);

        // Bind events from MaleficusButton
        Button = GetComponent<BNJMOButton>();
        Button.ButtonSuccessfullyReleased += On_MaleficusButton_ButtonPressed;
    }

    /// <summary>
    /// Unsubscribe to ActionButtonPressed event when object is destroyed.
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        ClearEventCallbakcs(ActionButtonExecuted);
    }

    private void On_MaleficusButton_ButtonPressed(BNJMOButton maleficusButton)
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
