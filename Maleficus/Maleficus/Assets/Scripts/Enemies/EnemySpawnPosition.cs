using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPosition : MonoBehaviour
{

    public Vector3 Position { get { return transform.position; } }

    [SerializeField] private bool isHideOnAwake = true;

    private void Awake()
    {
        if (isHideOnAwake == true)
        {
            GetComponent<Renderer>().enabled = false;
        }
    }
}
