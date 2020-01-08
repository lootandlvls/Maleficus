using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerFrameContext : MaleficusMonoBehaviour
{
    [SerializeField] EPlayerID PlayerID;
    private Dictionary<ESpellSlot, UI_SpellCooldowns> spellCooldownsIcons = new Dictionary<ESpellSlot, UI_SpellCooldowns>();
    private Dictionary<ESpellSlot, UI_PlayerLives> spellLivesIcons = new Dictionary<ESpellSlot, UI_PlayerLives>();

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();
        EventManager.Instance.SPELLS_SpellSpawned += On_SpellSpawned;
    }

    private void On_SpellSpawned(ISpell spell, EPlayerID playerID , ESpellSlot spellSlot)
    {
        if (PlayerID == playerID)
        {
            if (IS_KEY_CONTAINED(spellCooldownsIcons, spellSlot))
            {
                spellCooldownsIcons[spellSlot].StartCooldown(spell.Cooldown + spell.CastingDuration);
            }
        }
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        foreach (UI_SpellCooldowns SpellCooldown in GetComponentsInChildren<UI_SpellCooldowns>())
        {
            spellCooldownsIcons.Add(SpellCooldown.SpellSlot, SpellCooldown);
        }
    }


}
