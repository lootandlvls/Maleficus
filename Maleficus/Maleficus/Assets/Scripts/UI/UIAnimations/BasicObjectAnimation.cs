using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObjectAnimation : AbstractMenuAnimation {


    protected override void PlayAppropriateAnimation(EMenuState newState)
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
