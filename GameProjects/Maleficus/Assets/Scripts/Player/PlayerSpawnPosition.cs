﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPosition : MonoBehaviour
{
    public EPlayerID ToSpawnPlayerID    { get { return toSpawnPlayerID; } set { toSpawnPlayerID = value; } }
    public Vector3 Position             { get { return transform.position; } set { transform.position = value; } }
    public Quaternion Rotation          { get { return transform.rotation; } set { transform.rotation = value; } }

    [SerializeField] private EPlayerID toSpawnPlayerID;
    [SerializeField] private bool isHideShadowMeshOnAwake;


    private void Awake()
    {
        if (isHideShadowMeshOnAwake == true)
        {
            HideShadowMesh();
        }
    }

    private void Start()
    {
        EventManager.Instance.PLAYERS_PlayerSpawned += On_PLAYERS_PlayerSpawned;
        EventManager.Instance.APP_AppStateUpdated.AddListener(On_APP_AppStateUpdated);
        //EventManager.Instance.AR_StagePlaced += On_AR_StagePlaced;         
    }

    private void On_AR_StagePlaced()
    {
        HideShadowMesh();
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.PLAYERS_PlayerSpawned -= On_PLAYERS_PlayerSpawned;
            EventManager.Instance.APP_AppStateUpdated.RemoveListener(On_APP_AppStateUpdated);
            //EventManager.Instance.AR_StagePlaced -= On_AR_StagePlaced;
        }
    }

    private void On_APP_AppStateUpdated(StateUpdatedEventHandle<EAppState> eventHandle)
    {
        if (eventHandle.NewState == EAppState.IN_GAME_IN_RUNNING)
        {
            HideShadowMesh();
        }
    }

    private void On_PLAYERS_PlayerSpawned(EPlayerID playerID)
    {
        if (playerID == ToSpawnPlayerID)
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
