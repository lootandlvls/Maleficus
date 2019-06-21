using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : MonoBehaviour
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
        while (i < hitColliders.Length)
        {
            if ((hitColliders[i] != null) && hitColliders[i].tag == "Player1" )
            {
                IPlayer otherPlayer = hitColliders[i].gameObject.GetComponent<IPlayer>();
                AbstractSpell abstractSpell = this.GetComponent<AbstractSpell>();

                if (abstractSpell.CastingPlayerID != otherPlayer.PlayerID)
                { 
              
                HitInfo hitInfo = new HitInfo(abstractSpell, abstractSpell.CastingPlayerID, otherPlayer.PlayerID, transform.position, abstractSpell.HasPower, abstractSpell.DebuffEffects, abstractSpell.BuffEffects);
                EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);

                }
             }
           
            i++;
        }

    }
}
