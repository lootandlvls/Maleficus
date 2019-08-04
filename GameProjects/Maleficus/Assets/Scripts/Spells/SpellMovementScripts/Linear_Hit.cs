using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear_Hit : AbstractSpell
{
    private Vector3 movingDirection;  
    public Vector3 directionVector;
    
    
    [SerializeField] bool shoot;

    // Start is called before the first frame update
    void Start()
    {
        AbstractSpell abstractSpell = this.GetComponent<AbstractSpell>();
      
        myRigidBody = this.GetComponent<Rigidbody>();
    } 

    
    // Update is called once per frame
    void FixedUpdate()
    {
        
        Move();
       
        
    }


    public void Move()
    {
      
        movingDirection =  Vector3.forward * Speed * Time.deltaTime;

        directionVector = transform.TransformDirection(movingDirection);
        myRigidBody.velocity = new Vector3(directionVector.x, directionVector.y, directionVector.z);

    }

    private void OnTriggerEnter(Collider other)
    {

        Vector3 pushingDirection = Vector3.forward * HitPower;
        dirVector = transform.TransformDirection(pushingDirection);
        IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
        IEnemy otherEnemy = other.gameObject.GetComponent<IEnemy>();

        if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID))
        {
            ProcessHits(otherPlayer);
        }
        else if (otherEnemy != null)
        {
            ProcessHits(otherEnemy);
        }
        else if (other.tag.Equals("Object"))
        {
            ProjectileMoveScript destroyEffect = this.GetComponent<ProjectileMoveScript>();
            if (destroyEffect != null)
            {
                destroyEffect.DestroySpell();
            }
        }
        /*else
        {
            ProjectileMoveScript destroyEffect = this.GetComponent<ProjectileMoveScript>();
            if (destroyEffect != null)
            {
                destroyEffect.DestroySpell();
            }
            
        }*/
    }
}
