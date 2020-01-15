using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightedSpell : BNJMOBehaviour
{
    [SerializeField] private Image spellIcon;
    [SerializeField] private Text spellNameText;

    private EPlayerID playerID;

    protected override void Awake()
    {
        base.Awake();

        IS_NOT_NULL(spellIcon);
        IS_NOT_NULL(spellNameText);

        PlayerSpellSelectionContext context = GetComponentInParent<PlayerSpellSelectionContext>();
        if (IS_NOT_NULL(context))
        {
            playerID = context.PlayerID;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (SpellSelectionManager.IsInstanceSet)
        {
            SpellSelectionButton spellButton = SpellSelectionManager.Instance.GetHighlightedSpellButton(playerID);
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

    private void On_PLAYERS_PlayerJoined(EPlayerID playerID)
    {
        if (playerID == this.playerID)
        {
            SpellSelectionButton spellButton = SpellSelectionManager.Instance.GetHighlightedSpellButton(this.playerID);
            if (spellButton != null)
            {
                spellIcon.sprite = spellButton.Spell.SpellIcon;
            }
        }
    }

    private void OnSpellHighlighted(EPlayerID playerID , AbstractSpell highlightedSpell)
    {
        if (this.playerID == playerID)
        {
            spellIcon.sprite = highlightedSpell.SpellIcon;
            spellNameText.text = highlightedSpell.SpellName;
        }
    }
}
