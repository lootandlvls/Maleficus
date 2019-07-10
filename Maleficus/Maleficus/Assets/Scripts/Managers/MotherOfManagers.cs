using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherOfManagers: AbstractSingletonManager<MotherOfManagers>
{

    [Header ("App State")]
    [SerializeField] public EAppState DebugStartState = EAppState.IN_MENU_IN_MAIN;

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

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        EventManager.Instance.APP_SceneChanged += One_APP_SceneChanged; 
    }

    private void One_APP_SceneChanged(EScene newScene)
    {
        InitializeManagers();
    }


    private void InitializeManagers()
    {
        Debug.Log("Intializing managers");
        AbstractManager[] abstractManagers = FindObjectsOfType<AbstractManager>();
        foreach (AbstractManager abstractManager in abstractManagers)
        {
            Debug.Log("Initializing " + abstractManager.name);
            if (abstractManager != this)
            {
                abstractManager.Initialize();
            }
        }
    }

}
