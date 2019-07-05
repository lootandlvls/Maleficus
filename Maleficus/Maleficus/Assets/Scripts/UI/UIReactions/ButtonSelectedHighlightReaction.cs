using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MenuButton))]
public class ButtonSelectedHighlightReaction : AbstractUIReaction
{
    private MenuButton myMenuButton;

    private void Awake()
    {
        myMenuButton = GetComponent<MenuButton>();
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
