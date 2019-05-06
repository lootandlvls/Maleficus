using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class AbstractMenuAnimation : MonoBehaviour {

    [SerializeField] protected MenuState activeOnState;

    private void Start()
    {
        EventManager.Instance.UI_MenuStateUpdated += OnMenuStateUpdated;
    }

    protected virtual void OnMenuStateUpdated(MenuState newState, MenuState lastState)
    {
        PlayAppropriateAnimation(newState);
    }

    protected abstract void PlayAppropriateAnimation(MenuState newState);

}
