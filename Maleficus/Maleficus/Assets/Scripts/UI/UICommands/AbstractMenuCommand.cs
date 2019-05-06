using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMenuCommand : MonoBehaviour
{
    /// <summary>
    /// Executes the command
    /// </summary>
    public abstract void Execute();
}
