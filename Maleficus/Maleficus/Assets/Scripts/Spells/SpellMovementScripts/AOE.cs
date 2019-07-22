using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : AbstractSpell
{

   private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
       
        onExplosionEnter( transform.position, 3.5f * ARManager.Instance.SizeFactor);
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
                if (otherPlayer != null)
                {
                    if (abstractSpell.CastingPlayerID != otherPlayer.PlayerID)
                    {
                        hitPlayers.Add(otherPlayer);
                    }
                }
              

            }
        }
        ExplosionProcessHits(hitPlayers.ToArray());
        ExplosionProcessHits(hitEnemies.ToArray());

    }
}
