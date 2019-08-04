using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARLockButton : AbstractUIAction
{
    private Text myText;

    protected override void Awake()
    {
        base.Awake();

        myText = GetComponentInChildren<Text>();
    }

    public void SetIsLocked()
    {
        //GetComponentInChildren<Text>().color = Color.red;
        Color color = Color.red;
        color.a = 0.25f;
        GetComponent<Image>().color = color;
    }

    public void SetIsUnlocked()
    {
        //GetComponentInChildren<Text>().color = Color.green;
        Color color = Color.green;
        color.a = 0.25f;
        GetComponent<Image>().color = color;

    }
}

