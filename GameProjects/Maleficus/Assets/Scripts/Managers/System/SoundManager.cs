using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : AbstractSingletonManager<SoundManager>
{
    

    [SerializeField] private AudioSource ambiamce_AudioSource;
    [SerializeField] private AudioSource startFighting__AudioSource;
    [SerializeField] private AudioSource fighting_AudioSource;
    [SerializeField] private AudioSource gameOver_AudioSource;
    [SerializeField] private AudioSource gameWon_AudioSource;
    [SerializeField] private AudioSource Castspell_FireBall_AudioSource;
    [SerializeField] private AudioSource Hitspell_FireBall_AudioSource;
    [SerializeField] private AudioSource castSpell_AOE_AudioSource;
    [SerializeField] private AudioSource castSpell_Laser_AudioSource;
    [SerializeField] private AudioSource stagePlaced;


    private void Start()
    {
        //AppStateManager.Instance.AppStateUpdated += OnAppStateUpdated;
        EventManager.Instance.APP_AppStateUpdated.AddListener(OnAppStateUpdated);

        EventManager.Instance.SPELLS_SpellSpawned += On_SPELLS_SpellSpawned;
        EventManager.Instance.SPELLS_SpellHitEnemy += On_SPELLS_SpellHitEnemy;

        EventManager.Instance.AR_ARStagePlaced.AddListener(On_AR_ARStagePlaced);
    }

    private void On_AR_ARStagePlaced(ARStagePlacedEventHandle eventHandle)
    {
        stagePlaced.Play();
    }

    private void On_SPELLS_SpellHitEnemy(IEnemy obj)
    {
        
    }

    public override void OnSceneStartReinitialize()
    {

    }

    private void On_SPELLS_SpellSpawned(ISpell castedSpell, EPlayerID castingPlayer)
    {
        switch(castedSpell.SpellName)
        {
            case "S_AOE":
                castSpell_AOE_AudioSource.Play();
                break;

            case "FireBall_lvl_":
                Castspell_FireBall_AudioSource.Play();
                break;

            case "S_FireLaser":
                castSpell_Laser_AudioSource.Play();
                break;
        }
    }


    private void OnAppStateUpdated(StateUpdatedEventHandle<EAppState> eventHandle)
    {
        switch (eventHandle.NewState)
        {
            case EAppState.IN_MENU_IN_MAIN:
                ambiamce_AudioSource.Play();
                break;
            
            case EAppState.IN_GAME_IN_RUNNING:
                ambiamce_AudioSource.Stop();
                PlayFightingMusic();
                break;

            case EAppState.IN_GAME_IN_ENDED:
                fighting_AudioSource.Stop();
                gameOver_AudioSource.Play();
                break;

        }
    }

    private void PlayFightingMusic()
    {

        StartCoroutine(PlayFightingCoroutine());
    }

    private IEnumerator PlayFightingCoroutine()
    {
        startFighting__AudioSource.Play();
        yield return new WaitForSeconds(startFighting__AudioSource.clip.length);
        fighting_AudioSource.Play();
    }
}
