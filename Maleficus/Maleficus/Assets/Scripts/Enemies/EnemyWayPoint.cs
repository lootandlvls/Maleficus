using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWayPoint : MonoBehaviour
{
    public Vector3 Position { get { return transform.position; } }

    private void Start()
    {
        EventManager.Instance.ÁPP_AppStateUpdated += On_APP_AppStateUpdated;
    }

    private void On_APP_AppStateUpdated(EAppState newState, EAppState lastState)
    {
        if (newState == EAppState.IN_GAME_IN_RUNNING)
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
