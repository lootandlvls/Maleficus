using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class AbstractAnimation : MonoBehaviour {


    private void Start()
    {
        EventManager.Instance.MenuStateUpdated += OnMenuStateUpdated;
    }

    protected virtual void OnMenuStateUpdated(MenuState newState, MenuState lastState)
    {
        PlayAppropriateAnimation(newState);
    }

    protected abstract void PlayAppropriateAnimation(MenuState newState);

}
