using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObjectAnimation : AbstractAnimation {


    [SerializeField] private MenuState activeOnState;

    protected override void PlayAppropriateAnimation(MenuState newState)
    {
        if (newState == activeOnState)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
