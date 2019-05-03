using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMenuCommand : MonoBehaviour
{
    /// <summary>
    /// Defines condition when the button should be active
    /// </summary>
    /// <returns>true if button is active </returns>
    protected abstract bool ActiveCondition();

    /// <summary>
    /// Executes the command
    /// </summary>
    public abstract void Execute();
}
