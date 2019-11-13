using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using static Maleficus.MaleficusConsts;

public class MotherOfManagers: AbstractSingletonManager<MotherOfManagers>
{
    // When adding a new attribute here, remember to add profile setup in SpawnManager class

    [Separator("Networking")]
    [SerializeField] public bool IsServer = false;
    [ConditionalField(nameof(IsServer), inverse:true)] [SerializeField] public EConnectionMode ConnectionMode = EConnectionMode.PLAY_OFFLINE;

    [Separator("Input")]
    [SerializeField] public EInputMode InputMode = EInputMode.CONTROLLER;
    [ConditionalField(nameof(InputMode), false, EInputMode.CONTROLLER)] [SerializeField] public bool IsConnectControllerInAnyState = false;

    [Separator("Player")]
    [SerializeField] public bool IsSpawnPlayerOnConnect = false;
    [SerializeField] public bool IsSpawnAllPlayers = false;
    [SerializeField] public bool IsSpawnTouchAsPlayer1 = false;
    [SerializeField] public bool IsSpawnGhostPlayerPositionsIfNotFound = false;

    [Separator("Debug")]
    [SerializeField] public bool IsDebugLogEvents = true;
    [SerializeField] public bool IsReduceLightIntensityOnSceneStart = false;


    protected override void Awake()
    {
        base.Awake();

        InitializeManagers();
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.APP_SceneChanged.AddListener(On_APP_SceneChanged);

    }

    public override void OnSceneStartReinitialize()
    {

    }

    private void On_APP_SceneChanged(Event_GenericHandle<EScene> eventHandle)
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
                    return LOCAL_SERVER_IP;
                case EConnectionMode.CLOUD_SERVER:
                    return CLOUD_SERVER_IP;
                case EConnectionMode.BNJMO_SERVER:
                    return BNJMO_SERVER_IP;
                case EConnectionMode.GOOGLE_CLOUD_SERVER:
                    return GOOGLE_CLOUD_SERVER_IP;
                case EConnectionMode.PLAY_OFFLINE:
                    return PLAY_OFFLINE_IP;
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