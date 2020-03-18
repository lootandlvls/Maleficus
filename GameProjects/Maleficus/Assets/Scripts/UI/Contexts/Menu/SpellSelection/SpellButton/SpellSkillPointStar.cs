using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSkillPointStar : BNJMOBehaviour
{
    public int StarID = 0;


    public void Show()
    {
        RawImage image = GetComponent<RawImage>();
        if (IS_NOT_NULL(image))
        {
            image.enabled = true;
        }
    }

    public void Hide()
    {
        RawImage image = GetComponent<RawImage>();
        if (IS_NOT_NULL(image))
        {
            image.enabled = false;
        }
    }

}
