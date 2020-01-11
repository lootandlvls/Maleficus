using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerFrameContext : MaleficusMonoBehaviour
{
    [SerializeField] EPlayerID PlayerID;
    [SerializeField] int RemainingLives;
    [SerializeField] Image PlayerDeadImage;
    [SerializeField] Image PlayerDeadMaleficusHead;
    [SerializeField] Image PlayerAliveMaleficusHead;
    private Dictionary<ESpellSlot, UI_SpellCooldowns> spellCooldownsIcons = new Dictionary<ESpellSlot, UI_SpellCooldowns>();
    private Dictionary<int, UI_PlayerLives> PlayerLivesIcons = new Dictionary<int, UI_PlayerLives>();
    private bool isPlayerActive = false;

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();
        EventManager.Instance.SPELLS_SpellSpawned += On_SpellSpawned;
        EventManager.Instance.GAME_PlayerStatsUpdated += On_GAME_PlayerStatsUpdated;
        EventManager.Instance.PLAYERS_PlayerDied += On_PLAYERS_PlayerDied;
        EventManager.Instance.PLAYERS_PlayerSpawned += On_PLAYERS_PlayerSpawned;

    }

    private void On_PLAYERS_PlayerSpawned(EPlayerID playerID)
    {
        if (playerID == PlayerID)
        {
            PlayerDeadMaleficusHead.gameObject.SetActive(false);
        }
    }

    private void On_PLAYERS_PlayerDied(EPlayerID playerID)
    {
        if (playerID == PlayerID)
        {
            PlayerDeadMaleficusHead.gameObject.SetActive(true);
        }
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        foreach (UI_SpellCooldowns SpellCooldown in GetComponentsInChildren<UI_SpellCooldowns>())
        {
            spellCooldownsIcons.Add(SpellCooldown.SpellSlot, SpellCooldown);
        }

        foreach (UI_PlayerLives PlayerLife in GetComponentsInChildren<UI_PlayerLives>())
        {
            PlayerLivesIcons.Add(PlayerLife.LiveNumber, PlayerLife);
        }


    }

    protected override void Start()
    {
        base.Start();

        switch (GameManager.Instance.ChosenGameModeType)
        {
            case EGameMode.FFA_LIVES:
                GM_FFA_Lives gM_FFA_Lives = (GM_FFA_Lives)GameManager.Instance.CurrentGameMode;
                UpdateLives(gM_FFA_Lives.TotalLives);
                break;
        }
        CheckIsPlayerActive();
        if (isPlayerActive)
        {
            InitializeSpellsIcons();
        }


       

    }

    private void CheckIsPlayerActive()
    {
        EPlayerID[] activePlayerList = PlayerManager.Instance.GetConnectedPlayers();
        foreach (EPlayerID player in activePlayerList)
        {
            if (player == PlayerID)
            {
                isPlayerActive = true;
            }

        }
        if (!isPlayerActive)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void On_SpellSpawned(ISpell spell, EPlayerID playerID, ESpellSlot spellSlot)
    {
        if (PlayerID == playerID)
        {
            if (IS_KEY_CONTAINED(spellCooldownsIcons, spellSlot))
            {
                spellCooldownsIcons[spellSlot].StartCooldown(spell.Cooldown + spell.CastingDuration);
            }
        }
    }

    private void On_GAME_PlayerStatsUpdated(AbstractPlayerStats playerStats, EGameMode gameMode)
    {
        switch (gameMode)
        {
            case EGameMode.FFA_LIVES:
                PlayerStats_Lives playerStatsFFA = (PlayerStats_Lives)playerStats;

                if (PlayerID == playerStatsFFA.PlayerID)
                {
                    RemainingLives = playerStatsFFA.RemainingLives;
                    UpdateLives(playerStatsFFA.RemainingLives);
                }
               

                break;
        }
    }


    private void UpdateLives(int remainingLives)
    {
        switch (remainingLives)
        {
            case 0:
                PlayerLivesIcons[1].gameObject.SetActive(false);
                PlayerLivesIcons[2].gameObject.SetActive(false);
                PlayerLivesIcons[3].gameObject.SetActive(false);
                PlayerLivesIcons[4].gameObject.SetActive(false);
                PlayerLivesIcons[5].gameObject.SetActive(false);
                PlayerDeadImage.gameObject.SetActive(true);
                break;
            case 1:
                PlayerLivesIcons[1].gameObject.SetActive(true);
                PlayerLivesIcons[2].gameObject.SetActive(false);
                PlayerLivesIcons[3].gameObject.SetActive(false);
                PlayerLivesIcons[4].gameObject.SetActive(false);
                PlayerLivesIcons[5].gameObject.SetActive(false);
                break;
            case 2:
                PlayerLivesIcons[1].gameObject.SetActive(true);
                PlayerLivesIcons[2].gameObject.SetActive(true);
                PlayerLivesIcons[3].gameObject.SetActive(false);
                PlayerLivesIcons[4].gameObject.SetActive(false);
                PlayerLivesIcons[5].gameObject.SetActive(false);
                break;
            case 3:
                PlayerLivesIcons[1].gameObject.SetActive(true);
                PlayerLivesIcons[2].gameObject.SetActive(true);
                PlayerLivesIcons[3].gameObject.SetActive(true);
                PlayerLivesIcons[4].gameObject.SetActive(false);
                PlayerLivesIcons[5].gameObject.SetActive(false);
                break;
            case 4:
                PlayerLivesIcons[1].gameObject.SetActive(true);
                PlayerLivesIcons[2].gameObject.SetActive(true);
                PlayerLivesIcons[3].gameObject.SetActive(true);
                PlayerLivesIcons[4].gameObject.SetActive(true);
                PlayerLivesIcons[5].gameObject.SetActive(false);
                break;
            case 5:
                PlayerLivesIcons[1].gameObject.SetActive(true);
                PlayerLivesIcons[2].gameObject.SetActive(true);
                PlayerLivesIcons[3].gameObject.SetActive(true);
                PlayerLivesIcons[4].gameObject.SetActive(true);
                PlayerLivesIcons[5].gameObject.SetActive(true);
                break;

        }
        
    }

    private void InitializeSpellsIcons()
    {
        if (SpellManager.Instance.playersChosenSpells[PlayerID] != null) {

            spellCooldownsIcons[ESpellSlot.SPELL_1].SpellIcon.sprite = SpellManager.Instance.playersChosenSpells[PlayerID][ESpellSlot.SPELL_1].SpellIcon;
        }
        if (spellCooldownsIcons[ESpellSlot.SPELL_2].SpellIcon != null)
        {
            spellCooldownsIcons[ESpellSlot.SPELL_2].SpellIcon.sprite = SpellManager.Instance.playersChosenSpells[PlayerID][ESpellSlot.SPELL_2].SpellIcon;
        }
        if (spellCooldownsIcons[ESpellSlot.SPELL_3].SpellIcon != null)
        {
            spellCooldownsIcons[ESpellSlot.SPELL_3].SpellIcon.sprite = SpellManager.Instance.playersChosenSpells[PlayerID][ESpellSlot.SPELL_3].SpellIcon;
        }

            
       

    }
}
