using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpell : MonoBehaviour, ISpell
{
    private Vector3 movingDirection;
    public Rigidbody myRigidBody;
    public Vector3 dirVector;
    public EPlayerID CastingPlayerID { get; set; }
    public Transform parabolicSpell_EndPosition;

  


    public int HitPower { get { return hitPower; } }

    public float Speed { get { return speed; } }

    public Vector3 Direction { get { return dirVector; } }

    public Vector3 EndDestination { get; set; }

    public string SpellName { get { return spellName; } }

    public int SpellLevel { get { return spellLevel; } }

    public bool HasPower { get { return hasPower; } }

    public MovementType MovementType { get {  return movementType; } }

    public List<SpellEffects> DebuffEffects { get { return debuffEffects; } }

    public List<SpellEffects> BuffEffects { get { return buffEffects; } }

    [SerializeField] private int hitPower;
    [SerializeField] private float speed;
    [SerializeField] private string spellName;
    [SerializeField] private int spellLevel;
    [SerializeField] private bool OnSelfEffect;
    [SerializeField] private bool hasPower;
  
    [SerializeField] private MovementType movementType;
    [SerializeField] private  List<SpellEffects> debuffEffects;
    [SerializeField] private  List<SpellEffects> buffEffects;
    
   

    
// Start is called before the first frame update
    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        if (OnSelfEffect)
        {
            HitInfo hitInfo = new HitInfo(this, CastingPlayerID,  CastingPlayerID, transform.position, hasPower, debuffEffects, buffEffects);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
        }
        
       
    }

    // Update is called once per frame
    private void Update()
    {
      
    }
 
    private void OnTriggerEnter(Collider other)
    {
        IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
        if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID)) 
        {         

            HitInfo hitInfo = new HitInfo(this, CastingPlayerID, otherPlayer.PlayerID, transform.position,hasPower, debuffEffects , buffEffects);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
            ProjectileMoveScript destroyEffect = this.GetComponent<ProjectileMoveScript>();
            destroyEffect.DestroySpell();
        }

       
    }
}
