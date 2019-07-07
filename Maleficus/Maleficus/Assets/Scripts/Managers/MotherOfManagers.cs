using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherOfManagers : Singleton<MotherOfManagers>
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

}
