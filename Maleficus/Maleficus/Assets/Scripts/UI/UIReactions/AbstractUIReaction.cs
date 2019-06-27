using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// UI element that reacts to UI state change.
/// </summary>
public abstract class AbstractUIReaction : MonoBehaviour {

    [SerializeField] protected EMenuState activeOnState;
    [SerializeField] protected EMenuState activeOnSecondState;

    private void Start()
    {
        EventManager.Instance.UI_MenuStateUpdated += OnMenuStateUpdated;
    }

    protected virtual void OnMenuStateUpdated(EMenuState newState, EMenuState lastState)
    {
        PlayAppropriateReaction(newState, lastState);
    }

    protected abstract void PlayAppropriateReaction(EMenuState newState, EMenuState secondState);

}
