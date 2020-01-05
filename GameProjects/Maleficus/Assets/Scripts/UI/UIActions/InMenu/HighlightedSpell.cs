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
    }

    private void OnSpellHighlighted(EPlayerID playerID , AbstractSpell highlightedSpell)
    {
        if (player == playerID)
        {
            spellIcon.sprite = highlightedSpell.SpellIcon;
        }
    }
}
