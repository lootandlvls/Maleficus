using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Linear_Hit : AbstractSpell
{
    private Vector3 movingDirection;
    public Vector3 directionVector;
    private Vector3 forward = Vector3.forward;

    [SerializeField] private bool shoot;

    private SphereCollider mySphereCollider;
    private bool hasBeenTriggered = false;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myRigidBody = GetComponent<Rigidbody>();
        mySphereCollider = GetComponentWithCheck<SphereCollider>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Move();
    }


    private void Move()
    {

        movingDirection = forward * Speed * Time.deltaTime;

        directionVector = transform.TransformDirection(movingDirection);
        myRigidBody.velocity = new Vector3(directionVector.x, directionVector.y, directionVector.z);

    }

   
    private void OnTriggerEnter(Collider other)
    {


        if (hasBeenTriggered == false)
        {
            Vector3 pushingDirection = Vector3.forward;
            direction = transform.TransformDirection(pushingDirection);
            IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
            IEnemy otherEnemy = other.gameObject.GetComponent<IEnemy>();
            Shield shield = other.gameObject.GetComponent<Shield>();


            if (shield == null)
            {
                if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID) && other.tag == Maleficus.Consts.TAG_PLAYER)
                {
                    hasBeenTriggered = true;
                    Explode();
                    //ProcessHits(otherPlayer, ESpellStatus.ENTER);
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