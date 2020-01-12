using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;

public class SoundObject : MaleficusMonoBehaviour
{
    public event Action SoundFinishedPlayed;

    [Separator("Sound Object")]
    [SerializeField] private AudioClip audioClip;

    private AudioSource myAudioSource;
    private IEnumerator DestroyAfterDelayEnumerator;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myAudioSource = GetComponentWithCheck<AudioSource>();
    }

    /// <summary>
    /// Play 2D sound
    /// </summary>
    /// <param name="audioClipToPlay"></param>
    /// <param name="destroyWhenFinished"></param>
    /// <param name="isLoop"></param>
    public void PlaySound(AudioClip audioClipToPlay = null, bool destroyWhenFinished = true, bool isLoop = false)
    {
        if (IS_NOT_NULL(myAudioSource))
        {
            myAudioSource.loop = isLoop;

            // Assign audio clip
            if (audioClipToPlay != null)
            {
                myAudioSource.clip = audioClipToPlay;
            }
            else
            {
                myAudioSource.clip = audioClip;
            }
            if (IS_NOT_NULL(myAudioSource.clip))
            {
                // Play sound
                myAudioSource.Play();
                
                // Destroy when finished
                if (destroyWhenFinished)
                {
                    StartNewCoroutine(ref DestroyAfterDelayEnumerator, DestroyAfterDelayCoroutine(audioClip.length));
                }
            }
        }
    }

    /// <summary>
    /// Play 3D sound at given position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="audioClipToPlay"></param>
    /// <param name="destroyWhenFinished"></param>
    /// <param name="isLoop"></param>
    public void PlaySound(Vector3 position, AudioClip audioClipToPlay = null, bool destroyWhenFinished = true, bool isLoop = false)
    {
        if (myAudioSource != null)
        {
            myAudioSource.spatialBlend = 1.0f;
            transform.position = position;

            PlaySound(audioClipToPlay, destroyWhenFinished, isLoop);
        }
    }

    /// <summary>
    /// Play 3D sound and attach it to given parent
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="audioClipToPlay"></param>
    /// <param name="destroyWhenFinished"></param>
    /// <param name="isLoop"></param>
    public void PlaySound(Transform parent, AudioClip audioClipToPlay = null, bool destroyWhenFinished = true, bool isLoop = false)
    {
        if (myAudioSource != null)
        {
            myAudioSource.spatialBlend = 1.0f;
            transform.parent = parent;

            PlaySound(audioClipToPlay, destroyWhenFinished, isLoop);
        }
    }


    private IEnumerator DestroyAfterDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

}
