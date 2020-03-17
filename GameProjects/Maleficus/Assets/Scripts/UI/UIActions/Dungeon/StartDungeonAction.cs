using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StartDungeonAction : AbstractUIAction
{
    public event Action<EDungeonID> StartDungeonPressed;

    [SerializeField] private EDungeonID dungeonID = EDungeonID.NONE;

    protected override void Execute()
    {
        base.Awake();

        if (StartDungeonPressed != null)
        {
            StartDungeonPressed.Invoke(dungeonID);
        }
    }
}
