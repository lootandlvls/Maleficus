using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public abstract class AbstractSpell : BNJMOBehaviour, ISpell
{
    public event Action<AbstractSpell> SpellNotActiveAnymore;

    public float HitPower { get { return hitPower; } }
    public float Speed { get { return speed; } }
    public Vector3 Position { get { return transform.position; } }
    public Vector3 Direction { get { return direction; } }
    public Vector3 EndDestination { get; set; }
    public string SpellName { get { return spellName; } }
    public int SpellLevel { get { return spellLevel; } }
    public bool HasPushPower { get { return hasPushPower; } }
    public bool HasGrabPower { get { return hasGrabPower; } }
    public ESpellMovementType MovementType { get { return movementType; } }
    public List<ESpellEffects> DebuffEffects { get { return debuffEffects; } }
    public float DebuffDuration { get { return debuffDuration; } }
    public float DebuffPower { get { return debuffPower; } }
    public List<ESpellEffects> BuffEffects { get { return buffEffects; } }
    public float BuffPower { get { return buffPower; } }
    public float BuffDuration { get { return buffDuration; } }
    public EPlayerID CastingPlayerID { get; set; }
    public float Cooldown { get { return cooldown; } }
    public float CastDuration { get { return castDuration; } }
    public float SpellDuration { get { return spellDuration; } }
    public float PushDuration { get { return pushDuration; } }
    public ESpellID SpellID { get { return spell; } }
    public bool IsChargeable { get { return isChargeable; } }
    public bool IsTripleCast { get { return isTripleCast; } }
    public Sprite SpellIcon { get { return spellIcon; } }
    public int SkillPoint { get { return skillPoint; } }
    public AudioClip CastSound { get { return castSound; } }
    public AudioClip HitSound { get { return hitSound; } }


    [Separator("Characteristics")]
    [SerializeField] public float hitPower;
    [SerializeField] public float speed;
    [SerializeField] private string spellName;
    [SerializeField] private int spellLevel;
    [SerializeField] private bool OnSelfEffect;
    [SerializeField] private bool hasPushPower;
    [SerializeField] private bool hasGrabPower;
    [SerializeField] private ESpellID spell;
    [SerializeField] private bool isChargeable;
    [SerializeField] private bool isTripleCast;
    [SerializeField] private ESpellMovementType movementType;
    [SerializeField] private int skillPoint;

    [Separator("Durations")]
    [SerializeField] private float cooldown;
    [SerializeField] private float castDuration;
    [SerializeField] private float pushDuration;
    [SerializeField] private float spellDuration;

    [Separator("Debuffs")]
    [SerializeField] private List<ESpellEffects> debuffEffects;
    [SerializeField] private float debuffDuration;
    [SerializeField] private float debuffPower;

    [Separator("Buffs")]
    [SerializeField] private List<ESpellEffects> buffEffects;
    [SerializeField] private float buffDuration;
    [SerializeField] private float buffPower;

    [Separator("Audio")]
    [SerializeField] private AudioClip castSound;
    [SerializeField] private AudioClip hitSound;

    [Separator("UI")]
    [SerializeField] private Sprite spellIcon;



    protected Rigidbody myRigidBody;
    protected Vector3 direction;

    public GameObject hitPrefab;
    public List<GameObject> trails;
    public Vector3 parabolicSpell_EndPosition;


    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        SpellTimer spellTimer = GetComponent<SpellTimer>();
        if (spellTimer)
        {
            spellTimer.SpellTimerDone += DestroySpell;
        }
    }

    protected override void Start()
    {
        base.Start();

        direction = new Vector3(0, 0, 0);
        myRigidBody = GetComponent<Rigidbody>();
        // Activates a Spell unique Abilities
        EventManager.Instance.SPELLS_UniqueEffectActivated += On_SPELLS_UniqueEffectActivated;

        if (OnSelfEffect)
        {
            SHitInfo hitInfo = new SHitInfo(this, CastingPlayerID, CastingPlayerID, ESpellStatus.ENTER, transform.position);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
        }
        if ( SpellID != ESpellID.GET_OVER_HERE)
        {
               StartCoroutine(WaitBeforeDestroySpellCoroutine(spellDuration));
        }

    }

    protected void SetDirection(Vector3 direction)
    {
        direction.y = 0;
        this.direction = direction;
    }

    protected void SetPushDuration(float duration)
    {
        pushDuration = duration;
    }

    protected void SetDebuffDuration(float duration)
    {
        debuffDuration = duration; 
    }

    protected void ProcessHits(IPlayer[] hitPlayers, ESpellStatus spellStatus)
    {
        foreach (IPlayer hitPlayer in hitPlayers)
        {
            SHitInfo hitInfo = new SHitInfo(this, CastingPlayerID, hitPlayer.PlayerID, spellStatus, hitPlayer.Position);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
            if (SpellID != ESpellID.AOE_EXPLOSION && SpellID != ESpellID.FIRE_LASER && SpellID != ESpellID.FIRE_SHOCKBLAST && SpellID != ESpellID.AIR_SLASH )
            {
                DestroySpell();
            }
        }
    }

    protected void ProcessHits(IPlayer hitPlayer, ESpellStatus spellStatus)
    {
        if (hitPlayer.IsDead == false)
        {
            Debug.Log("PROCESSHITS");
            SHitInfo hitInfo = new SHitInfo(this, CastingPlayerID, hitPlayer.PlayerID, spellStatus, hitPlayer.Position);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
            
           if (SpellID != ESpellID.AOE_EXPLOSION && SpellID != ESpellID.FIRE_LASER && SpellID != ESpellID.FIRE_SHOCKBLAST && SpellID != ESpellID.BLACK_HOLE && SpellID != ESpellID.AIR_SLASH && SpellID != ESpellID.GET_OVER_HERE)
           {
                Debug.Log("SPELL DESTROYED");
                DestroySpell();
           }
        }
    }

    protected void ProcessHits(IEnemy hitEnemy)
    {
        if (hitEnemy.IsDead == false)
        {
            EventManager.Instance.Invoke_SPELLS_SpellHitEnemy(hitEnemy);

            DestroySpell();
        
        }
    }

    protected void ExplosionProcessHits(IPlayer[] hitPlayers)
    {
        foreach (IPlayer hitPlayer in hitPlayers)
        {
            Vector3 movingDirection = (hitPlayer.Position - transform.position).normalized;
            direction = new Vector3(movingDirection.x, 0.0f, movingDirection.z);
            ProcessHits(hitPlayer, ESpellStatus.ENTER);
        }
    }

    protected void ExplosionProcessHits(IEnemy[] hitEnemies)
    {
        foreach (IEnemy hitEnemy in hitEnemies)
        {

            ProcessHits(hitEnemy);
        }
    }

    protected void DestroySpell()
    {
        InvokeEventIfBound(SpellNotActiveAnymore, this);

        if (trails.Count > 0)
        {
            for (int i = 0; i < trails.Count; i++)
            {
                trails[i].transform.parent = null;
                var ps = trails[i].GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                }
            }
        }

        speed = 0;
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }

        Quaternion rot = Quaternion.FromToRotation(Vector3.up, Vector3.down);
        Vector3 pos = transform.position;

        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot) as GameObject;

            var ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null)
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else
                Destroy(hitVFX, ps.main.duration);
        }

        StartCoroutine(DestroyParticleCoroutine(0f));
    }
        
    private IEnumerator DestroyParticleCoroutine(float waitTime)
    {
        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> tList = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform)
            {
                tList.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++)
                {
                    tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }

    private IEnumerator WaitBeforeDestroySpellCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        DestroySpell();
    }

    private void On_SPELLS_UniqueEffectActivated(ESpellID SpellID, EPlayerID PlayerID)
    {
        if (this != null)
        {
            if (MovementType == ESpellMovementType.UNIQUE)
            {
                if (CastingPlayerID == PlayerID)
                {
                    if (SpellID == ESpellID.PLASMA_FISSION)
                    {
                        AbstractSpell Part_1 = null;
                        AbstractSpell Part_2 = null;
                        foreach (var spell in SpellManager.Instance.AllSpells) // TODO: Optimize this algorithm !!!!!
                        {
                            if (spell.SpellID == ESpellID.PLASMA_FISSION_BALLS)
                            {
                                Part_1 = (AbstractSpell)spell;
                                Part_2 = (AbstractSpell)spell;
                            }
                        }

                        Vector3 rotation_1 = this.transform.rotation.eulerAngles + new Vector3(0, 90, 0);
                        Vector3 rotation_2 = this.transform.rotation.eulerAngles - new Vector3(0, 90, 0);

                        Quaternion Qrotation_1 = Quaternion.Euler(rotation_1);
                        Quaternion Qrotation_2 = Quaternion.Euler(rotation_2);
                        if (Part_1 != null && Part_2 != null)
                        {
                            AbstractSpell spell_Part_1 = Instantiate(Part_1, transform.position, Qrotation_1);
                            spell_Part_1.CastingPlayerID = CastingPlayerID;
                            AbstractSpell spell_Part_2 = Instantiate(Part_2, transform.position, Qrotation_2);
                            spell_Part_2.CastingPlayerID = CastingPlayerID;
                        }
                        DestroySpell();
                    }
                }
            }
        }
    }

}


