using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;

public class ARManager : AbstractSingletonManager<ARManager>
{
    public float SizeFactor { get { return sizeFactor; } }

    private ContentPositioningBehaviour[] contentPositionings;
    private AnchorInputListenerBehaviour[] anchorInputListeners;

    private ARLockButton lockButton;

    private float sizeFactor;

    private bool isAnchorListeningActive = true;

    protected override void Awake()
    {
        base.Awake();

        FindAndBindButtonActions();
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
        isAnchorListeningActive = isActive;

        foreach (AnchorInputListenerBehaviour listener in anchorInputListeners)
        {
            listener.enabled = isActive;
        }

        if (isActive == true)
        {
            lockButton.SetIsUnlocked();
        }
        else
        {
            lockButton.SetIsLocked();
        }
    }

    protected override void FindAndBindButtonActions()
    {
        base.FindAndBindButtonActions();

        contentPositionings = FindObjectsOfType<ContentPositioningBehaviour>();
        foreach (ContentPositioningBehaviour cpb in contentPositionings)
        {
            cpb.OnContentPlaced.AddListener(OnContentPlaced);
        }

        AugmentedStage[] augmentedStages = FindObjectsOfType<AugmentedStage>();
        foreach (AugmentedStage augmentedStage in augmentedStages)
        {
            sizeFactor = augmentedStage.transform.localScale.x;
            break;
        }

        anchorInputListeners = FindObjectsOfType<AnchorInputListenerBehaviour>();

        ARLockButton[] arLockButtons = FindObjectsOfType<ARLockButton>();
        foreach (ARLockButton arLockButton in arLockButtons)
        {
            if (lockButton == null)
            {
                lockButton = arLockButton;
                lockButton.SetIsUnlocked();

                lockButton.ActionButtonPressed += () =>
                {
                    SetAnchorsInputActive(!isAnchorListeningActive);
                };
            }
            
            break;
        }
    }
}

