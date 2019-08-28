using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherOfManagers: AbstractSingletonManager<MotherOfManagers>
{
    [Header("Server AppState")]
    [SerializeField] public bool IsServer = false;

    [Header ("Input")]
    [SerializeField] public EInputMode InputMode = EInputMode.CONTROLLER;

    [Header ("Player")]
    [SerializeField] public bool IsSpawnPlayerOnConnect = false;
    [SerializeField] public bool IsSpawnAllPlayers = false;
    [SerializeField] public bool IsSpawnGhostPlayerPositionsIfNotFound = false;
    [SerializeField] public bool IsSpawnTouchAsPlayer1 = false;

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


    public override void OnSceneStartReinitialize()
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
                abstractManager.OnSceneStartReinitialize();
            }
        }
    }

    
}
