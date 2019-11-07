using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton behaviour class, used for components that should only have one instance.
/// Child class can decide weither to initialize on the Awake or through the public function Initialize.
/// </summary>
/// <typeparam name="T"> Child class </typeparam>
public abstract class AbstractSingletonManager<T> : AbstractManager where T : AbstractSingletonManager<T>
{
    private static T instance;
    public static T Instance { get { return instance; } }

    /// <summary>
    /// Returns whether the instance has been initialized or not.
    /// </summary>
    public static bool IsInstanceSet { get { return instance != null; } }

    /// <summary>
    /// Base awake method that sets the singleton's unique instance.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (instance != null)
        {
            Debug.LogErrorFormat("Trying to instantiate a second instance of singleton class {0}", GetType().Name);
        }
        else
        {
            instance = (T)this;
        }
    }

    /// <summary>
    /// Base awake method that resets the singleton's unique instance.
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (instance == this)
        {
            instance = null;
        }
    }
}
