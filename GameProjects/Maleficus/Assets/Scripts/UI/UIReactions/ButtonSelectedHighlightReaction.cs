using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MaleficusButton))]
public class ButtonSelectedHighlightReaction : AbstractUIReaction
{
    private MaleficusButton myMenuButton;

    private void Awake()
    {
        myMenuButton = GetComponent<MaleficusButton>();
    }

    protected override void PlayAppropriateReaction(EMenuState newState)
    {
        if (newState == activeOnState)
        {
            myMenuButton.Highlight();
            UIManager.Instance.OnSelectedButton(myMenuButton);
        }
    }
}
