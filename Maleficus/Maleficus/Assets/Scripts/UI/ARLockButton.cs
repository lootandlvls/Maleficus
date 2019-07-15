using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARLockButton : MonoBehaviour
{
    private Text myText;

    private void Awake()
    {
        myText = GetComponentInChildren<Text>();
    }

    public void SetIsLocked()
    {
        myText.color = Color.red;
    }

    public void SetIsUnlocked()
    {
        myText.color = Color.green;
    }
}

