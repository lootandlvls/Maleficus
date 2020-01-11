
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : AbstractSingletonManager<SoundManager>
{

    protected override void Start()
    {
        base.Start();

        //AppStateManager.Instance.AppStateUpdated += OnAppStateUpdated;
        EventManager.Instance.APP_AppStateUpdated.AddListener(OnAppStateUpdated);

        EventManager.Instance.SPELLS_SpellSpawned += On_SPELLS_SpellSpawned;
        EventManager.Instance.SPELLS_SpellHitEnemy += On_SPELLS_SpellHitEnemy;
    }


    //public SoundObject SpawnSoundObject(AudioClip audioClip, bool isLoop = false)
    //{
    //    GameObject gameObject = new GameObject("SO_" + audioClip.name);
    //    //SoundObject soundObject = Instantiate<SoundObject>()
    //}

    //public SoundObject SpawnSoundObject(AudioClip audioClip, Vector3 position, bool isLoop = false)
    //{

    //}



    private void On_SPELLS_SpellHitEnemy(IEnemy obj)
    {
        
    }



    private void On_SPELLS_SpellSpawned(ISpell castedSpell, EPlayerID castingPlayer , ESpellSlot spellSlot)
    {

    }


    private void OnAppStateUpdated(Event_StateUpdated<EAppState> eventHandle)
    {
        //switch (eventHandle.NewState)
        //{
        //    case EAppState.IN_MENU_IN_MAIN:
        //        ambiamce_AudioSource.Play();
        //        break;

        //    case EAppState.IN_GAME_IN_RUNNING:
        //        ambiamce_AudioSource.Stop();
        //        PlayFightingMusic();
        //        break;

        //    case EAppState.IN_GAME_IN_ENDED:
        //        fighting_AudioSource.Stop();
        //        gameOver_AudioSource.Play();
        //        break;

        //}
    }

    //    private void PlayFightingMusic()
    //    {

    //        StartCoroutine(PlayFightingCoroutine());
    //    }

    //    private IEnumerator PlayFightingCoroutine()
    //    {
    //        startFighting__AudioSource.Play();
    //        yield return new WaitForSeconds(startFighting__AudioSource.clip.length);
    //        fighting_AudioSource.Play();
    //    }
}
