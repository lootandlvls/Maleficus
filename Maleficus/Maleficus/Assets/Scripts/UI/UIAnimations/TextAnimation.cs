using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimation : AbstractAnimation {

    
    [SerializeField] private MenuState activeOnState;
    
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    protected override void PlayAppropriateAnimation(MenuState newState)
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
