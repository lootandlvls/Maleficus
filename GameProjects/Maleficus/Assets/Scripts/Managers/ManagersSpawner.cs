using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

/// <summary>
/// Utility class that makes it possible to start and test any scene (instead of entry scene) by spawning needed managers if they are not found in the scene.
/// </summary>
public class ManagersSpawner : MotherOfManagers
{
    [Separator("Managers Spawner Specific")]
    [SerializeField] private EScene currentScene;

    protected override void Awake()
    {
        // Do not call base.Awake() to prevent setting Instance of MotherOfManagers to this one

        MotherOfManagers managersInstance = FindObjectOfType<MotherOfManagers>();
        if ((managersInstance == null) || (managersInstance == this))
        {
            // Load managers prefab 
            GameObject prefabToSpawn;
            if (IsServer == true)
            {
                prefabToSpawn = Resources.Load<GameObject>(MaleficusConsts.PATH_MANAGERS_SERVER);
            }
            else // Client
            {
                prefabToSpawn = Resources.Load<GameObject>(MaleficusConsts.PATH_MANAGERS_CLIENT);
            }

            // Set up Mother Of Managers Profile
            managersInstance = prefabToSpawn.GetComponent<MotherOfManagers>();
            managersInstance.IsServer = IsServer;
            managersInstance.ConnectionMode = ConnectionMode;
            managersInstance.InputMode = InputMode;
            managersInstance.IsSpawnPlayerOnConnect = IsSpawnPlayerOnConnect;
            managersInstance.IsSpawnAllPlayers = IsSpawnAllPlayers;
            managersInstance.IsSpawnTouchAsPlayer1 = IsSpawnTouchAsPlayer1;
            managersInstance.IsSpawnGhostPlayerPositionsIfNotFound = IsSpawnGhostPlayerPositionsIfNotFound;
            managersInstance.IsARGame = IsARGame;
            managersInstance.ARPlacementMethod = ARPlacementMethod;
            managersInstance.EnemiesMovementMethod = EnemiesMovementMethod;
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
