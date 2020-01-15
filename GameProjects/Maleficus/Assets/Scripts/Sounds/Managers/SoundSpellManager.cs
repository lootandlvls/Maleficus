using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSpellManager : AbstractSingletonManager<SoundSpellManager>
{

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.SPELLS_SpellSpawned += On_SPELLS_SpellSpawned;
        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;
    }

    private void On_SPELLS_SpellSpawned(ISpell spell, EPlayerID playerID, ESpellSlot spellSlot)
    {
        AudioClip castSound = spell.CastSound;
        if (castSound)
        {
            SoundManager.Instance.SpawnSoundObject(castSound);
        }
    }

    private void On_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {
        ISpell spell = hitInfo.CastedSpell;
        AudioClip hitSound = spell.HitSound;
        if (hitSound)
        {
            SoundManager.Instance.SpawnSoundObject(hitSound);
        }
    }


}
