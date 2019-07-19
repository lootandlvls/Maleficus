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
        GetComponentInChildren<Text>().color = Color.red;
    }

    public void SetIsUnlocked()
    {
        GetComponentInChildren<Text>().color = Color.green;
    }
}

