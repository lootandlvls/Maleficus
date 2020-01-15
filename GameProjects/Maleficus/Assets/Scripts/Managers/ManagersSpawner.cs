using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using static Maleficus.MaleficusConsts;

/// <summary>
/// Utility class that makes it possible to start and test any scene (instead of entry scene) by spawning needed managers if they are not found in the scene.
/// </summary>
public class ManagersSpawner : MotherOfManagers
{
    [Separator("Managers Spawner Specific")]
    [SerializeField] private EScene currentScene = EScene.NONE;

    protected override void Awake()
    {
        // Do not call base.Awake() to prevent setting Instance of MotherOfManagers to this one
        InitializeComponents();
        InitializeObjecsInScene();

        MotherOfManagers managersInstance = FindObjectOfType<MotherOfManagers>();
        if ((managersInstance == null) || (managersInstance == this))
        {
            // Load managers prefab 
            GameObject prefabToSpawn;
            if (IsServer == true)
            {
                prefabToSpawn = Resources.Load<GameObject>(PATH_MANAGERS_SERVER);
            }
            else // Client
            {
                prefabToSpawn = Resources.Load<GameObject>(PATH_MANAGERS_CLIENT);
            }

            // Set up Mother Of Managers Profile
            managersInstance = prefabToSpawn.GetComponent<MotherOfManagers>();
            // Networking
            managersInstance.IsServer = IsServer;
            managersInstance.ConnectionMode = ConnectionMode;
            // Input
            managersInstance.InputMode = InputMode;
            managersInstance.IsConnectControllerInAnyState = IsConnectControllerInAnyState;
            managersInstance.CanDebugButtonEvents = CanDebugButtonEvents;
            managersInstance.CanDebugJoystickEvents = CanDebugJoystickEvents;
            // Game
            managersInstance.IsUseDebugGameMode = IsUseDebugGameMode;
            managersInstance.DebugGameMode = DebugGameMode;
            // Player
            managersInstance.IsJoinAndSpawnPlayerOnControllerConnect = IsJoinAndSpawnPlayerOnControllerConnect;
            managersInstance.IsSpawnRemainingAIPlayersOnGameStart = IsSpawnRemainingAIPlayersOnGameStart;
            managersInstance.IsSpawnTouchAsPlayer1 = IsSpawnTouchAsPlayer1;
            managersInstance.IsSpawnGhostPlayerPositionsIfNotFound = IsSpawnGhostPlayerPositionsIfNotFound;
            // Spell
            managersInstance.IsLoadDebugSpells = IsLoadDebugSpells;
            // Debug
            managersInstance.IsDebugLogEvents = IsDebugLogEvents;
            managersInstance.IsReduceLightIntensityOnSceneStart = IsReduceLightIntensityOnSceneStart;

            // Spawn managers prefab
            GameObject spawnedObject = Instantiate(prefabToSpawn);

            // Set up current scene
            AppStateManager appStateManager = spawnedObject.GetComponentInChildren<AppStateManager>();
            appStateManager.SetUpDebugStartScene(currentScene);

        }
    }
}
