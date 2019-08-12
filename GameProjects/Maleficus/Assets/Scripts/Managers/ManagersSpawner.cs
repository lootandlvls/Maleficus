using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class that makes it possible to start and test any scene (instead of entry scene) by spawning needed managers if they are not found in the scene.
/// </summary>
public class ManagersSpawner : MonoBehaviour
{
    [SerializeField] private EScene currentScene;

    private void Awake()
    {
        MotherOfManagers managersInstance = FindObjectOfType<MotherOfManagers>();
        if (managersInstance == null)
        {
            GameObject spawnedObject = Instantiate(Resources.Load<GameObject>(MaleficusConsts.PATH_MANAGERS));
            AppStateManager appStateManager = spawnedObject.GetComponentInChildren<AppStateManager>();
            appStateManager.SetUpDebugStartScene(currentScene);
        }
    }
}
