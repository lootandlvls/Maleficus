using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractManager : MonoBehaviour
{
    /// <summary>
    /// Initialize Manager (called in Awake)
    /// </summary>
    public virtual void Initialize()
    {
        // Define in child class
    }
}
