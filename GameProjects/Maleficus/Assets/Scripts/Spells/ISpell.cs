using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpell
{
    ESpellID SpellID { get; }

    string SpellName { get; }

    int SpellLevel { get; }

    EPlayerID CastingPlayerID { get;  }

    int HitPower { get;  }

    Vector3 Direction { get; }

    Vector3 EndDestination { get;  }

    bool HasPower { get; }

    List<ESpellEffects> DebuffEffects { get; }

    List<ESpellEffects> BuffEffects { get; }

    ESpellMovementType MovementType { get;}

    float Cooldown { get; }

    float Duration { get; }

    float PushDuration { get; }

    bool IsChargeable { get; }

    bool IsTripleCast { get; }

    Sprite SpellIcon { get; }

    AudioClip CastSound { get; }
    AudioClip HitSound { get; }

    int SkillPoint { get; }
}
