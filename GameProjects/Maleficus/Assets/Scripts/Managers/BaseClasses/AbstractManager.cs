using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractManager : MaleficusMonoBehaviour
{
    /// <summary>
    /// Mark this object as should not be destroyed when a new scene is loaded
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Initialize Manager (called by MotherOfManagers when scene is changed)
    /// </summary>
    public abstract void OnSceneStartReinitialize();

 
}
