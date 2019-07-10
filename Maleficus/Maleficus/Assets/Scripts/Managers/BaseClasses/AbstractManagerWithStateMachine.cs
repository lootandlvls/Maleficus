using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// A Manager class that uses a state machine. See parent AbstractManager class for more information about what a Manager is.
/// IMPORTANT: do following steps when inheriting from this class
/// 1) Define "startState" in Awake() of child class
/// 2) Define "debugStateID" in Awake() of child class
/// 3) Bind "StateUpdateEvent" in start method of child class
/// </summary>
/// <typeparam name="E"></typeparam>
public abstract class AbstractSingletonManagerWithStateMachine<T, E> : AbstractSingletonManager<T> where T : AbstractSingletonManager<T>
{
    protected event Action<E, E> StateUpdateEvent;

    public E CurrentState { get { return currentState; } }
    public E LastState { get { return lastState; } }

    protected E currentState;
    protected E lastState;

    // Define start state in Awake() of child class
    protected E startState;
    // Define debug state ID in Awake() of child class
    protected int debugStateID;

    protected virtual void Start()
    {
        // Bind event in start method of child class!
        // Example:
        // StateUpdateEvent += EventManager.Instance.Invoke_UI_MenuStateUpdated;


        StartCoroutine(LateStartCoroutine());
    }

    protected virtual IEnumerator LateStartCoroutine()
    {
        yield return new WaitForEndOfFrame();
        UpdateState(startState);
    }

    protected virtual void Update()
    {
        // Debug current state
        DebugManager.Instance.Log(debugStateID, CurrentState.GetType() + " : " + currentState);
    }


    protected virtual void UpdateState(E newState)
    {
        if (newState.Equals(currentState))
        {
            return;
        }

        lastState = currentState;
        currentState = newState;

        if (StateUpdateEvent != null)
        {
            StateUpdateEvent.Invoke(currentState, lastState);
        }
    }
}
