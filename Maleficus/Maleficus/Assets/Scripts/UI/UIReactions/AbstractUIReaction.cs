using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// UI element that reacts to UI state change.
/// </summary>
public abstract class AbstractUIReaction : MonoBehaviour {

    [SerializeField] protected EMenuState activeOnState;

    protected EMenuState[] activeOnStates;

    private void Awake()
    {
        List<EMenuState> temp = new List<EMenuState>();
        switch (activeOnState)
        {
            case EMenuState.LOGIN:
                temp = new List<EMenuState>(MaleficusTypes.MENU_STATE_IN_LOGIN);
                break;
        }

        temp.Add(activeOnState);
        activeOnStates = temp.ToArray();
    }

    private void Start()
    {
        EventManager.Instance.UI_MenuStateUpdated += OnMenuStateUpdated;
    }

    protected virtual void OnMenuStateUpdated(EMenuState newState, EMenuState lastState)
    {
        PlayAppropriateReaction(newState);
    }

    protected abstract void PlayAppropriateReaction(EMenuState newState);


}
