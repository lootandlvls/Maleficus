using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Explosion : AbstractSpell
{
    [SerializeField] float explosionRadius;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        OnExplosionEnter(transform.position, explosionRadius);
    }

    private void OnExplosionEnter(Vector3 center, float radius)
    {
        AbstractSpell abstractSpell = GetComponent<AbstractSpell>();
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        List<IPlayer> hitPlayers = new List<IPlayer>();
        List<IEnemy> hitEnemies = new List<IEnemy>();

        foreach (Collider collider in hitColliders)
        {

            if (collider != null)
            {
                Debug.Log("Collider : " + collider.name);

                IPlayer otherPlayer = collider.gameObject.GetComponent<IPlayer>();

                if (collider.tag.Equals("Enemy"))
                {
                    IEnemy otherEnemy = collider.gameObject.GetComponent<IEnemy>();
                    hitEnemies.Add(otherEnemy);

                }
                if (otherPlayer != null && collider.tag == "Player")
                {

                    if (abstractSpell.CastingPlayerID != otherPlayer.PlayerID)
                    {
                        Debug.Log("PLAYER WITH ID " + Maleficus.Utils.GetIntFrom(otherPlayer.PlayerID) + "HAS BEEN HIT ");
                        hitPlayers.Add(otherPlayer);
                    }
                }


            }
        }
        ExplosionProcessHits(hitPlayers.ToArray());

        //ExplosionProcessHits(hitEnemies.ToArray());

    }


}
