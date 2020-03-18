using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSkillPointStar : BNJMOBehaviour
{
    public int StarID = 0;


    public void ShowStar()
    {
        RawImage image = GetComponent<RawImage>();
        if (IS_NOT_NULL(image))
        {
            image.enabled = true;
        }
    }

    public void HideStar()
    {
        RawImage image = GetComponent<RawImage>();
        if (IS_NOT_NULL(image))
        {
            image.enabled = false;
        }
    }

}
