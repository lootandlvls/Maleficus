using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// A Manager class that uses a state machine. See parent AbstractManager class for more information about what a Manager is.
/// IMPORTANT: do following INSTRUCTIONS steps when inheriting from this class
/// 1) Assign appropriate currentState from MaleficusTypes
/// 2) Define "debugStateID" in Awake() of child class
/// 3) Bind "StateUpdateEvent" in start method of child class
/// </summary>
/// <typeparam name="E"></typeparam>
public abstract class AbstractSingletonManagerWithStateMachine<T, E> : AbstractSingletonManager<T> where T : AbstractSingletonManager<T>
{

    protected event Action<E, E> StateUpdateEvent;

    /// <summary>
    /// Start default state according to scene
    /// Assign in Awake from MaleficusTypes
    /// </summary>
    protected Dictionary<EScene, E> startStates;

    public E CurrentState { get { return currentState; } }
    public E LastState { get { return lastState; } }

    protected E currentState;
    protected E lastState;

    /// <summary>
    /// Debug current state with DebugManager using this ID
    /// Define debug state ID in Awake() of child class
    /// </summary>
    protected int debugStateID;

    protected override void Awake()
    {
        base.Awake();

        /*  INSTRUCTIONS
         * 1) Assign appropriate currentState from MaleficusTypes
         * Example:
         * startStates = MaleficusTypes.StartAppStates;
        */

        /* INSTRUCTIONS
         *   2) Define "debugStateID" in Awake() of child class
         *   Example:
         *   debugStateID = 50; // for UI
         */
    }

    protected virtual void Start()
    {
        /* INSTRUCTIONS
         *  3) Bind event in start method of child class!
         *  Example:
         *  StateUpdateEvent += EventManager.Instance.Invoke_UI_MenuStateUpdated;
         *  */

        // Register to scene change
        EventManager.Instance.APP_SceneChanged += On_APP_SceneChanged;
        // Wait for another frame before setting state
        StartCoroutine(LateStartCoroutine());
    }

    protected virtual IEnumerator LateStartCoroutine()
    {
        yield return new WaitForEndOfFrame();

        // Update state according to current Scene
        Debug.Log("Current scene : " + AppStateManager.Instance.CurrentScene);
        EScene currentScene = AppStateManager.Instance.CurrentScene;
        if (startStates.ContainsKey(currentScene))
        {
            UpdateState(startStates[currentScene]);
        }
    }

    protected virtual void Update()
    {
        // Debug current state
        DebugManager.Instance.Log(debugStateID, CurrentState.GetType() + " : " + currentState);
    }

    /// <summary>
    /// Update the state and trigger corresponding event
    /// </summary>
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

    private void On_APP_SceneChanged(EScene newScene)
    {
        Debug.Log(gameObject.name + " > New Scene " + newScene + " . Update state from " + currentState + " to " + startStates[newScene]);
        UpdateState(startStates[newScene]);
    }
}
