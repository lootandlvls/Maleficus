using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MaleficusMonoBehaviour
{
    private AudioSource myAudioSource;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myAudioSource = GetComponentWithCheck<AudioSource>();
    }

    public void PlaySound(bool is3D, bool isLoop = false)
    {

    }
}
