using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpell : MonoBehaviour, ISpell
{
    //  private Vector3 movingDirection;




    public int HitPower { get { return hitPower; } }

    public float Speed { get { return speed; } }

    public Vector3 Direction { get { return direction; } }

    public Vector3 EndDestination { get; set; }

    public string SpellName { get { return spellName; } }

    public int SpellLevel { get { return spellLevel; } }

    public bool HasPower { get { return hasPower; } }

    public EMovementType MovementType { get { return movementType; } }

    public List<ESpellEffects> DebuffEffects { get { return debuffEffects; } }

    public List<ESpellEffects> BuffEffects { get { return buffEffects; } }

    public EPlayerID CastingPlayerID { get; set; }

    public float Cooldown { get { return cooldown; } }

    public float CastingDuration { get { return spellDuration; } }

    public float PushDuration { get { return pushDuration; } }

    public ESpells Spell { get { return spell; } }


    [SerializeField] public int hitPower;
    [SerializeField] public float speed;
    [SerializeField] private string spellName;
    [SerializeField] private int spellLevel;
    [SerializeField] private bool OnSelfEffect;
    [SerializeField] private bool hasPower;
    [SerializeField] private ESpells spell;

    [SerializeField] private EMovementType movementType;
    [SerializeField] private List<ESpellEffects> debuffEffects;
    [SerializeField] private List<ESpellEffects> buffEffects;

    [SerializeField] private float cooldown;
    [SerializeField] private float spellDuration;
    [SerializeField] private float pushDuration;

    protected Rigidbody myRigidBody;

    protected Vector3 direction;



    public Vector3 parabolicSpell_EndPosition;



    // Start is called before the first frame update
    private void Start()
    {

        direction = new Vector3(0, 0, 0);
        myRigidBody = GetComponent<Rigidbody>();

        if (OnSelfEffect)
        {
            SHitInfo hitInfo = new SHitInfo(this, CastingPlayerID, CastingPlayerID, transform.position, hasPower, debuffEffects, buffEffects);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
        }
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
        if (hitPlayer.IsDead == false)
        {
            SHitInfo hitInfo = new SHitInfo(this, CastingPlayerID, hitPlayer.PlayerID, hitPlayer.Position, hasPower, debuffEffects, buffEffects);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);

            ProjectileMoveScript destroyEffect = this.GetComponent<ProjectileMoveScript>();

            if (destroyEffect != null)
            {
                destroyEffect.DestroySpell();
            }
        }
    }

    protected void ProcessHits(IEnemy hitEnemy)
    {
        if (hitEnemy.IsDead == false)
        {
            EventManager.Instance.Invoke_SPELLS_SpellHitEnemy(hitEnemy);

            ProjectileMoveScript destroyEffect = this.GetComponent<ProjectileMoveScript>();
            if (destroyEffect != null)
            {
                destroyEffect.DestroySpell();
            }
        }
    }




    // Vector3 dir = (other.transform.position - transform.position) * power;
    protected void ExplosionProcessHits(IPlayer[] hitPlayers)
    {
        foreach (IPlayer hitPlayer in hitPlayers)
        {
            Vector3 movingDirection = (hitPlayer.Position - transform.position).normalized;
            direction = new Vector3(movingDirection.x, 0.0f, movingDirection.z);
            ProcessHits(hitPlayer);
        }



    }
    protected void ExplosionProcessHits(IEnemy[] hitEnemies)
    {
        foreach (IEnemy hitEnemy in hitEnemies)
        {

            ProcessHits(hitEnemy);
        }



    }




}
