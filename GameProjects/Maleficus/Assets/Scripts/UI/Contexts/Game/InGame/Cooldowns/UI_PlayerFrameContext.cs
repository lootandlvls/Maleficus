using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerFrameContext : BNJMOBehaviour
{
    [SerializeField] private EPlayerID PlayerID = EPlayerID.NONE;
    [SerializeField] private GameObject playerFrameView;
    [SerializeField] private GameObject playerHead_AliveImage;
    [SerializeField] private GameObject playerHead_DeadImage;
    [SerializeField] private GameObject gameOverImage;

    private Dictionary<ESpellSlot, UI_SpellCooldowns> spellCooldownsIcons = new Dictionary<ESpellSlot, UI_SpellCooldowns>();
    private Dictionary<int, UI_PlayerLives> PlayerLivesIcons = new Dictionary<int, UI_PlayerLives>();
    private bool isPlayerActive = false;
    private int remainingLives;

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();
        EventManager.Instance.SPELLS_SpellSpawned += On_SpellSpawned;
        EventManager.Instance.GAME_PlayerStatsUpdated += On_GAME_PlayerStatsUpdated;
        EventManager.Instance.PLAYERS_PlayerJoined += On_PLAYERS_PlayerJoined;    
        EventManager.Instance.PLAYERS_PlayerDied += On_PLAYERS_PlayerDied;
        EventManager.Instance.PLAYERS_PlayerSpawned += On_PLAYERS_PlayerSpawned;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.SPELLS_SpellSpawned       -= On_SpellSpawned;
            EventManager.Instance.GAME_PlayerStatsUpdated   -= On_GAME_PlayerStatsUpdated;
            EventManager.Instance.PLAYERS_PlayerJoined      -= On_PLAYERS_PlayerJoined;
            EventManager.Instance.PLAYERS_PlayerDied        -= On_PLAYERS_PlayerDied;
            EventManager.Instance.PLAYERS_PlayerSpawned     -= On_PLAYERS_PlayerSpawned;
        }
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        foreach (UI_SpellCooldowns SpellCooldown in GetComponentsInChildren<UI_SpellCooldowns>())
        {
            if (IS_KEY_NOT_CONTAINED(spellCooldownsIcons, SpellCooldown.SpellSlot))
            {
                spellCooldownsIcons.Add(SpellCooldown.SpellSlot, SpellCooldown);
            }
        }

        foreach (UI_PlayerLives PlayerLife in GetComponentsInChildren<UI_PlayerLives>())
        {
            if (IS_KEY_NOT_CONTAINED(PlayerLivesIcons, PlayerLife.LiveNumber))
            {
                PlayerLivesIcons.Add(PlayerLife.LiveNumber, PlayerLife);
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        switch (GameManager.Instance.ChosenGameModeType)
        {
            case EGameMode.FFA_LIVES:
                GM_FFA_Lives gM_FFA_Lives = (GM_FFA_Lives)GameManager.Instance.CurrentGameMode;
                if (IS_NOT_NULL(gM_FFA_Lives))
                {
                    UpdateLives(gM_FFA_Lives.TotalLives);
                }
                break;
        }

        // Check if player has joined
        if (PlayerManager.Instance.HasPlayerJoined(PlayerID))
        {
            isPlayerActive = true;
            InitializeSpellsIcons();
        }
        else
        {
            playerFrameView.SetActive(false);
        }

    }

    private void On_PLAYERS_PlayerSpawned(EPlayerID playerID)
    {
        if (playerID == PlayerID)
        {
            SetAliveHead();
        }
    }

    private void On_PLAYERS_PlayerDied(EPlayerID playerID)
    {
        if (playerID == PlayerID)
        {
                SetDeadHead();
        }
    }

    private void On_SpellSpawned(ISpell spell, EPlayerID playerID, ESpellSlot spellSlot)
    {
        if (PlayerID == playerID)
        {
            if (IS_KEY_CONTAINED(spellCooldownsIcons, spellSlot))
            {
                spellCooldownsIcons[spellSlot].StartCooldown(spell.Cooldown + spell.CastDuration);
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
                    remainingLives = playerStatsFFA.RemainingLives;
                    UpdateLives(playerStatsFFA.RemainingLives);
                }
                break;
        }
    }

    private void On_PLAYERS_PlayerJoined(EPlayerID playerID, EControllerID controllerID)
    {
        if (playerID == PlayerID)
        {
            playerFrameView.SetActive(true);
            isPlayerActive = true;
            InitializeSpellsIcons();
        }
    }

    private void SetDeadHead()
    {
        playerHead_AliveImage.SetActive(false);
        playerHead_DeadImage.SetActive(true);
    }

    private void SetAliveHead()
    {
        if (IS_NOT_NULL(playerHead_AliveImage))
        {
            playerHead_AliveImage.SetActive(true);
        }
        if (IS_NOT_NULL(playerHead_DeadImage))
        {
            playerHead_DeadImage.SetActive(false);
        }
    }

    private void SetGameOver()
    {
        if (IS_NOT_NULL(gameOverImage))
        {
            gameOverImage.SetActive(true);
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
                SetDeadHead();
                SetGameOver();
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
        foreach (ESpellSlot spellSlot in Enum.GetValues(typeof(ESpellSlot)))
        {
            if ((spellSlot != ESpellSlot.NONE)
                && (IS_KEY_CONTAINED(spellCooldownsIcons, spellSlot))
                && (IS_NOT_NULL(spellCooldownsIcons[spellSlot]))
                && (IS_NOT_NULL(spellCooldownsIcons[spellSlot].SpellIcon))
                && (IS_KEY_CONTAINED(SpellManager.Instance.playersChosenSpells, PlayerID))
                && (IS_NOT_NULL(SpellManager.Instance.playersChosenSpells[PlayerID]))
                && (IS_KEY_CONTAINED(SpellManager.Instance.playersChosenSpells[PlayerID], spellSlot))
                && (IS_NOT_NULL(SpellManager.Instance.playersChosenSpells[PlayerID][spellSlot])))
            {
                spellCooldownsIcons[spellSlot].SpellIcon.sprite = SpellManager.Instance.playersChosenSpells[PlayerID][spellSlot].SpellIcon;
            }
        }
    }
}
