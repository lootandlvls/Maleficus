using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : AbstractSpell
{
    Transform childTransform;
    [SerializeField] GameObject explosion ;
    [SerializeField] float explosionRadius ;

    private SphereCollider mySphereCollider;

    protected override void Start()
    {
        base.Start();

        childTransform = this.transform.GetChild(0);
       //OnExplosionEnter(transform.position, explosionRadius);
        StartCoroutine(TrapActivated());
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        mySphereCollider = GetComponentWithCheck<SphereCollider>();
    }

    private IEnumerator TrapActivated()
    {
        float size = 1;

        float startTime = Time.time;
        LogConsole("BEFORE WHILE LOOP");
        while (Time.time - startTime < SpellDuration)
        {
            Debug.Log(size);
            size += 0.2f;
            Vector3 vecSize = new Vector3(size / 1000, size / 1000, size / 1000);
            childTransform.localScale += vecSize;
         //   LogConsole("IN THE WHILE LOOP  ||  remaining time : " +  (Time.time - startTime));
           
            yield return new WaitForEndOfFrame();
        }
        LogConsole("AFTER WHILE LOOP");
        //  yield return new WaitForSeconds(0f);
        LogConsole("OnExplosionEnter pre ");
        OnExplosionEnter(transform.position, mySphereCollider.radius);
        LogConsole("OnExplosionEnter after ");
        //GameObject trapExplosion = Instantiate(explosion, transform.position, transform.rotation);
    }


    private void OnExplosionEnter(Vector3 center, float radius)
    {
        LogConsole("OnExplosionEnter");

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
