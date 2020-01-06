﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIAction : AbstractUIAction
{
    public override void Execute()
    {
        base.Execute();

        EventManager.Instance.Invoke_PLAYERS_AllPlayersReady();
    }
}
