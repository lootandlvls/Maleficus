using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : AbstractSpell
{
    Transform childTransform;
    [SerializeField]   GameObject explosion ;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        childTransform = this.transform.GetChild(0);
       
        StartCoroutine(TrapActivated());
    }
   
  private  IEnumerator TrapActivated()
    {
        float size = 1;

        float startTime = Time.time;

        while (Time.time - startTime < Duration)
        {
         //   Debug.Log(size);
            size = 1.0f + (59.0f / (Time.time - startTime));
            Vector3 vecSize = new Vector3(size/1000 , size / 1000, size  / 1000);
            childTransform.localScale += vecSize;
            yield return new WaitForSeconds(0.05f);
        }
        GameObject trapExplosion = Instantiate(explosion, transform.position, transform.rotation);
        OnExplosionEnter(transform.position, 5);  
        DestroySpell();
        
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
                if (otherPlayer != null &&   collider.tag == "Player")
                {
                    if (abstractSpell.CastingPlayerID != otherPlayer.PlayerID)
                    {
                        hitPlayers.Add(otherPlayer);
                    }
                }


            }
        }
        ExplosionProcessHits(hitPlayers.ToArray());
        //ExplosionProcessHits(hitEnemies.ToArray());

    }
}
