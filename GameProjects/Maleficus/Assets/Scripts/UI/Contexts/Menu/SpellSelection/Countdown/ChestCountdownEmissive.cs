using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

public class ChestCountdownEmissive : BNJMOBehaviour
{
    private AnimationLerpColor animationLerpChestEmissive;
    private MeshRenderer meshRenderer;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        meshRenderer = GetComponentWithCheck<MeshRenderer>();
        animationLerpChestEmissive = GetComponent<AnimationLerpColor>();
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        SpellSelectionCountdown.Instance.CountdownProgressed += On_SpellSelectionCountdown_CountdownProgressed;  
    }

    private void On_SpellSelectionCountdown_CountdownProgressed(int remainingTime)
    {
        animationLerpChestEmissive.StartAnimation(meshRenderer, "_EmissionColor");
    }
}
