using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionSpellCollision : BNJMOBehaviour, IPlayer
{
    public EPlayerID PlayerID { get; set; }
    public ETeamID TeamID { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public bool IsDead { get; set; }
}
