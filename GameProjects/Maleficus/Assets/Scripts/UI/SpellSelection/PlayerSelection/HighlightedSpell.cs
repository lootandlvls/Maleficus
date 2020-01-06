using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightedSpell : MaleficusMonoBehaviour
{
    Image spellIcon;
    [SerializeField] EPlayerID player;
    // Start is called before the first frame update
    protected override void InitializeEventsCallbacks()
    {
        
        base.InitializeEventsCallbacks();
        spellIcon = GetComponent<Image>();
        SpellSelectionManager.Instance.SpellButtonHighlighted += OnSpellHighlighted;

        EventManager.Instance.PLAYERS_PlayerJoined += On_PLAYERS_PlayerJoined;
    }

    private void On_PLAYERS_PlayerJoined(EPlayerID playerID)
    {
        if (playerID == player)
        {
            spellIcon.sprite = SpellSelectionManager.Instance.GetHighlightedSpellButton(player).Spell.SpellIcon;
        }
    }

    private void OnSpellHighlighted(EPlayerID playerID , AbstractSpell highlightedSpell)
    {
        if (player == playerID)
        {
            spellIcon.sprite = highlightedSpell.SpellIcon;
        }
    }
}
