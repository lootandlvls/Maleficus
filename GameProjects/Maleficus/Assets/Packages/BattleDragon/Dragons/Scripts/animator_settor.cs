﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class animator_settor : MonoBehaviour
{

    public string AnimatorName = "";
    public float time = 0.0f;

    // Use this for initialization
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (null != animator && "" != AnimatorName)
        {
            animator.Play(AnimatorName, -1, time);
            animator.speed = 0;

          //  animation[name].wrapMode = WrapMode.Loop;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
