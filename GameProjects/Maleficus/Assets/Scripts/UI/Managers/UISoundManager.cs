using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : AbstractSingletonManager<UISoundManager>
{
    [SerializeField] AudioClip buttonHighlighted_Clip;
    //[SerializeField] AudioClip buttonSelected_Clip;
    [SerializeField] AudioClip buttonPressed_Clip;
    [SerializeField] AudioClip countdown_Clip;
    [SerializeField] AudioClip countdownFinished_Clip;
    [SerializeField] AudioClip playerJoined_Clip;
    [SerializeField] AudioClip playerLeft_Clip;
    [SerializeField] AudioClip playerReady_Clip;


    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.PLAYERS_PlayerJoined += On_PLAYERS_PlayerJoined;
        EventManager.Instance.PLAYERS_PlayerLeft += On_PLAYERS_PlayerLeft;
        EventManager.Instance.PLAYERS_PlayerReady += On_PLAYERS_PlayerReady;
    }



    private void On_PLAYERS_PlayerJoined(EPlayerID playerID)
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_SPELL_SELECTION)
        {
            SoundManager.Instance.SpawnSoundObject(playerJoined_Clip);
        }
    }

    private void On_PLAYERS_PlayerLeft(EPlayerID playerID)
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_SPELL_SELECTION)
        {
            SoundManager.Instance.SpawnSoundObject(playerLeft_Clip);
        }
    }

    private void On_PLAYERS_PlayerReady(EPlayerID playerID)
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_MENU_IN_SPELL_SELECTION)
        {
            SoundManager.Instance.SpawnSoundObject(playerReady_Clip);
        }
    }

    #region SPAWN_SOUND
    public void SpawnSound_ButtonHighlighted()
    {
        SoundManager.Instance.SpawnSoundObject(buttonHighlighted_Clip);
    }

    //public void SpawnSound_ButtonSelected()
    //{
    //    SoundManager.Instance.SpawnSoundObject(buttonSelected_Clip);
    //}

    public void SpawnSound_ButtonPressed()
    {
        SoundManager.Instance.SpawnSoundObject(buttonPressed_Clip);
    }

    public void SpawnSound_Countdown()
    {
        SoundManager.Instance.SpawnSoundObject(countdown_Clip);
    }
        
    public void SpawnSound_CountdownFinished()
    {
        SoundManager.Instance.SpawnSoundObject(countdownFinished_Clip);
    }

    #endregion

}
