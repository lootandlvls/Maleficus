using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasActivator : BNJMOBehaviour
{
    protected override void Awake()
    {
        base.Awake();

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
