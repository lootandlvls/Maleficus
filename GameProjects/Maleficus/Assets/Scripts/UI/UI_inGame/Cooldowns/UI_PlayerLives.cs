using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerLives : MaleficusMonoBehaviour
{

    public int LiveNumber { get { return liveNumber; } }
    [SerializeField] int liveNumber;
    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

      
    }

}
