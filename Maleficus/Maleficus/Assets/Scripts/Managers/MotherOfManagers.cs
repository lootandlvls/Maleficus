﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherOfManagers: AbstractSingletonManager<MotherOfManagers>
{

    //[Header ("States")]
    //[Tooltip("Scene to load if currently in the ENTRY scene.")]
    //[SerializeField] public EScene StartSceneOnEntry        = EScene.MENU;
    //[Tooltip("Keep NONE if you want to use default start state from AppStateManager")]
    //[SerializeField] public EAppState DebugStartAppState    = EAppState.NONE;           
    //[Tooltip("Keep NONE if you want to use default start state from UIStateManagers")]
    //[SerializeField] public EMenuState DebugStartMenuState  = EMenuState.NONE;

    [Header ("Input")]
    [SerializeField] public EInputMode InputMode = EInputMode.CONTROLLER;

    [Header ("Player")]
    [SerializeField] public bool IsSpawnPlayerOnConnect = false;
    [SerializeField] public bool IsSpawnAllPlayers = false;
    [SerializeField] public bool IsSpawnARPlayers = false;

    [Header ("Debug")]
    [SerializeField] public bool IsDebugLogEvents = false;

    protected override void Awake()
    {
        base.Awake();

        InitializeManagers();
    }

    private void Start()
    {
        EventManager.Instance.APP_SceneChanged += On_APP_SceneChanged; 
    }

    private void On_APP_SceneChanged(EScene newScene)
    {
        // Find and destroy other Managers in the scene
        //MotherOfManagers[] otherManagers = FindObjectsOfType<MotherOfManagers>();
        //foreach (MotherOfManagers motherOfManager in otherManagers)
        //{
        //    if (motherOfManager != this)
        //    {
        //        Destroy(motherOfManager.gameObject);
        //    }
        //}

        InitializeManagers();
    }


    private void InitializeManagers()
    {
        Debug.Log("Intializing managers");
        AbstractManager[] abstractManagers = FindObjectsOfType<AbstractManager>();
        foreach (AbstractManager abstractManager in abstractManagers)
        {
            if (abstractManager != this)
            {
                abstractManager.Initialize();
            }
        }
    }

}