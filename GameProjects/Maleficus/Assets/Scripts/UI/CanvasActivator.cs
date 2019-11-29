using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasActivator : MaleficusMonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
