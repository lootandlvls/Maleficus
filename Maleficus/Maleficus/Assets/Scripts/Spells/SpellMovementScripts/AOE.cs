using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : AbstractSpell
{

   private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
       
        onExplosionEnter( transform.position, 3.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
  //  && (this.GetComponent<AbstractSpell>().CastingPlayerID != hitColliders[i].GetComponent<IPlayer>().PlayerID)
    void onExplosionEnter(Vector3 center , float radius)
    {
        AbstractSpell abstractSpell = GetComponent<AbstractSpell>();
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        List<IPlayer> hitPlayers = new List<IPlayer>();
        List<IEnemy> hitEnemies = new List<IEnemy>();
        while (i < hitColliders.Length)
        {
            if (hitColliders[i] != null)
            {
                IPlayer otherPlayer = hitColliders[i].gameObject.GetComponent<IPlayer>();
                IEnemy otherEnemy = hitColliders[i].gameObject.GetComponent<IEnemy>();
                if (otherPlayer != null)
                {
                    if (abstractSpell.CastingPlayerID != otherPlayer.PlayerID)
                    {
                        hitPlayers.Add(otherPlayer);
                    }
                }
                else if (otherEnemy != null)
                {
                    hitEnemies.Add(otherEnemy);
                }
            }
           
            i++;
        }
        ExplostionProcessHits(hitPlayers.ToArray());

    }
}
