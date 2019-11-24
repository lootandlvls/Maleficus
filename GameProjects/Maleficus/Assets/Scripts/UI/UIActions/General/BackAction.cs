using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BackAction : AbstractUIAction
{
    public bool IsStaysInSameAppState { get { return isStaysInSameAppState; } }

    [SerializeField] private bool isStaysInSameAppState = true; 

}
