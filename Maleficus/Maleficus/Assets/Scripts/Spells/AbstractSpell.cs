using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpell : MonoBehaviour, ISpell
{
    private Vector3 movingDirection;
    public Rigidbody myRigidBody;
    public Vector3 dirVector;
    public EPlayerID CastingPlayerID { get; set; }

    public int HitPower { get { return hitPower; } }

    public float Speed { get { return speed; } }

    public Vector3 Direction { get { return dirVector; } }

    public Vector3 EndDestination { get; set; }

    public string SpellName { get { return spellName; } }

    public int SpellLevel { get { return spellLevel; } }

    public bool HasEffect { get { return hasEffect; } }

      public MovementType MovementType { get {  return movementType; } }

    List<Debuff> ISpell.DebuffEffects { get { return debuffEffects; } }

    List<Buff> ISpell.BuffEffects { get { return buffEffects; } }

    [SerializeField] private int hitPower;
    [SerializeField] private float speed;
    [SerializeField] private string spellName;
    [SerializeField] private int spellLevel;
    [SerializeField] private bool hasEffect;
  
    [SerializeField] private MovementType movementType;
    [SerializeField] private  List<Debuff> debuffEffects;
    [SerializeField] private  List<Buff> buffEffects;

    public bool shoot = true;
// Start is called before the first frame update
    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (shoot)
        {
            Move();
        }
    }

    //this function will be over written by the spells children classes
    public void SpellAbility( )
    {
        
      

    }
    public void Move()
    {
        /*  movingDirection.z = speed * Time.deltaTime;

          dirVector = transform.TransformDirection(movingDirection);
          myRigidBody.velocity = new Vector3(dirVector.x, dirVector.y, dirVector.z);*/
        movingDirection = new Vector3(myRigidBody.position.x - 1, 15, myRigidBody.position.z );
       Vector3 facedDirection= transform.TransformDirection(Vector3.forward);
        myRigidBody.AddForce(movingDirection * 30);
        shoot = false;


    }






    private void OnTriggerEnter(Collider other)
    {
        IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
        if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID)) 
        {
           
            HitInfo hitInfo = new HitInfo(this, CastingPlayerID, otherPlayer.PlayerID, transform.position,hasEffect, debuffEffects , buffEffects);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
            ProjectileMoveScript destroyEffect = this.GetComponent<ProjectileMoveScript>();
            destroyEffect.DestroySpell();
        }
    }
}
