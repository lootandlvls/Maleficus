using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI element that executes a Action when interacted with.
/// </summary>
public abstract class AbstractUIAction : MonoBehaviour
{

    public abstract void Execute();
}
