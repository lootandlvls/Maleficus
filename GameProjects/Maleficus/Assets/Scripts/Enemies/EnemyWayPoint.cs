using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWayPoint : MonoBehaviour
{
    public Vector3 Position { get { return transform.position; } }

    private void Start()
    {
        EventManager.Instance.APP_AppStateUpdated.AddListener(On_APP_AppStateUpdated);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.APP_AppStateUpdated.RemoveListener(On_APP_AppStateUpdated);
        }
    }

    private void On_APP_AppStateUpdated(Event_StateUpdated<EAppState> eventHandle)
    {
        if (eventHandle.NewState == EAppState.IN_GAME_IN_RUNNING)
        {
            HideShadowMesh();
        }
    }

    private void HideShadowMesh()
    {
        Renderer[] myMeshRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer myMeshRenderer in myMeshRenderers)
        {
            myMeshRenderer.enabled = false;
        }
    }

}
