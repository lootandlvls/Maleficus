using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelectAnimation : AbstractMenuAnimation
{
    private MenuButton myMenuButton;

    private void Awake()
    {
        myMenuButton = GetComponent<MenuButton>();
    }

    protected override void PlayAppropriateAnimation(MenuState newState)
    {
        if (newState == activeOnState)
        {
            myMenuButton.Highlight();
            UIManager.Instance.OnSelectedButton(myMenuButton);
        }
    }
}
