using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HighlightedSpell : BNJMOBehaviour
{
    public EPlayerID PlayerID { get; set; }
    public EHighlightSpellSide HighlightSpellSide { get { return highlightSpellSide; } }

    [SerializeField] private EHighlightSpellSide highlightSpellSide = EHighlightSpellSide.NONE;
    [SerializeField] private Image spellIcon;
    [SerializeField] private Text spellNameText;

    protected override void Awake()
    {
        base.Awake();

        IS_NOT_NONE(highlightSpellSide);
        IS_NOT_NULL(spellIcon);
        IS_NOT_NULL(spellNameText);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (SpellSelectionManager.IsInstanceSet)
        {
            SpellSelectionButton spellButton = SpellSelectionManager.Instance.GetHighlightedSpellButton(PlayerID);
            if (spellButton != null)
            {
                spellIcon.sprite = spellButton.Spell.SpellIcon;
            }
        }
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        SpellSelectionManager.Instance.SpellButtonHighlighted   += OnSpellHighlighted;
        EventManager.Instance.PLAYERS_PlayerJoined              += On_PLAYERS_PlayerJoined;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (SpellSelectionManager.IsInstanceSet)
        {
            SpellSelectionManager.Instance.SpellButtonHighlighted -= OnSpellHighlighted;
        }
        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.PLAYERS_PlayerJoined -= On_PLAYERS_PlayerJoined;
        }
    }

    public void Show()
    {
        foreach (Image image in GetComponentsInChildren<Image>())
        {
            image.enabled = true;
        }
    }

    public void Hide()
    {
        foreach (Image image in GetComponentsInChildren<Image>())
        {
            image.enabled = false;
        }
    }

    private void On_PLAYERS_PlayerJoined(EPlayerID playerID, EControllerID controllerID)
    {
        if (PlayerID == playerID)
        {
            SpellSelectionButton spellButton = SpellSelectionManager.Instance.GetHighlightedSpellButton(PlayerID);
            if (spellButton != null)
            {
                spellIcon.sprite = spellButton.Spell.SpellIcon;
            }
        }
    }

    private void OnSpellHighlighted(EPlayerID playerID , AbstractSpell highlightedSpell)
    {
        if (PlayerID == playerID)
        {
            spellIcon.sprite = highlightedSpell.SpellIcon;
            spellNameText.text = highlightedSpell.SpellName;
        }
    }
}
