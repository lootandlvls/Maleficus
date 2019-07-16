using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpell : MonoBehaviour, ISpell
{
  //  private Vector3 movingDirection;
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

    [SerializeField] public int hitPower;
    [SerializeField] public float speed;
    [SerializeField] private string spellName;
    [SerializeField] private int spellLevel;
    [SerializeField] private bool OnSelfEffect;
    [SerializeField] private bool hasPower;

    public float cooldown;
    public float spellDuration;
  
    [SerializeField] private  MovementType movementType;
    [SerializeField] private  List<SpellEffects> debuffEffects;
    [SerializeField] private  List<SpellEffects> buffEffects;
    
   

    
// Start is called before the first frame update
    private void Start()
    {  

        dirVector = new Vector3(0, 0, 0);
        myRigidBody = GetComponent<Rigidbody>();
        if (OnSelfEffect)
        {
            SHitInfo hitInfo = new SHitInfo(this, CastingPlayerID,  CastingPlayerID, transform.position, hasPower, debuffEffects, buffEffects);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
        }
        
       
    }

    // Update is called once per frame
    private void Update()
    {
      
    }

 


    protected void ProcessHits(IPlayer[] hitPlayers)
    {
        foreach (IPlayer hitPlayer in hitPlayers)
        {
           
            // Debug.Log(dirVector);
            SHitInfo hitInfo = new SHitInfo(this, CastingPlayerID, hitPlayer.PlayerID, hitPlayer.Position, hasPower, debuffEffects, buffEffects);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);

            ProjectileMoveScript destroyEffect = this.GetComponent<ProjectileMoveScript>();

            if (destroyEffect != null)
            {
                destroyEffect.DestroySpell();
            }
        }
    }

    protected void ProcessHits(IPlayer hitPlayer)
    {
        
        // Debug.Log(dirVector);
        SHitInfo hitInfo = new SHitInfo(this, CastingPlayerID, hitPlayer.PlayerID, hitPlayer.Position, hasPower, debuffEffects, buffEffects);
        EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);

        ProjectileMoveScript destroyEffect = this.GetComponent<ProjectileMoveScript>();
        if (destroyEffect != null)
        {
            destroyEffect.DestroySpell();
        }
    }

     protected void ProcessHits(IEnemy hitEnemy)
    {
        
        // Debug.Log(dirVector);
        EventManager.Instance.Invoke_SPELLS_SpellHitEnemy(hitEnemy);

        ProjectileMoveScript destroyEffect = this.GetComponent<ProjectileMoveScript>();
        if (destroyEffect != null)
        {
            destroyEffect.DestroySpell();
        }
    }




    // Vector3 dir = (other.transform.position - transform.position) * power;
    protected void ExplostionProcessHits(IPlayer[] hitPlayers)
    {
        foreach (IPlayer hitPlayer in hitPlayers)
        {   
            Vector3 movingDirection =  (hitPlayer.Position - transform.position).normalized  * HitPower;
            dirVector = movingDirection;
            ProcessHits(hitPlayer);
        }
           
       
       
    }



}
