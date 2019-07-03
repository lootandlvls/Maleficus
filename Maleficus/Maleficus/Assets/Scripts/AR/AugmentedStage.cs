using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class AugmentedStage : MonoBehaviour
{

    public void OnAutomaticHitTest(HitTestResult hitResult)
    {
        transform.rotation = hitResult.Rotation;
    }


}
