using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class MotherOfManagers: AbstractSingletonManager<MotherOfManagers>
{
    // When adding a new attribute here, remember to add profile setup in SpawnManager class

    [Separator("Networking")]
    [SerializeField] public bool IsServer = false;
    [ConditionalField(nameof(IsServer), inverse:true)] [SerializeField] public EConnectionMode ConnectionMode = EConnectionMode.EVRYTHING_LOCAL;

    [Separator("Input")]
    [SerializeField] public EInputMode InputMode = EInputMode.CONTROLLER;

    [Separator("Player")]
    [SerializeField] public bool IsSpawnPlayerOnConnect = false;
    [SerializeField] public bool IsSpawnAllPlayers = false;
    [SerializeField] public bool IsSpawnTouchAsPlayer1 = false;
    [SerializeField] public bool IsSpawnGhostPlayerPositionsIfNotFound = false;

    [Separator("AR")]
    [SerializeField] public bool IsARGame = false;
    [ConditionalField(nameof(IsARGame))] [SerializeField] public EPlacementMethod ARPlacementMethod;
    [ConditionalField(nameof(IsARGame))] [SerializeField] public EEnemyMovementMethod EnemiesMovementMethod;

    [Separator("Debug")]
    [SerializeField] public bool IsDebugLogEvents = true;
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
            switch (ConnectionMode)
            {
                case EConnectionMode.LOCAL_SERVER:
                    Debug.Log(MaleficusConsts.LOCAL_SERVER_IP);
                    return MaleficusConsts.LOCAL_SERVER_IP;

                case EConnectionMode.CLOUD_SERVER:
                    return MaleficusConsts.CLOUD_SERVER_IP;

                default:
                    return "0.0.0.0";
            }
        }
    }
}


//[CustomEditor(typeof(MotherOfManagers))]
//public class MotherOfManagersEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        var myScript = target as MotherOfManagers;

//        myScript.IsServer = GUILayout.Toggle(myScript.IsServer, "IsServer");

//        if (myScript.IsServer)
//        {
//            myScript.i = EditorGUILayout.IntSlider("I field:", myScript.i, 1, 100);

//        }

//    }
//}