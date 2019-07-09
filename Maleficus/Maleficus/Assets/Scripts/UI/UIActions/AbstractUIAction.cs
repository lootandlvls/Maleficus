using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI element that executes a Action when interacted with.
/// Simply drag component on GameObject with a Button and it will automatically binds to event.
/// </summary>

[RequireComponent(typeof (Button))]
public abstract class AbstractUIAction : MonoBehaviour
{
    protected virtual void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Execute);
    }

    public abstract void Execute();
}
