using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.AI;

public class ARManager : AbstractSingletonManagerWithStateMachine<ARManager, EARState>
{
    public float SizeFactor { get { return sizeFactor; } }

    private AnchorBehaviour midAirAnchorBehaviour;
    private ContentPositioningBehaviour[] contentPositionings;
    private AnchorInputListenerBehaviour[] anchorInputListeners;

    private ARLockButton lockButton;
    private AugmentedStage augmentedStage;

    private float sizeFactor = 1.0f;

    private bool isAnchorListeningActive = true;

    protected override void Awake()
    {
        base.Awake();

        startStates = MaleficusConsts.START_AR_STATES;
        debugStateID = 89;
    }

    protected override void Start()
    {
        base.Start();

        StateUpdateEvent += EventManager.Instance.AR_ARStateUpdated.Invoke;
    }

    public override void OnSceneStartReinitialize()
    {
        FindReferencesInScene();
    }

    protected override void Update()
    {
        base.Update();

        if (midAirAnchorBehaviour != null)
        {
            UpdateState((EARState)((int)midAirAnchorBehaviour.CurrentStatus + 1));
        }
    }

    private void OnContentPlaced(GameObject placedContent)
    {
        switch (MotherOfManagers.Instance.ARPlacementMethod)
        {
            case EPlacementMethod.MID_AIR:
                if (placedContent.name == "Mid Air Stage")
                {
                    SetAnchorsInputActive(false);
                }
                break;


            case EPlacementMethod.ON_PLANE:
                if (placedContent.name == "Ground Plane Stage")
                {
                    SetAnchorsInputActive(false);
                    augmentedStage.ShowStage();
                }
                break;
        }
        EventManager.Instance.AR_ARStagePlaced.Invoke(new ARStagePlacedEventHandle());
       // EventManager.Instance.Invoke_AR_StagePlaced();
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

    private void FindReferencesInScene()
    {
        midAirAnchorBehaviour = FindObjectOfType<AnchorBehaviour>();

        contentPositionings = FindObjectsOfType<ContentPositioningBehaviour>();
        foreach (ContentPositioningBehaviour cpb in contentPositionings)
        {
            cpb.OnContentPlaced.AddListener(OnContentPlaced);
        }

        AugmentedStage[] augmentedStages = FindObjectsOfType<AugmentedStage>();
        foreach (AugmentedStage augmentedStage in augmentedStages)
        {
            this.augmentedStage = augmentedStage;
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

