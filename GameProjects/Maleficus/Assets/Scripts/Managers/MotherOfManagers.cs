using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherOfManagers: AbstractSingletonManager<MotherOfManagers>
{
    // When adding a new attribute here, remember to add profile setup in SpawnManager class

    [Header("Server AppState")]
    [SerializeField] public bool IsServer = false;

    [Header("Networking")]
    [SerializeField] public EConnectionMode ConnectMode = EConnectionMode.EVRYTHING_LOCAL;

    [Header ("Input")]
    [SerializeField] public EInputMode InputMode = EInputMode.CONTROLLER;

    [Header ("Player")]
    [SerializeField] public bool IsSpawnPlayerOnConnect = false;
    [SerializeField] public bool IsSpawnAllPlayers = false;
    [SerializeField] public bool IsSpawnTouchAsPlayer1 = false;
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


    public override void OnSceneStartReinitialize()
    {

    }

    private void On_APP_SceneChanged(Event_AbstractHandle<EScene> eventHandle)
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

    public string ServerIP
    {
        get
        {
            switch (ConnectMode)
            {
                case EConnectionMode.LOCAL_SERVER:
                    return MaleficusConsts.LOCAL_SERVER_IP;

                case EConnectionMode.CLOUD_SERVER:
                    return MaleficusConsts.CLOUD_SERVER_IP;

                default:
                    return "";
            }
        }
    }
}
