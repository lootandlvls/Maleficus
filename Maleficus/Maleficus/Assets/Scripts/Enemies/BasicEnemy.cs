using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Klasse fuer Huendchen Gegner
public class BasicEnemy : MonoBehaviour, IEnemy
{
    //public event Action<IEnemy> EnemyAttacked;
    //public event Action<IEnemy> EnemyDied;

    public int Damage { get { return damage; } }
    public float AttackTimeout { get { return attackTimeOut; } }
    public float SpawnRate { get { return spawnRate; } }
    public EnemyType IsType { get { return isType; } }


    [Header("Attributes")]
    [SerializeField] protected EnemyType isType;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float attackTimeOut = 3;
    [SerializeField] protected float attackDelay = 0.5f;
    [SerializeField] protected float spawnRate = 3;
    [SerializeField] protected float walkingSpeed = 1.5f;
    [SerializeField] protected float attackingThreshold = 1.5f;
    [Header("Assign")]
    [SerializeField] protected GameObject damageParticleEffect;
    [SerializeField] protected GameObject spawnParticleEffect;
    [SerializeField] private AudioClip[] attackSounds;

    protected Vector3 playerPosition;

    protected int gestureIndex;
    protected Animator myAnimator;
    protected NavMeshAgent myNavAgent;
    protected AudioSource myAudioSource;


    protected enum EnemyState
    {
        SPAWNING,
        MOVING_TOWARDS_PLAYER,
        ATTACKING,
        GETTING_HIT,
        DEAD,
        IDLE,
        KNOCKED,
        ROAR
    }
    protected EnemyState myState;

    
    protected virtual void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myNavAgent = GetComponent<NavMeshAgent>();
        myAudioSource = SoundUtilities.AddAudioListener(gameObject, false, 0.15f, false);
       

      /*  if (spawnParticleEffect != null)
        {
            GameObject obj = Instantiate(spawnParticleEffect, transform.position, Quaternion.identity);
            obj.transform.parent = transform;
        }*/
        
    }


    protected virtual void Start()
    {
        EventManager.Instance.ÁPP_AppStateUpdated   += On_APP_AppStateUpdated;
        EventManager.Instance.SPELLS_SpellHitEnemy  += On_SPELLS_SpellHitEnemy;

        // Init Enemy
        UpdateState(EnemyState.SPAWNING);
        transform.localScale *= ARManager.Instance.SizeFactor;
        myNavAgent.speed = walkingSpeed;
    }

    private void On_SPELLS_SpellHitEnemy(IEnemy hitEnemy)
    {
        Debug.Log("Enemy attacked : " + ((BasicEnemy)hitEnemy == this).ToString());
        if ((BasicEnemy) hitEnemy == this)
        {
            UpdateState(EnemyState.DEAD);
        }
    }

    protected virtual void Update()
    {
        
        // Update player position
        foreach (Player player in PlayerManager.Instance.ActivePlayers.Values)
        {
            playerPosition = player.Position;
            break;
        }



        // Update enemy state
        switch (myState)
        {
            case EnemyState.SPAWNING:
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    UpdateState(EnemyState.MOVING_TOWARDS_PLAYER);
                }
                break;

            case EnemyState.MOVING_TOWARDS_PLAYER:
                myNavAgent.destination = playerPosition;
                transform.LookAt(playerPosition);

                float distanceFactor = 1.0f;
                if (MotherOfManagers.Instance.IsARGame == true)
                {
                    distanceFactor = ARManager.Instance.SizeFactor;
                }

                if (Vector3.Distance(transform.position, playerPosition) < attackingThreshold * distanceFactor)
                {
                    UpdateState(EnemyState.ATTACKING);
                }
                break;

            case EnemyState.ATTACKING:
                Vector3 playersDir = (playerPosition - transform.position).normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playersDir, transform.up), Time.deltaTime * 1);
                break;

            case EnemyState.GETTING_HIT:
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    UpdateState(EnemyState.MOVING_TOWARDS_PLAYER);
                }
                break;
        }


    }


    protected virtual void UpdateState(EnemyState newState)
    {
        myState = newState;

        // Make sure to stop attacking 
        StopAllCoroutines();

        switch(newState)
        {
            case EnemyState.MOVING_TOWARDS_PLAYER:
                myAnimator.SetBool("IsMoving", true);
                myNavAgent.speed = walkingSpeed;
                if (MotherOfManagers.Instance.IsARGame == true)
                {
                    myNavAgent.speed *= ARManager.Instance.SizeFactor;
                }
                myNavAgent.destination = playerPosition;
                break;

            case EnemyState.ATTACKING:
                myAnimator.SetBool("IsMoving", false);
                myNavAgent.speed = 0;
                AttackEveryPeriodOfTime();

                break;

            case EnemyState.GETTING_HIT:
                myNavAgent.speed = 0;
                myAnimator.SetTrigger("GetHit");
                
                break;

            case EnemyState.DEAD:
                StopAllCoroutines();
                myNavAgent.speed = 0;
                myAnimator.SetBool("IsMoving", false);
                myAnimator.SetTrigger("Die");
                myNavAgent.enabled = false;


                EventManager.Instance.ÁPP_AppStateUpdated-= On_APP_AppStateUpdated;
                
                EventManager.Instance.Invoke_ENEMIES_EnemyDied(this);

                DieAndVanish();
                break;

            case EnemyState.IDLE:
                myAnimator.SetBool("IsMoving", false);
                myNavAgent.speed = 0;
                break;
        }
    }


    private void AttackEveryPeriodOfTime()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        while(myState == EnemyState.ATTACKING)
        {
            int attackID = UnityEngine.Random.Range(1, 3);
            if (attackID == 1)
            {
                myAnimator.SetTrigger("Attack1");
            }
            else
            {
                myAnimator.SetTrigger("Attack2");
            }
            
            InvokeAttackedEvent();
            yield return new WaitForSeconds(attackTimeOut);
        }
    }

    protected void DieAndVanish()
    {
        StartCoroutine(DieCoroutine());
    }
    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(1);
        float timer = Time.time;
        while (Time.time - timer < 3)
        {
            transform.position += Vector3.down * Time.deltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
    
    protected void InvokeAttackedEvent()
    {
        StartCoroutine(InvokeAttackedEventCoroutine());   
    }
    private IEnumerator InvokeAttackedEventCoroutine()
    {
        yield return new WaitForSeconds(attackDelay);
        SoundUtilities.PlayRandomSound(myAudioSource, attackSounds);

        EventManager.Instance.Invoke_ENEMIES_EnemyAttackedPlayer(this);
    }


    protected void On_APP_AppStateUpdated(EAppState newState, EAppState lastState)
    {
        if (newState == EAppState.IN_GAME_IN_ENDED)
        {
            UpdateState(EnemyState.IDLE);
        }
    }



}
