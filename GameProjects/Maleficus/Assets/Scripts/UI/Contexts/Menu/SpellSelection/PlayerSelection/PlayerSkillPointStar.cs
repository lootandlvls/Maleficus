using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillPointStar : BNJMOBehaviour
{
    public int StarID;

    private Image image;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        image = GetComponentWithCheck<Image>();
    }

    public void Show()
    {
        image.enabled = true;
    }

    public void Hide()
    {
        image.enabled = false;
    }
}
