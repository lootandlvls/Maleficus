using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimation : AbstractMenuAnimation {
    
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    protected override void PlayAppropriateAnimation(EMenuState newState)
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
