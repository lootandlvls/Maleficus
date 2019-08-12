using System.Collections;
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
    [SerializeField] public bool IsSpawnGhostPlayerPositionsIfNotFound = false;

    [Header ("AR")]
    [SerializeField] public bool IsARGame = false;
    [SerializeField] public EPlacementMethod ARPlacementMethod;
    [SerializeField] public EEnemyMovementMethod EnemiesMovementMethod;

    [Header ("Debug")]
    [SerializeField] public bool IsDebugLogEvents = false;
    [SerializeField] public bool IsReduceLightIntensityOnSceneStart = false;

    protected override void Awake()
    {
        base.Awake();

        InitializeManagers();
    }

    private void Start()
    {
        EventManager.Instance.APP_SceneChanged.AddListener(On_APP_SceneChanged);
    }


    public override void Initialize()
    {

    }

    private void On_APP_SceneChanged(BasicEventHandle<EScene> eventHandle)
    {
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
