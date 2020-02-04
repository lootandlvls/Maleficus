using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : AbstractSpell
{

    protected override void Start()
    {
        base.Start();

        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (IS_NOT_NULL(sphereCollider))
        {
            OnExplosionEnter(transform.position, sphereCollider.radius);
        }
    }


    private void OnExplosionEnter(Vector3 center , float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        List<IPlayer> hitPlayers = new List<IPlayer>();
        List<IEnemy> hitEnemies = new List<IEnemy>();

        foreach (Collider collider in hitColliders)
        {
            if (collider != null)
            {
                IPlayer otherPlayer = collider.gameObject.GetComponent<IPlayer>();
               
                if (collider.tag.Equals("Enemy"))
                {
                    IEnemy otherEnemy = collider.gameObject.GetComponent<IEnemy>();
                    hitEnemies.Add(otherEnemy);
                }
                if (otherPlayer != null  && collider.tag == "Player")
                {
                    if (CastingPlayerID != otherPlayer.PlayerID)
                    {
                        hitPlayers.Add(otherPlayer);
                    }
                }
            }
        }
        ExplosionProcessHits(hitPlayers.ToArray());
    }
}
