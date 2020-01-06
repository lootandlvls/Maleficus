using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerFrameContext : MaleficusMonoBehaviour
{



    private Dictionary<ESpellSlot, UI_SpellCooldowns> spellCooldownsIcons = new Dictionary<ESpellSlot, UI_SpellCooldowns>();


    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();
        EventManager.Instance.SPELLS_SpellSpawned += On_SpellSpawned;
    }

    private void On_SpellSpawned(ISpell A, EPlayerID arg2)
    {
        throw new NotImplementedException();
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
