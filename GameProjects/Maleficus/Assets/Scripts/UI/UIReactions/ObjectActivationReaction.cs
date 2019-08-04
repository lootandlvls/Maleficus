using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActivationReaction : AbstractUIReaction {


    protected override void PlayAppropriateReaction(EMenuState newState)
    {
        if (newState.ContainedIn(activeOnStates))
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
