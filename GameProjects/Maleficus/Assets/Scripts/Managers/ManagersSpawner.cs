using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class that makes it possible to start and test any scene (instead of entry scene) by spawning needed managers if they are not found in the scene.
/// </summary>
public class ManagersSpawner : MotherOfManagers
{
    [Header ("Managers Spawner Specific")]
    [SerializeField] private EScene currentScene;

    protected override void Awake()
    {
        // Do not call base.Awake() to prevent setting Instance of MotherOfManagers to this one

        MotherOfManagers managersInstance = FindObjectOfType<MotherOfManagers>();
        if ((managersInstance == null) || (managersInstance == this))
        {
            // Spawn Managers
            GameObject spawnedObject = Instantiate(Resources.Load<GameObject>(MaleficusConsts.PATH_MANAGERS));

            // Set up Mother Of Managers Profile
            managersInstance = spawnedObject.GetComponent<MotherOfManagers>();
            managersInstance.IsServer = IsServer;
            managersInstance.ConnectMode = ConnectMode;
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

            // Set up current scene
            AppStateManager appStateManager = spawnedObject.GetComponentInChildren<AppStateManager>();
            appStateManager.SetUpDebugStartScene(currentScene);

        }
    }
}
