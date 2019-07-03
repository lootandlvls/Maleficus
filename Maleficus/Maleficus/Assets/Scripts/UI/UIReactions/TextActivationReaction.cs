using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextActivationReaction : AbstractUIReaction {
    
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    protected override void PlayAppropriateReaction(EMenuState newState, EMenuState secondState)
    {
        if (newState == activeOnState)
        {
            text.enabled = true;
        }
        else
        {
            text.enabled = false;
        }
    }
}
