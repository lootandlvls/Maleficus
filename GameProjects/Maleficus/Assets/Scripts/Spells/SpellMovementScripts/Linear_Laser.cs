using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear_Laser : AbstractSpell
{
    private void OnTriggerStay(Collider other)
    {
        Vector3 movingDirection = Vector3.forward * HitPower;
        dirVector = transform.TransformDirection(movingDirection);
        IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
        IEnemy otherEnemy = other.gameObject.GetComponent<IEnemy>();

        if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID))
        {
            ProcessHits(otherPlayer);
        }
        else if (otherEnemy != null)
        {
            ProcessHits(otherEnemy);

        }
    }
}
