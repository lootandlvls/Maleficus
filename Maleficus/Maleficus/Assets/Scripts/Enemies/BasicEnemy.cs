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
    //[SerializeField] protected GestureSymbol[] gestureSymbols;
    [SerializeField] protected GameObject damageParticleEffect;
    [SerializeField] protected GameObject spawnParticleEffect;
    [SerializeField] private AudioClip[] attackSounds;

    protected Vector3 playerPosition;
    //protected Gesture[] gestures;
    //protected Gesture currentGesture;
    protected int gestureIndex;
    protected Animator myAnimator;
    protected NavMeshAgent myNavAgent;
    protected AudioSource myAudioSource;


    protected enum State
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
    protected State myState;

    
    protected virtual void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myNavAgent = GetComponent<NavMeshAgent>();
        myAudioSource = SoundUtilities.AddAudioListener(gameObject, false, 0.15f, false);


        if (spawnParticleEffect != null)
        {
            GameObject obj = Instantiate(spawnParticleEffect, transform.position, Quaternion.identity);
            obj.transform.parent = transform;
        }
        
    }


    protected virtual void Start()
    {
        // Init events
        //EnemyAttacked   += EnemyManager.Instance.Action_OnEnemyAttacked;
        //EnemyDied       += EnemyManager.Instance.Action_OnEnemyDied;


        //AppStateManager.Instance.AppStateUpdated        += OnAppStateUpdated;
        EventManager.Instance.ÁPP_AppStateUpdated        += OnAppStateUpdated;

        //PlayersManager.Instance.PlayerPerformedGesture  += OnPlayerPerformedGesture;

        // Init gestures
        //InitGestureSymbols();
        

        // Init Enemy
        UpdateState(State.SPAWNING);


        //playerPosition = PlayersManager.Instance.PlayersPosition;
        myNavAgent.speed = walkingSpeed;


        transform.LookAt(playerPosition);
    }

    protected virtual void Update()
    {
        // Update player position
        foreach(Player player in PlayerManager.Instance.ActivePlayers.Values)
        {
            playerPosition = player.Position;
            break;
        }

        switch (myState)
        {
            case State.SPAWNING:
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    UpdateState(State.MOVING_TOWARDS_PLAYER);
                }
                break;

            case State.MOVING_TOWARDS_PLAYER:
                if (Vector3.Distance(transform.position, playerPosition) < attackingThreshold)
                {
                    UpdateState(State.ATTACKING);
                }
                break;

            case State.ATTACKING:
                Vector3 playersDir = (playerPosition - transform.position).normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playersDir, transform.up), Time.deltaTime * 1);
                break;

            case State.GETTING_HIT:
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    UpdateState(State.MOVING_TOWARDS_PLAYER);
                }
                break;
        }


    }

    //protected void InitGestureSymbols()
    //{
    //    Debug.Log("Init gestures");
    //    gestures = new Gesture[gestureSymbols.Length];
    //    for (int i = 0; i < gestures.Length; i++)
    //    {
    //        gestures[i] = PosturesAndGesturesDataBase.Instance.GetARandomGesture();
    //        gestures[i].ToPlayerID = (PlayerID)(Utils.GetRndIndex(PlayersManager.PLAYER_NUMBERS) + 1);
    //        gestureSymbols[i].SetSprite(gestures[i].GestureSymbol);
    //    }
    //    currentGesture = gestures[0];
    //    gestureIndex = 0;
    //}


    protected virtual void UpdateState(State newState)
    {
        myState = newState;

        // Make sure to stop attacking 
        StopAllCoroutines();

        switch(newState)
        {
            case State.MOVING_TOWARDS_PLAYER:
                myAnimator.SetBool("IsMoving", true);
                myNavAgent.speed = walkingSpeed;
                myNavAgent.destination = playerPosition;
                break;

            case State.ATTACKING:
                myAnimator.SetBool("IsMoving", false);
                myNavAgent.speed = 0;
                AttackEveryPeriodOfTime();

                break;

            case State.GETTING_HIT:
                myNavAgent.speed = 0;
                myAnimator.SetTrigger("GetHit");
                
                break;

            case State.DEAD:
                StopAllCoroutines();
                myNavAgent.speed = 0;
                myAnimator.SetBool("IsMoving", false);
                myAnimator.SetTrigger("Die");
                myNavAgent.enabled = false;


                //AppStateManager.Instance.AppStateUpdated        -= OnAppStateUpdated;
                EventManager.Instance.ÁPP_AppStateUpdated-= OnAppStateUpdated;

                //PlayersManager.Instance.PlayerPerformedGesture  -= OnPlayerPerformedGesture;
                //EnemyAttacked -= EnemyManager.Instance.Action_OnEnemyAttacked;


                //if(EnemyDied != null) EnemyDied.Invoke(this);
                EventManager.Instance.Invoke_ENEMIES_EnemyDied(this);

                DieAndVanish();
                break;

            case State.IDLE:
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
        while(myState == State.ATTACKING)
        {
            int attackID = UnityEngine.Random.Range(1, 3);
            Debug.Log(attackID);
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

        //if (EnemyAttacked != null) EnemyAttacked.Invoke(this);
        EventManager.Instance.Invoke_ENEMIES_EnemyAttackedPlayer(this);
    }


    protected void OnAppStateUpdated(EAppState newState, EAppState lastState)
    {
        if (newState == EAppState.IN_GAME_IN_ENDED)
        {
            UpdateState(State.IDLE);
        }
    }

    //protected virtual void OnPlayerPerformedGesture(PlayerID byPlayer, GestureType gestureType)
    //{
    //    if (AppStateManager.Instance.CurrentState == AppState.PLAYING)
    //    {
    //        if ((currentGesture.ToPlayerID == byPlayer) && (currentGesture.IsType == gestureType))
    //        {
    //            // Instantiate Lazer from player
    //            PlayersManager.Instance.SpawnLazer(byPlayer, gestureType, transform);
    //            PlayersManager.Instance.SpawnSpellBall(byPlayer, gestureType, transform);

    //            // Instantiate particle effect
    //            if (damageParticleEffect != null)
    //            {
    //                Instantiate(damageParticleEffect, transform.position, Quaternion.identity);
    //            }

    //            // Update gesture and check if dead
    //            gestureIndex++;
    //            if (gestureIndex == gestures.Length)
    //            {
    //                gestureSymbols[gestureIndex - 1].HideSprite();
    //                UpdateState(State.DEAD);
    //            }
    //            else
    //            {
    //                currentGesture = gestures[gestureIndex];
    //                gestureSymbols[gestureIndex - 1].HideSprite();
    //                UpdateState(State.GETTING_HIT);

    //            }
    //        }
    //    }
        
    //}

}
