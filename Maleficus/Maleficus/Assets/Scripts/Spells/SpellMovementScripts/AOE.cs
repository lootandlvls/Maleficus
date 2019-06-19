using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        onExplosionEnter(transform.position, 8);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onExplosionEnter(Vector3 center , float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if ((hitColliders[i] != null) && (this.GetComponent<AbstractSpell>().CastingPlayerID != hitColliders[i].GetComponent<IPlayer>().PlayerID))
            {
                IPlayer otherPlayer = hitColliders[i].gameObject.GetComponent<IPlayer>();
                HitInfo hitInfo = new HitInfo(this.GetComponent<AbstractSpell>(), this.GetComponent<AbstractSpell>().CastingPlayerID, otherPlayer.PlayerID, transform.position, this.GetComponent<AbstractSpell>().HasPower, this.GetComponent<AbstractSpell>().DebuffEffects, this.GetComponent<AbstractSpell>().BuffEffects);
                EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
                
            }
            i++;
        }

    }
}
