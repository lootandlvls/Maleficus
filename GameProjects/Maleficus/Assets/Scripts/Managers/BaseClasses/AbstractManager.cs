using UnityEngine;

public abstract class AbstractManager : BNJMOBehaviour
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

        OnReinitializeManager();
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        // Reinitialize manager whenever the scene is changed
        EventManager.Instance.APP_SceneChanged.AddListener(On_APP_SceneChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.APP_SceneChanged.RemoveListener(On_APP_SceneChanged);
        }
    }

    private void On_APP_SceneChanged(Event_GenericHandle<EScene> eventHandle)
    {
        OnReinitializeManager();
    }

    protected virtual void OnReinitializeManager()
    {
        // Define in specification
    }


}
