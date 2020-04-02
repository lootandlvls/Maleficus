﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Linear_Hit : AbstractSpell
{
    private Vector3 movingDirection;
    public Vector3 directionVector;

    private Vector3 dir = Vector3.forward;
    private Vector3 startPosition;
    private float startTime;
    private float backDuration ;
    private float localScale;
    [SerializeField] private bool shoot;

    private SphereCollider mySphereCollider;
    private bool hasBeenTriggered = false;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myRigidBody = GetComponent<Rigidbody>();
        mySphereCollider = GetComponentWithCheck<SphereCollider>();
    }

    
    protected override void Start()
    {
        base.Start();
        startPosition = transform.position;
      
        backDuration = SpellDuration / 6    ;
        localScale = transform.localScale.x ;


     //   LogConsole("CASTING PLAYER ID IS " + Maleficus.Utils.GetIntFrom(CfastingPlayerID));
        if (SpellID == ESpellID.GET_OVER_HERE)
        {
            StartCoroutine(returnCoroutine());
        }
      
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Move();
       
            transform.localScale = new Vector3((float)localScale + ChargingPower, (float)localScale + ChargingPower, (float)localScale + ChargingPower);
            mySphereCollider.radius = transform.localScale.x - 0.2f;
      


        if ( dir == Vector3.back && SpellID == ESpellID.GET_OVER_HERE)
        {
            StartCoroutine(waitAndDestroy());
        }
    }


    private void Move()
    {

        movingDirection = dir * Speed * Time.deltaTime;
        directionVector = transform.TransformDirection(movingDirection);
        myRigidBody.velocity = new Vector3(directionVector.x, directionVector.y, directionVector.z);

    }

    IEnumerator waitAndDestroy()
    {
        yield return new WaitForSeconds(backDuration);
        DestroySpell();
    }
    IEnumerator returnCoroutine()
    {
        startTime = Time.time;
        yield return new WaitForSeconds(SpellDuration);

        if (dir == Vector3.forward)
        {
            dir = Vector3.back;
            mySphereCollider.enabled = false;
            speed *= 6 ;

        }

       
    }
   
    private void OnTriggerEnter(Collider other)
    {
        float collisionTime = Time.time;
        Debug.Log("HAS BEEN TRIGGERED");
        //     LogConsole("Collision " + other.gameObject.ToString());

        if (hasBeenTriggered == false)
        {
       
            direction = transform.TransformDirection(dir);
            IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
            IEnemy otherEnemy = other.gameObject.GetComponent<IEnemy>();
            Shield shield = other.gameObject.GetComponent<Shield>();


            if (shield == null)
            {
                if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID) && other.tag == Maleficus.Consts.TAG_PLAYER)
                {
                    
                    hasBeenTriggered = true;
                    if (HasPushPower)
                    {
                       
                      // Explode();
                       ProcessHits(otherPlayer, ESpellStatus.ENTER);
                    }
                    else if (HasGrabPower)
                    {
                        if (SpellID == ESpellID.GET_OVER_HERE)
                        {
                            backDuration = collisionTime - startTime - 0.09f;
                            dir = Vector3.back;
                          
                            SetDirection(transform.TransformDirection(-dir));
                            speed = 850;
                            SetPushDuration(backDuration);
                            SetDebuffDuration(backDuration);
                            ProcessHits(otherPlayer, ESpellStatus.ENTER);
                        }
                        else
                        {
                            ProcessHits(otherPlayer, ESpellStatus.ENTER);
                        }
                        

                    }
                    
                }
               else if (other.tag.Equals("Object"))
                {
                    DestroySpell();
                }
            }
            else
            {
                if (CastingPlayerID != shield.CastingPlayerID)
                {
                    transform.Rotate(180, 0, 0);
                    CastingPlayerID = shield.CastingPlayerID;
                }
            }
        }
    }

    private void Explode()
    {
        List<IPlayer> hitPlayers = new List<IPlayer>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + mySphereCollider.center, mySphereCollider.radius);
        foreach (Collider collider in hitColliders)
        {
            if (collider != null)
            {
               // Debug.Log(collider.name);
                IPlayer otherPlayer = collider.gameObject.GetComponent<IPlayer>();
             
                if (otherPlayer != null && collider.tag == "Player")
                {
                    Debug.Log(otherPlayer.PlayerID);
                    if (CastingPlayerID != otherPlayer.PlayerID)
                    {
                        hitPlayers.Add(otherPlayer);
                    }
                }
            }
        }
        ProcessHits(hitPlayers.ToArray(), ESpellStatus.ENTER);
    }

    private void OnTriggerExit(Collider other)
    {
        Vector3 pushingDirection = Vector3.forward;
        direction = transform.TransformDirection(pushingDirection);
        IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
        IEnemy otherEnemy = other.gameObject.GetComponent<IEnemy>();
        Shield shield = other.gameObject.GetComponent<Shield>();

        if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID) && other.tag == Maleficus.Consts.TAG_PLAYER)
        {
            LogConsole("Trigger exti");
        }

    }
}