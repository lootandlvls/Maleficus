using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.AI;

public class ARManager : AbstractSingletonManagerWithStateMachine<ARManager, EARState>
{
    public float SizeFactor                                 { get { return sizeFactor; } }
    public Transform AugmentedStageTransform                { get { return augmentedStage.transform; } }

    private AnchorBehaviour midAirAnchorBehaviour;
    private ContentPositioningBehaviour[] contentPositionings;
    private AnchorInputListenerBehaviour[] anchorInputListeners;
    private DefaultTrackableEventHandler trackableEventHandler;

    private ARLockButton lockButton;
    private ImageTrackedIndicator imageTrackedIndicator;
    private AugmentedStage augmentedStage;

    private float sizeFactor = 1.0f;

    private bool isAnchorListeningActive = true;

    private Vector3 stagePosition;
    private bool isStagePlaced = false;

    private Vector3 trackerPosition;
    private Vector3 trackerRotation;
    private bool isTrackerTracked = false;

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

        EventManager.Instance.AR_ARStagePlaced.AddListener          (On_AR_ARStagePlaced);
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
                    stagePosition = placedContent.transform.position;

                    // Broadcast event only first time both objects tracked
                    if ((isTrackerTracked == true) && (isStagePlaced == false))
                    {
                        BroadcastStagePosition();
                    }

                    isStagePlaced = true;
                }
                break;


            case EPlacementMethod.ON_PLANE:
                if (placedContent.name == "Ground Plane Stage")
                {
                    SetAnchorsInputActive(false);
                    augmentedStage.ShowStage();
                }
                break;

            case EPlacementMethod.IMAGE_TRACKER:
                if (placedContent.name == "ImageTarget")
                {
                    trackerPosition = placedContent.transform.position;
                    if (imageTrackedIndicator != null)
                    {
                        imageTrackedIndicator.SetIsTracked();
                    }
                }
                break;


        }


    }

    

    private void OnImageTracked(Transform trackerTransform)
    {
        trackerPosition = trackerTransform.position;
        trackerRotation = trackerTransform.rotation.eulerAngles;

        // Broadcast event only first time both objects tracked
        if ((isTrackerTracked == false) && (isStagePlaced == true))
        {
            BroadcastStagePosition();
        }

        isTrackerTracked = true;


    }

    private void SetAnchorsInputActive(bool isActive)
    {
        isAnchorListeningActive = isActive;

        foreach (AnchorInputListenerBehaviour listener in anchorInputListeners)
        {
            listener.enabled = isActive;
        }

        if (lockButton != null)
        {
            if (isActive == true)
            {
                lockButton.SetIsUnlocked();
            }
            else
            {
                lockButton.SetIsLocked();
            }
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
            if ((MotherOfManagers.Instance.ARPlacementMethod == EPlacementMethod.IMAGE_TRACKER)
                && (augmentedStage != null) && (augmentedStage.transform.parent != null))
            {
                sizeFactor *= augmentedStage.transform.parent.localScale.x;
            }
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

        imageTrackedIndicator = FindObjectOfType<ImageTrackedIndicator>();
        if (imageTrackedIndicator != null)
        {
            imageTrackedIndicator.SetIsUntracked();
        }


        trackableEventHandler = FindObjectOfType<DefaultTrackableEventHandler>();
        if (trackableEventHandler != null)
        {
            trackableEventHandler.ImageTracked += OnImageTracked;
        }
    }



    #region NETWORK
    private void On_AR_ARStagePlaced(NetEvent_ARStagePlaced eventHandle)
    {
        Vector3 trackerToStage = new Vector3
            (
            eventHandle.X_TrackerToStage,
            eventHandle.Y_TrackerToStage,
            eventHandle.Z_TrackerToStage
            );
        Vector3 trackerRotation = new Vector3
            (
            eventHandle.X_TrackerRotation,
            eventHandle.Y_TrackerRotation,
            eventHandle.Z_TrackerRotation
            );

        // Update local coordinate
        this.stagePosition = trackerPosition + trackerToStage;
        this.trackerRotation = trackerRotation;

        if (augmentedStage != null)
        {
            augmentedStage.transform.position = stagePosition;
        }
    }


    private void BroadcastStagePosition()
    {
        EClientID clientID = NetworkManager.Instance.OwnClientID;
        Vector3 trackerToStage = stagePosition - trackerPosition;
        NetEvent_ARStagePlaced eventHanlde = new NetEvent_ARStagePlaced(
            clientID,
            trackerRotation.x,
            trackerRotation.y,
            trackerRotation.z,
            trackerToStage.x,
            trackerToStage.y,
            trackerToStage.z);
        EventManager.Instance.AR_ARStagePlaced.Invoke(eventHanlde, EEventInvocationType.TO_SERVER_ONLY);
    }
    #endregion
}

