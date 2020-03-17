using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerLives : BNJMOBehaviour
{

    public int LiveNumber { get { return liveNumber; } }
    [SerializeField] int liveNumber;
    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

      
    }

}
