﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class AugmentedStage : MonoBehaviour
{
    public void OnAutomaticHitTest(HitTestResult hitResult)
    {
        if (MotherOfManagers.Instance.ARPlacementMethod == EPlacementMethod.MID_AIR)
        {
            transform.rotation = hitResult.Rotation;
        }
    }

    public void ShowStage()
    {
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }
    }
}