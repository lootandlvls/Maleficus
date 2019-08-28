using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTrackedIndicator : MonoBehaviour
{
    public void SetIsTracked()
    {
        //GetComponentInChildren<Text>().color = Color.red;
        Color color = Color.red;
        color.a = 0.25f;
        GetComponent<Image>().color = color;
    }

    public void SetIsUntracked()
    {
        //GetComponentInChildren<Text>().color = Color.green;
        Color color = Color.green;
        color.a = 0.25f;
        GetComponent<Image>().color = color;

    }
}
