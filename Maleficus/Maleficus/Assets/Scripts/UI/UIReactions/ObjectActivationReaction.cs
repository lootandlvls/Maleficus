using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActivationReaction : AbstractUIReaction {


    protected override void PlayAppropriateReaction(EMenuState newState, EMenuState secondState)
    {
        if (newState == activeOnState || secondState == activeOnState)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
