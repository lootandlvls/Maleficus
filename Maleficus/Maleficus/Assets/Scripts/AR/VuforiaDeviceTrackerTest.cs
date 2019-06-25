using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VuforiaDeviceTrackerTest : MonoBehaviour
{
    private void Awake()
    {
        VuforiaARController.Instance.RegisterVuforiaInitializedCallback(OnVuforiaInitialized);
    }

    private void OnVuforiaInitialized()
    {
        // VuforiaConfiguration.Instance.DeviceTracker.FusionMode = FusionProviderType. .OPTIMIZE_IMAGE_TARGETS_AND_VUMARKS;
        var deviceTracker = TrackerManager.Instance.InitTracker<PositionalDeviceTracker>();
        deviceTracker.Start();
    }
}
