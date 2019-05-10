using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpell
{
    PlayerID PlayerID { get; }
    int HitPower { get; }
    Vector3 Direction { get; }
    Vector3 EndDestination { get; }

}
