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

    private const float WAYPOINT_THRESHOLD = 2.0f;

    public int Damage { get { return damage; } }
    public float AttackTimeout { get { return attackTimeOut; } }
    public float SpawnRate { get { return spawnRate; } }
    public EEnemyType IsType { get { return isType; } }


    [Header("Attributes")]
    [SerializeField] protected EEnemyType isType;
    [SerializeField] protected EEnemyMovementType movementType;
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


    private EnemyWayPoint[] wayPoints;

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
        if (movementType == EEnemyMovementType.WAYPOINTS)
        {
            myNavAgent.enabled = false;
        }
        myAudioSource = SoundUtilities.AddAudioListener(gameObject, false, 0.15f, false);

        FindAllWayPoints();
    }



    protected virtual void Start()
    {
        EventManager.Instance.ÁPP_AppStateUpdated   += On_APP_AppStateUpdated;
        EventManager.Instance.SPELLS_SpellHitEnemy  += On_SPELLS_SpellHitEnemy;

        // Init Enemy
        UpdateState(EnemyState.SPAWNING);

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

                if (movementType == EEnemyMovementType.NAV_MESH)
                {
                    myNavAgent.destination = playerPosition;
                    transform.LookAt(playerPosition);
                }
                else if (movementType == EEnemyMovementType.WAYPOINTS)
                {
                    Vector3 destination = GetNextDestinationToPlayer();
                    Vector3 direction = (destination - transform.position).normalized;
                    transform.position += direction * walkingSpeed * Time.deltaTime * ARManager.Instance.SizeFactor;
                    transform.LookAt(destination);
                }

                if (Vector3.Distance(transform.position, playerPosition) < attackingThreshold * ARManager.Instance.SizeFactor)
                {
                    UpdateState(EnemyState.ATTACKING);
                }
                break;

            case EnemyState.ATTACKING:
                Vector3 playersDir = (playerPosition - transform.position).normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playersDir, transform.up), Time.deltaTime * 1);

                if (Vector3.Distance(transform.position, playerPosition) > attackingThreshold * ARManager.Instance.SizeFactor)
                {
                    UpdateState(EnemyState.MOVING_TOWARDS_PLAYER);
                }
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

                if (movementType == EEnemyMovementType.NAV_MESH)
                {
                    myNavAgent.speed = walkingSpeed;
                    if (MotherOfManagers.Instance.IsARGame == true)
                    {
                        myNavAgent.speed *= ARManager.Instance.SizeFactor;
                    }
                    myNavAgent.destination = playerPosition;
                }
                else if (movementType == EEnemyMovementType.WAYPOINTS)
                {

                }
                
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
            Vector3 vanishDownUpdate = Vector3.down * Time.deltaTime * 0.5f;
            if (MotherOfManagers.Instance.IsARGame == true)
            {
                vanishDownUpdate *= ARManager.Instance.SizeFactor;
            }
            transform.position += vanishDownUpdate;
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




    private void FindAllWayPoints()
    {
        List<EnemyWayPoint> temp = new List<EnemyWayPoint>();
        foreach (EnemyWayPoint waypoint in FindObjectsOfType<EnemyWayPoint>())
        {
            temp.Add(waypoint);
        }
        wayPoints = temp.ToArray();
    }


    private Vector3 GetNextDestinationToPlayer()
    {
        float minDistance = Vector3.Distance(transform.position, playerPosition);
        Vector3 nextDestination = playerPosition;
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
        foreach(EnemyWayPoint wayPoint in wayPoints)
        {
            Vector3 directionToWayPoint = (wayPoint.Position - transform.position).normalized;
            float distanceToWayPoint = Vector3.Distance(transform.position, wayPoint.Position);
            float dotProduct = Vector3.Dot(directionToPlayer, directionToWayPoint);
            if ((distanceToWayPoint < minDistance) && (dotProduct > 0.0f) && (distanceToWayPoint > WAYPOINT_THRESHOLD * ARManager.Instance.SizeFactor))
            {
                minDistance = distanceToWayPoint;
                nextDestination = wayPoint.Position;
                Debug.Log(wayPoint.name + " is nearer");
            }
        }
        return nextDestination;
    }
}




