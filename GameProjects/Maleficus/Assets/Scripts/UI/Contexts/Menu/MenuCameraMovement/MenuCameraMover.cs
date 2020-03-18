using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BNJMO;



public class MenuCameraMover : BNJMOBehaviour
{
    public event Action<EMenuState> CameraMovementEnded;

    private Dictionary<EMenuState, MenuCameraTransform> cameraTransforms = new Dictionary<EMenuState, MenuCameraTransform>();

    private AnimationLerpTransform lerpTransform;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        lerpTransform = GetComponentWithCheck<AnimationLerpTransform>();
    }

    protected override void InitializeObjecsInScene()
    {
        base.InitializeObjecsInScene();

        // Find Menu Camera Transform in scene
        foreach (MenuCameraTransform menuCameraTransform in FindObjectsOfType<MenuCameraTransform>())
        {
            if (IS_KEY_NOT_CONTAINED(cameraTransforms, menuCameraTransform.MenuState))
            {
                cameraTransforms.Add(menuCameraTransform.MenuState, menuCameraTransform);
            }
        }
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.UI_MenuStateUpdated.Event += On_UI_MenuStateUpdated_Event;

        if (IS_NOT_NULL(lerpTransform))
        {
            lerpTransform.AnimationEnded += On_LerpTransform_AnimationEnded;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (lerpTransform)
        {
            lerpTransform.AnimationEnded -= On_LerpTransform_AnimationEnded;
        }
    }

    private void On_UI_MenuStateUpdated_Event(Event_StateUpdated<EMenuState> eventHandle)
    {
        EMenuState newMenuState = eventHandle.NewState;

        if (cameraTransforms.ContainsKey(newMenuState))
        {
            MenuCameraTransform newCameraTransform = cameraTransforms[newMenuState];
            lerpTransform.StartAnimation(transform, transform, newCameraTransform.transform, newCameraTransform.TransitionTime);
        }
    }

    private void On_LerpTransform_AnimationEnded(AnimationLerp<Transform> animationLerp)
    {
        if (UIManager.IsInstanceSet)
        {
            InvokeEventIfBound(CameraMovementEnded, UIManager.Instance.CurrentState);
        }
    }
}
