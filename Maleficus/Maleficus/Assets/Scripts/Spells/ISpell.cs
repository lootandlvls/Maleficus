﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpell
{
    string SpellName { get; }
    int SpellLevel { get; }
    EPlayerID CastingPlayerID { get;  }
    int HitPower { get;  }
    Vector3 Direction { get; }
    Vector3 EndDestination { get;  }
    bool HasPower { get; }
    List<SpellEffects> DebuffEffects { get; }
    List<SpellEffects> BuffEffects { get; }
    MovementType MovementType { get;}
}