﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuCameraTransform : BNJMOBehaviour
{
    public EMenuState MenuState = EMenuState.NONE;
    public float TransitionTime = 2.0f;

    public void SetCameraTransformToThis()
    {
        if (Camera.main)
        {
            Camera.main.transform.position = transform.position;
            Camera.main.transform.rotation = transform.rotation;
        }
    }

    public void SetThisToCameraTransform()
    {
        if (Camera.main)
        {
            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MenuCameraTransform))]
public class MenuCameraTransformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MenuCameraTransform myTarget = (MenuCameraTransform)target;

        if (GUILayout.Button("Set Camera Transform To This"))
        {
            myTarget.SetCameraTransformToThis();
        }

        if (GUILayout.Button("Set This To Camera Transform"))
        {
            myTarget.SetThisToCameraTransform();
        }
    }
}
#endif