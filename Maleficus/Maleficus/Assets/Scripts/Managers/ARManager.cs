using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;

public class ARManager : AbstractSingletonManager<ARManager>
{
    private ContentPositioningBehaviour[] contentPositionings;
    private AnchorInputListenerBehaviour[] anchorInputListeners;

    [SerializeField] private Text lockButtonText;

    private bool isAnchorListeningActive = true;

    protected override void Awake()
    {
        base.Awake();

        contentPositionings = FindObjectsOfType<ContentPositioningBehaviour>();
        foreach (ContentPositioningBehaviour cpb in contentPositionings)
        {
            cpb.OnContentPlaced.AddListener(OnContentPlaced);
        }

        anchorInputListeners = FindObjectsOfType<AnchorInputListenerBehaviour>();
    }

    private void OnContentPlaced(GameObject placedContent)
    {
        if (placedContent.name == "Mid Air Stage")
        {
            SetAnchorsInputActive(false);
        }
    }

    private void SetAnchorsInputActive(bool isActive)
    {
        foreach (AnchorInputListenerBehaviour listener in anchorInputListeners)
        {
            listener.enabled = isActive;
            isAnchorListeningActive = isActive;
        }

        if (isAnchorListeningActive == true)
        {
            lockButtonText.color = Color.green;
        }
        else
        {
            lockButtonText.color = Color.red;
        }
    }

    public void OnLockButtonPressed()
    {
        SetAnchorsInputActive(!isAnchorListeningActive);
    }
}

