using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear_Explosive : AbstractSpell { 

    private Vector3 movingDirection;
    public Vector3 directionVector;
    private float maxLocalScale;
    
    private Vector3 forward = Vector3.forward;
    private SphereCollider mySphereCollider;


    protected override void Start()
    {
        base.Start();
        maxLocalScale = 1 + ChargingPower;
       
        StartCoroutine(ScaleUpdateCoroutine());
   
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Move();
    }


    public void Move()
    {

        movingDirection = forward * Speed * Time.deltaTime;

        directionVector = transform.TransformDirection(movingDirection);
        myRigidBody.velocity = new Vector3(directionVector.x, directionVector.y, directionVector.z);

    }

    IEnumerator ScaleUpdateCoroutine()
    {

        float startTime = Time.time;

        while (transform.localScale.z <= maxLocalScale)
        {
            transform.localScale = new Vector3(transform.localScale.x + (Time.time - startTime) / (SpellDuration * 4), transform.localScale.y + (Time.time - startTime) / (SpellDuration * 4), transform.localScale.z + (Time.time - startTime) / (SpellDuration * 4));
        //    mySphereCollider.radius += (Time.time - startTime) / (SpellDuration * 4);
            yield return new WaitForEndOfFrame();
        }



    }
    private void OnTriggerStay(Collider other)
    {
        Vector3 movingDirection = Vector3.forward;
        direction = transform.TransformDirection(movingDirection);
        IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
        IEnemy otherEnemy = other.gameObject.GetComponent<IEnemy>();
        Shield shield = other.gameObject.GetComponent<Shield>();
          ;
        if (shield == null)
        {
            if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID) && other.tag == Maleficus.Consts.TAG_PLAYER)
            {
                if ((other.transform.position - transform.position )!= Vector3.zero )
                {

                    SetDirection(other.transform.position - transform.position);
                                    }
                ProcessHits(otherPlayer, ESpellStatus.STAY);
            }
            else if (otherEnemy != null)
            {
                ProcessHits(otherEnemy);

            }
        }

    }
}
