using System.Collections;
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
        backDuration = SpellDuration;

      if (SpellID == ESpellID.GET_OVER_HERE)
        {
            StartCoroutine(returnCoroutine());
        }
      
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Move();

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
        dir = Vector3.back;
       
    }
   
    private void OnTriggerEnter(Collider other)
    {
        float collisionTime = Time.time;
        Debug.Log(other.name);

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
                         Explode();
                    }
                    else if (HasGrabPower)
                    {
                        if (SpellID == ESpellID.GET_OVER_HERE)
                        {
                            backDuration = collisionTime - startTime;
                            dir = Vector3.back;
                            SetPushDuration(backDuration);                       
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