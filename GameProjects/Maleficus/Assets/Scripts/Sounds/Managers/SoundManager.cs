
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : AbstractSingletonManager<SoundManager>
{

    protected override void Start()
    {
        base.Start();


    }

    public void SpawnSoundObject(AudioClip audioClipToPlay = null, bool destroyWhenFinished = true, bool isLoop = false)
    {
        if (IS_NULL(audioClipToPlay))
        {
            GameObject gameObject = new GameObject("SO_" + audioClipToPlay.name);
            //SoundObject soundObjec = Ins


        }
    }




    
}
