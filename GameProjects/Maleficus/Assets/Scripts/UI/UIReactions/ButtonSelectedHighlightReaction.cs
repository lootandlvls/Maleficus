using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BNJMOButton))]
public class ButtonSelectedHighlightReaction : AbstractUIReaction
{
    private BNJMOButton myMenuButton;

    private void Awake()
    {
        myMenuButton = GetComponent<BNJMOButton>();
    }

    protected override void PlayAppropriateReaction(EMenuState newState)
    {
        if (newState.ContainedIn(activeOnStates))
        {
            myMenuButton.Highlight();
        }
    }
}
