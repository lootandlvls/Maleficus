using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BNJMO;

[Serializable]
public struct MenuCameraTransform
{
    public EMenuState MenuState;
    public Transform CameraTransform;
    public float TransitionTime;
}

public class MenuCameraMover : BNJMOBehaviour
{
    [SerializeField] MenuCameraTransform[] cameraTransforms;

    private AnimationLerpTransform lerpTransform;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        lerpTransform = GetComponentWithCheck<AnimationLerpTransform>();
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.UI_MenuStateUpdated.Event += On_UI_MenuStateUpdated_Event;
    }

    private void On_UI_MenuStateUpdated_Event(Event_StateUpdated<EMenuState> eventHandle)
    {
        EMenuState newMenuState = eventHandle.NewState;

        if ((newMenuState == EMenuState.IN_MENU_MAIN)
            || (newMenuState == EMenuState.IN_MENU_IN_SPELL_SELECTION))
        {
            MenuCameraTransform newCameraTransform = GetMenuCameraTransform(newMenuState);
            if (IS_NOT_NONE(newCameraTransform.MenuState))
            {
                lerpTransform.StartAnimation(transform, transform, newCameraTransform.CameraTransform, newCameraTransform.TransitionTime);
            }
        }
    }


    private MenuCameraTransform GetMenuCameraTransform(EMenuState menuState)
    {
        foreach (MenuCameraTransform cameraTransform in cameraTransforms)
        {
            if (cameraTransform.MenuState == menuState)
            {
                return cameraTransform;
            }
        }
        return new MenuCameraTransform();
    }
}
