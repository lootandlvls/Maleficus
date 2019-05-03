using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRotateAnimation : AbstractAnimation
{
    [SerializeField] private MenuState activeOnState;

    private bool canRotate = false;

    private void Update()
    {
        if (canRotate == true)
        {
            transform.Rotate(0.0f, 1.0f, 0.0f);
        }

        DebugManager.Instance.Log(2, "Warrior : " + transform.rotation.eulerAngles.ToString());
    }


    protected override void PlayAppropriateAnimation(MenuState newState)
    {
        if (newState == activeOnState)
        {
            canRotate = true;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else
        {
            canRotate = false;
        }
    }
}
