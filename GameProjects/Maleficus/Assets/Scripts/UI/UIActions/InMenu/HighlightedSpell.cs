using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightedSpell : MaleficusMonoBehaviour
{
    Image spellIcon;

    // Start is called before the first frame update
    protected override void InitializeEventsCallbacks()
    {
        
        base.InitializeEventsCallbacks();
        spellIcon = GetComponent<Image>();
        EventManager.Instance.UI_SpellHighlighted += On_UI_SpellHighlighted;
    }

    private void On_UI_SpellHighlighted(EPlayerID playerID , AbstractSpell highlightedSpell)
    {
        spellIcon.sprite = highlightedSpell.SpellIcon;
    }
}
