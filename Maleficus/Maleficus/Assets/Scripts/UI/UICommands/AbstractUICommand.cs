using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI element that executes a command when interacted with.
/// </summary>
public abstract class AbstractUICommand : MonoBehaviour
{

    public abstract void Execute();
}
