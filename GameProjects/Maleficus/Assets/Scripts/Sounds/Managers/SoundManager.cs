
using System.Collections.Generic;
using UnityEngine;

using static Maleficus.Consts;

public class SoundManager : AbstractSingletonManager<SoundManager>
{

    private List<SoundObject> aliveSoundObjects = new List<SoundObject>();
    private SoundObject soundObjectPrefab;

    protected override void Awake()
    {
        base.Awake();

        soundObjectPrefab = Resources.Load<SoundObject>(PATH_SOUND_OBJECT);
        IS_NOT_NULL(soundObjectPrefab);
    }

    public SoundObject SpawnSoundObject(AudioClip audioClipToPlay, bool destroyWhenFinished = true, bool isLoop = false)
    {
        SoundObject soundObject = SpawnSoundObject(audioClipToPlay);
        if (soundObject)
        {
            soundObject.PlaySound(audioClipToPlay, destroyWhenFinished, isLoop);
        }
        return soundObject;
    }

    public SoundObject SpawnSoundObject(Vector3 position, AudioClip audioClipToPlay, bool destroyWhenFinished = true, bool isLoop = false)
    {
        SoundObject soundObject = SpawnSoundObject(audioClipToPlay);
        if (soundObject)
        {
            soundObject.PlaySound(position, audioClipToPlay, destroyWhenFinished, isLoop);
        }
        return soundObject;
    }

    public SoundObject SpawnSoundObject(Transform transform, AudioClip audioClipToPlay, bool destroyWhenFinished = true, bool isLoop = false)
    {
        SoundObject soundObject = SpawnSoundObject(audioClipToPlay);
        if (soundObject)
        {
            soundObject.PlaySound(transform, audioClipToPlay, destroyWhenFinished, isLoop);
        }
        return soundObject;
    }

    private SoundObject SpawnSoundObject(AudioClip audioClipToPlay)
    {
        if ((audioClipToPlay != null)
            && (IS_NOT_NULL(soundObjectPrefab)))
        {
            SoundObject soundObject = Instantiate(soundObjectPrefab);
            soundObject.gameObject.name = "SO_" + audioClipToPlay.name;
            aliveSoundObjects.Add(soundObject);
            soundObject.SoundObjectWillGetDestroyed += On_SoundObject_SoundObjectWillGetDestroyed;
            return soundObject;
        }
        return null;
    }


    private void On_SoundObject_SoundObjectWillGetDestroyed(SoundObject soundObject)
    {
        if (IS_VALUE_CONTAINED(aliveSoundObjects, soundObject))
        {
            aliveSoundObjects.Remove(soundObject);
        }
    }
}
