using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpell : MaleficusMonoBehaviour, ISpell
{
    //  private Vector3 movingDirection;



    public int HitPower { get {
           // Debug.Log("$%&$/$%& Hit power : " + hitPower);
            return hitPower; } }

    public float Speed { get { return speed; } }

    public Vector3 Direction { get { return direction; } }

    public Vector3 EndDestination { get; set; }

    public string SpellName { get { return spellName; } }

    public int SpellLevel { get { return spellLevel; } }

    public bool HasPower { get { return hasPushPower; } }

    public ESpellMovementType MovementType { get { return movementType; } }

    public List<ESpellEffects> DebuffEffects { get { return debuffEffects; } }

    public List<ESpellEffects> BuffEffects { get { return buffEffects; } }

    public EPlayerID CastingPlayerID { get; set; }

    public float Cooldown { get { return cooldown; } }

    public float Duration { get { return spellDuration; } }

    public float PushDuration { get { return pushDuration; } }

    public ESpellID SpellID { get { return spell; } }

    public bool IsChargeable { get { return isChargeable; } }

    public bool IsTripleCast { get { return isTripleCast; } }

    public Sprite SpellIcon { get { return spellIcon; } }

    public int SkillPoint { get { return skillPoint; } }

    public AudioClip CastSound { get { return castSound; } }

    public AudioClip HitSound { get { return hitSound; } }

    [SerializeField] public int hitPower;
    [SerializeField] public float speed;
    [SerializeField] private string spellName;
    [SerializeField] private int spellLevel;
    [SerializeField] private bool OnSelfEffect;
    [SerializeField] private bool hasPushPower;
    [SerializeField] private ESpellID spell;
    [SerializeField] private bool isChargeable;
    [SerializeField] private Sprite spellIcon;
    [SerializeField] private AudioClip castSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private int skillPoint;
    [SerializeField] private bool isTripleCast;


    [SerializeField] private ESpellMovementType movementType;
    [SerializeField] private List<ESpellEffects> debuffEffects;
    [SerializeField] private List<ESpellEffects> buffEffects;

    [SerializeField] private float cooldown;
    [SerializeField] private float spellDuration;
    [SerializeField] private float pushDuration;

    protected Rigidbody myRigidBody;

    protected Vector3 direction;


    public AudioClip shotSFX;
    public AudioClip hitSFX;
    public GameObject hitPrefab;
    public List<GameObject> trails;
    public Vector3 parabolicSpell_EndPosition;



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        direction = new Vector3(0, 0, 0);
        myRigidBody = GetComponent<Rigidbody>();
        // Activates a Spell unique Abilities
        EventManager.Instance.SPELLS_UniqueEffectActivated += On_SPELLS_UniqueEffectActivated;

        if (OnSelfEffect)
        {
            SHitInfo hitInfo = new SHitInfo(this, CastingPlayerID, CastingPlayerID, transform.position, hasPushPower, isChargeable , isTripleCast, debuffEffects, buffEffects);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
        }
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
                        foreach (var spell in SpellManager.Instance.All_Spells)
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

    protected void ProcessHits(IPlayer[] hitPlayers)
    {
        foreach (IPlayer hitPlayer in hitPlayers)
        {

            // Debug.Log(dirVector);
            SHitInfo hitInfo = new SHitInfo(this, CastingPlayerID, hitPlayer.PlayerID, hitPlayer.Position, hasPushPower, isChargeable, isTripleCast, debuffEffects, buffEffects);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
            if (SpellID != ESpellID.AOE_EXPLOSION && SpellID != ESpellID.FIRE_LASER && SpellID != ESpellID.FIRE_SHOCKBLAST)
            {
                DestroySpell();
            }
            
           
        }
    }

    protected void ProcessHits(IPlayer hitPlayer)
    {
        if (hitPlayer.IsDead == false)
        {
          //  Debug.Log("333333333333333333333333 player hit");
            SHitInfo hitInfo = new SHitInfo(this, CastingPlayerID, hitPlayer.PlayerID, hitPlayer.Position, hasPushPower, isChargeable, isTripleCast, debuffEffects, buffEffects);
           
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
            
           if (SpellID != ESpellID.AOE_EXPLOSION && SpellID != ESpellID.FIRE_LASER && SpellID != ESpellID.FIRE_SHOCKBLAST )
           {
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
    //Funtion to destroy the spell
    public void DestroySpell()
    {

        if (shotSFX != null && GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().PlayOneShot(hitSFX);
        }

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

        StartCoroutine(DestroyParticle(0f));

    }
    public IEnumerator DestroyParticle(float waitTime)
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




}
