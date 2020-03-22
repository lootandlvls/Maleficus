﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BNJMO;



public class MenuCameraMover : BNJMOBehaviour
{
    public event Action<EMenuState> CameraMovementEnded;

    [SerializeField] private bool canBeStopped = false;
    
    private Dictionary<EMenuState, MenuCameraTransform> cameraTransforms = new Dictionary<EMenuState, MenuCameraTransform>();
    private AnimationLerpTransform cameraTransformLerp;
    private MenuCameraTransform lastCameraTransform;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        cameraTransformLerp = GetComponentWithCheck<AnimationLerpTransform>();
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

                if (menuCameraTransform.MenuState == EMenuState.NONE)
                {
                    lastCameraTransform = menuCameraTransform;
                }
            }
        }
    }


    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.UI_MenuStateUpdated.Event += On_UI_MenuStateUpdated_Event;
        EventManager.Instance.INPUT_ButtonPressed.Event += On_INPUT_ButtonPressed;

        if (IS_NOT_NULL(cameraTransformLerp))
        {
            cameraTransformLerp.AnimationEnded += On_LerpTransform_AnimationEnded;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.UI_MenuStateUpdated.Event -= On_UI_MenuStateUpdated_Event;
            EventManager.Instance.INPUT_ButtonPressed.Event -= On_INPUT_ButtonPressed;
        }

        if (cameraTransformLerp)
        {
            cameraTransformLerp.AnimationEnded -= On_LerpTransform_AnimationEnded;
        }
    }

    private void On_UI_MenuStateUpdated_Event(Event_StateUpdated<EMenuState> eventHandle)
    {
        EMenuState newMenuState = eventHandle.NewState;

        if (cameraTransforms.ContainsKey(newMenuState))
        {
            MenuCameraTransform newCameraTransform = cameraTransforms[newMenuState];
            cameraTransformLerp.StartValue = lastCameraTransform.transform;
            cameraTransformLerp.EndValue = newCameraTransform.transform;
            cameraTransformLerp.PlayTime = newCameraTransform.TransitionTime;
            cameraTransformLerp.StartAnimation();

            lastCameraTransform = newCameraTransform;
        }
    }

    private void On_INPUT_ButtonPressed(NetEvent_ButtonPressed eventHandle)
    {
        EInputButton inputButton = eventHandle.InputButton;
        EPlayerID playerID = Maleficus.Utils.GetPlayerIDFrom(eventHandle.SenderID);
        if ((canBeStopped == true)
            && (eventHandle.InputButton == EInputButton.CONFIRM)
            && (PlayerManager.Instance.HasPlayerJoined(playerID) == true))
        {
            cameraTransformLerp.StopAnimation(true);
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