using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear_Laser : AbstractSpell
{
    private void OnTriggerStay(Collider other)
    {
        Vector3 movingDirection = Vector3.forward;
        direction = transform.TransformDirection(movingDirection);
        IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
        IEnemy otherEnemy = other.gameObject.GetComponent<IEnemy>();
        Shield shield = other.gameObject.GetComponent<Shield>();
        if (shield == null)
        {
            if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID) && other.tag == Maleficus.Consts.TAG_PLAYER)
            {
                ProcessHits(otherPlayer, ESpellStatus.STAY);
            }
            else if (otherEnemy != null)
            {
                ProcessHits(otherEnemy);
            }
        }
    }
}
