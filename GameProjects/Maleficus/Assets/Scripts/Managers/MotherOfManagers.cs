using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using static Maleficus.Consts;

public class MotherOfManagers: AbstractSingletonManager<MotherOfManagers>
{
    // When adding a new attribute here, remember to add profile setup in SpawnManager class

    [Separator("Networking")]
    [SerializeField] public bool IsServer = false;
    [ConditionalField(nameof(IsServer), inverse:true)] [SerializeField] public EConnectionMode ConnectionMode = EConnectionMode.PLAY_OFFLINE;

    [Separator("Input")]
    [SerializeField] public EInputMode InputMode = EInputMode.CONTROLLER;
    [ConditionalField(nameof(InputMode), false, EInputMode.CONTROLLER)] [SerializeField] public bool IsConnectControllerInAnyState = false;
    [SerializeField] public bool CanDebugButtonEvents = false;
    [SerializeField] public bool CanDebugJoystickEvents = false;

    [Separator("Game")]
    [SerializeField] public bool IsUseDebugGameMode = false;
    [ConditionalField(nameof(IsUseDebugGameMode))] [SerializeField] public EGameMode DebugGameMode = EGameMode.NONE;


    [Separator("Player")]
    [SerializeField] public bool IsJoinAndSpawnPlayerOnControllerConnect = false;
    [SerializeField] public bool IsSpawnRemainingAIPlayersOnGameStart = false;
    [SerializeField] public float MaximumNumberOfAIToSpawn = 4.0f;
    [SerializeField] public bool IsSpawnTouchAsPlayer1 = false;
    [SerializeField] public bool IsSpawnGhostPlayerPositionsIfNotFound = false;

    [Separator("Spell")]
    [SerializeField] public bool IsLoadDebugSpells = false;
    [SerializeField] public bool IsLimitMaxPushPower = false;

    [Separator("Debug")]
    [SerializeField] public bool IsDebugLogEvents = true;
    [SerializeField] public bool IsReduceLightIntensityOnSceneStart = false;

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