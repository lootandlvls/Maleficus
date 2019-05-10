
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitInfo
{
    public HitInfo(ISpell castedSpell, PlayerID castingPlayerID, PlayerID hitplayerID, Vector3 hitPosition)
    {
        this.castedSpell = castedSpell;
        this.castingPlayerID = castingPlayerID;
        this.hitPlayerID = hitplayerID;
        this.hitPosition = hitPosition;
    }

    public ISpell CastedSpell { get { return castedSpell; } }
    public PlayerID CastingPlayerID { get { return castingPlayerID; } }
    public PlayerID HitPlayerID { get { return hitPlayerID; } }
    public Vector3 HitPosition { get { return hitPosition; } }
    public Vector3 HitVelocity { get { return hitPosition + castedSpell.Direction * castedSpell.HitPower; } }


    private ISpell castedSpell;
    private PlayerID castingPlayerID;
    private PlayerID hitPlayerID;
    private Vector3 hitPosition;


}
