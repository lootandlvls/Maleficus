using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class AbstractMenuAnimation : MonoBehaviour {

    [SerializeField] protected EMenuState activeOnState;

    private void Start()
    {
        EventManager.Instance.UI_MenuStateUpdated += OnMenuStateUpdated;
    }

    protected virtual void OnMenuStateUpdated(EMenuState newState, EMenuState lastState)
    {
        PlayAppropriateAnimation(newState);
    }

    protected abstract void PlayAppropriateAnimation(EMenuState newState);

}
