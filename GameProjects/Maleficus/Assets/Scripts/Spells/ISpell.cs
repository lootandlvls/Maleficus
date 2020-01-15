﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpell
{
    ESpellID SpellID { get; }

    string SpellName { get; }

    int SpellLevel { get; }

    EPlayerID CastingPlayerID { get;  }

    float HitPower { get;  }

    Vector3 Direction { get; }

    Vector3 EndDestination { get;  }

    bool HasPushPower { get; }

    List<ESpellEffects> DebuffEffects { get; }

    List<ESpellEffects> BuffEffects { get; }

    ESpellMovementType MovementType { get;}

    float Cooldown { get; }

    float CastDuration { get; }

    float PushDuration { get; }

    float SpellDuration { get; }

    bool IsChargeable { get; }

    bool IsTripleCast { get; }

    Sprite SpellIcon { get; }

    AudioClip CastSound { get; }

    AudioClip HitSound { get; }

    int SkillPoint { get; }
}
