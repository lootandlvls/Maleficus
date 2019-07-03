using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear_Hit : AbstractSpell
{
    private Vector3 movingDirection;  
    public Vector3 directionVector;
    [SerializeField] float Spellspeed;
    [SerializeField] private float spellDuration;
    [SerializeField] bool shoot;

    // Start is called before the first frame update
    void Start()
    {
        AbstractSpell abstractSpell = this.GetComponent<AbstractSpell>();
        Spellspeed = abstractSpell.Speed;
        myRigidBody = this.GetComponent<Rigidbody>();
    } 

    // Update is called once per frame
    void Update()
    {
        
            Move();
       
        
    }


    public void Move()
    {
          movingDirection.z = Spellspeed * Time.deltaTime;

          directionVector = transform.TransformDirection(movingDirection);
          myRigidBody.velocity = new Vector3(dirVector.x, dirVector.y, dirVector.z);
       
         shoot = false;


    }
    private void OnTriggerEnter(Collider other)
    {
        Vector3 movingDirection = Vector3.forward * HitPower;
        dirVector = transform.TransformDirection(movingDirection);
        IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();

       if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID))
        {
            ProcessHits(otherPlayer);
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
