using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear_Hit : AbstractSpell
{
    private Vector3 movingDirection;  
    public Vector3 directionVector;
    private Vector3 forward = Vector3.forward;
    
    [SerializeField] bool shoot;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        AbstractSpell abstractSpell = this.GetComponent<AbstractSpell>();
      
        myRigidBody = this.GetComponent<Rigidbody>();
    } 

    
    // Update is called once per frame
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

    private void OnTriggerEnter(Collider other)
    {

        Vector3 pushingDirection = Vector3.forward;
        direction = transform.TransformDirection(pushingDirection);
        IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
        IEnemy otherEnemy = other.gameObject.GetComponent<IEnemy>();
        Shield shield = other.gameObject.GetComponent<Shield>();

        if (shield == null)
        {
            if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID) && other.tag == "Player")
            {
                Debug.Log("Casting player : " + Maleficus.MaleficusUtilities.GetIntFrom(CastingPlayerID) + " Player HIT : " + Maleficus.MaleficusUtilities.GetIntFrom(otherPlayer.PlayerID));
                ProcessHits(otherPlayer);
            }
            else if (otherEnemy != null)
            {
                ProcessHits(otherEnemy);
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
