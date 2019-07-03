using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : AbstractSpell
{

   private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
       
        onExplosionEnter( transform.position, 3);
    }

    // Update is called once per frame
    void Update()
    {

    }
  //  && (this.GetComponent<AbstractSpell>().CastingPlayerID != hitColliders[i].GetComponent<IPlayer>().PlayerID)
    void onExplosionEnter(Vector3 center , float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        List<IPlayer> hitPlayers = new List<IPlayer>();
        while (i < hitColliders.Length)
        {
            if ((hitColliders[i] != null) && hitColliders[i].tag == "Player1" )
            {
                IPlayer otherPlayer = hitColliders[i].gameObject.GetComponent<IPlayer>();
                AbstractSpell abstractSpell = this.GetComponent<AbstractSpell>();

                if (abstractSpell.CastingPlayerID != otherPlayer.PlayerID)
                {

                    hitPlayers.Add(otherPlayer);

                }
             }
           
            i++;
        }
        ProcessHits(hitPlayers.ToArray());

    }
}
