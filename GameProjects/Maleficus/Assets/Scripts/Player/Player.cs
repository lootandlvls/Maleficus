﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MaleficusMonoBehaviour, IPlayer                                                
{
    public EPlayerID PlayerID                                               { get; set; }
    public ETeamID TeamID                                                   { get; set; }
    public Vector3 Position                                                 { get { return transform.position; } }
    public Quaternion Rotation                                              { get { return transform.rotation; } }
    public bool IsDead                                                      { get; private set; } = false;                                          
    public Dictionary<ESpellSlot, bool> ReadyToUseSpell                     { get; } = new Dictionary<ESpellSlot, bool>();
    public Dictionary<ESpellSlot, float> SpellCooldown                      { get; } = new Dictionary<ESpellSlot, float>();
    public Dictionary<ESpellSlot, float> SpellCastDuration                      { get; } = new Dictionary<ESpellSlot, float>();
    public bool IsReadyToShoot                                              { get; set; }
    public bool IsPlayerCharging                                            { get; set; }
    public Vector3 SpellInitPosition                                        { get { return spellInitPosition.position; } }
    public Vector3 SpellEndPosition                                         { get { return spellEndPosition.position; } }
    public int SpellChargingLVL                                             { get { return spellChargingLVL; } }
    public bool hasCastedSpell = false;
   
    [Header("Charging Spell Effects")]
    [SerializeField] private GameObject chargingBodyEnergy;
    [SerializeField] private GameObject chargingWandEnergy;

   

    [SerializeField] private float angularSpeed;
    [SerializeField] private float speed;
    [Range(0.1f, 3.0f)]
    [SerializeField] private float fallingTime = 0.3f;
    [SerializeField] private float unhittableTime = 1.0f;
    [SerializeField] private Transform spellInitPosition;
    [SerializeField] private Transform spellEndPosition;

    

    private int spellChargingLVL = 1;

    private float lastTimeSinceRotated;

    private float currentSpeed;
    private IEnumerator UpdatePushVelocityEnumerator;
    private IEnumerator SpellChargingEnumerator;

    /// <summary> Spell Manager will initialize the variables for this Dictionary </summary> 
    private Dictionary<ESpellSlot, GameObject> chargingEffects = new Dictionary<ESpellSlot, GameObject>();

    private Vector3 movingDirection;
    //private Rigidbody myRigidBody;
    private DirectionalSprite[] myDirectionalSprites;
    private Animator myAnimator;

    private Vector3 pushVelocity;
    private Vector3 GravityVelocity;

    private bool HitEffectActive = false;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myDirectionalSprites = GetComponentsInChildren<DirectionalSprite>();
        myAnimator = GetComponent<Animator>();

    }

    protected override void Awake()
    {
        base.Awake();
        //TODO: only activate after respawn
        StartCoroutine(UnhittableCoroutine());
   

    }
    protected override void Start()
    {
        base.Start();

        IsReadyToShoot = false;
        IsPlayerCharging = false;

        InitializeDictionaries();

        myAnimator.SetBool("idle", true);

        currentSpeed = speed;
    }

    protected override void Update()
    {
        base.Update();

        if (true)//AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {

            JoystickInput playerInput = PlayerManager.Instance.GetPlayerInput(PlayerID);
            if (playerInput != null)
            {

                float Move_X = playerInput.JoystickValues[EInputAxis.MOVE_X];
                float Move_Y = playerInput.JoystickValues[EInputAxis.MOVE_Y];
                float Rotate_X = playerInput.JoystickValues[EInputAxis.ROTATE_X];
                float Rotate_Y = playerInput.JoystickValues[EInputAxis.ROTATE_Y];

                Move(Move_X, Move_Y);
                Rotate(Rotate_X, Rotate_Y);

                if (playerInput.HasMoved() == true)
                // Moving?
                {
                    if (myAnimator != null)
                    {
                        myAnimator.SetBool("idle", false);
                    }
                }
                // Not moving?
                else
                {
                    if (myAnimator != null)
                    {
                        myAnimator.SetBool("idle", true);
                    }
                   
                   
                }

                if (playerInput.HasRotated() == true)
                // Rotating?
                {

                    SetDirectionalSpritesVisible(true);
                    lastTimeSinceRotated = Time.time;
                }
                else
                // Not Rotating?
                {
                    SetDirectionalSpritesVisible(false);

                    if ((playerInput.HasMoved() == true)
                        && (Time.time - lastTimeSinceRotated > 0.5f))
                    // Moving for 1 second since last rortation?
                    {
                        LookAtMovingDirection(playerInput);
                    }
                }
            }
        }
    }

    
    #region INPUT
    private void Move(float axis_X, float axis_Z)
    {
        movingDirection = new Vector3(axis_X, 0.0f, axis_Z).normalized * (Mathf.Pow(axis_X, 2.0f) + Mathf.Pow(axis_Z, 2.0f));

        Vector3 movementVelocity = movingDirection * currentSpeed * 0.1f;
        
        Vector3 finalVelocity = movementVelocity + pushVelocity + GravityVelocity;
        transform.localPosition += finalVelocity * Time.deltaTime;
    }

    private void Rotate(float axis_X, float axis_Z)
    {
        DebugManager.Instance.Log(4, " PLAYER ROTATE ");
        if (Mathf.Abs(axis_X) + Mathf.Abs(axis_Z) > 0.0f)
        {
            RotateRelativeToGrandParentRotation(axis_X, -axis_Z);
        }
    }

    private void LookAtMovingDirection(JoystickInput playerInput)
    {
        float axis_X = playerInput.JoystickValues[EInputAxis.MOVE_X];
        float axis_Z = playerInput.JoystickValues[EInputAxis.MOVE_Y];

        RotateRelativeToGrandParentRotation(axis_X, axis_Z);
    }

    private void RotateRelativeToGrandParentRotation(float axis_X, float axis_Z)
    {
        Transform myGrandTransform = GetGrandFatherTransfrom();
        Vector3 newForwardDirection = myGrandTransform.TransformDirection(new Vector3(axis_X, 0.0f, axis_Z));
        Quaternion newRotation = Quaternion.LookRotation(newForwardDirection, transform.up);
        transform.rotation = newRotation;
    }


    public void StartChargingSpell(ISpell spell, ESpellSlot spellSlot)
    {
        if (IsPlayerCharging == false)
        {
            if (spell.MovementType == ESpellMovementType.LINEAR_HIT)
            {
                IsPlayerCharging = true;

                LogConsole("Player started Charging", "SPELL_CHARGE");
                StartNewCoroutine(ref SpellChargingEnumerator, SpellChargingCoroutine(spellSlot));
                //StartCoroutine(SpellChargingCoroutine(spellSlot));
            }
            
        }
    }

    public void StopChargingSpell(ISpell spell, ESpellSlot spellSlot)
    {
        LogConsole("player stopped charging " + spellSlot, "SPELL_CHARGE");
        IsPlayerCharging = false;

        if (spell.MovementType == ESpellMovementType.LINEAR_LASER)
        {
            StartCoroutine(SlowDownPlayerCoroutine(0, spell.CastDuration));
        }
        else
        {
            StopSlowDownPlayer();

        }
    }
 
    private void InitializeChargingEffects()
    {

    }

    private void InitializeDictionaries()
    {
        foreach (ESpellSlot spellSlot in Enum.GetValues(typeof(ESpellSlot)))
        {
            if (spellSlot != ESpellSlot.NONE)
            {
                ReadyToUseSpell[spellSlot] = true;
                AbstractSpell chosenSpell = SpellManager.Instance.GetChosenSpell(PlayerID, spellSlot);
                if (IS_NOT_NULL(chosenSpell))
                {
                    SpellCooldown[spellSlot] = chosenSpell.Cooldown;
                    SpellCastDuration[spellSlot] = chosenSpell.CastDuration;
                }
            }
        }
    }

    private IEnumerator SlowDownPlayerCoroutine(float slowDownSpeed, float duration)
    {
        myAnimator.SetBool("channeling", true);
        currentSpeed = slowDownSpeed;
        Debug.Log("PLAYER SLOWED : SPEED = " + currentSpeed);
        yield return new WaitForSeconds(duration);

        currentSpeed = speed;
        myAnimator.SetBool("channeling", false);
    }

    private void SlowDownPlayer(float slowDownSpeed)
    {
        myAnimator.SetBool("channeling", true);
        currentSpeed = slowDownSpeed;
    }

    private void StopSlowDownPlayer()
    {
        myAnimator.SetBool("channeling", false);
        currentSpeed = speed;
    }

    private IEnumerator SetReadyToUseSpellCoroutine(float time, ESpellSlot spellSlot)                    
    {
        yield return new WaitForSeconds(time);

        IsReadyToShoot = true;
        ReadyToUseSpell[spellSlot] = true;

        LogConsole("ready to use the spell again", "SPELL_CHARGE");
    }



    private IEnumerator SpellChargingCoroutine(ESpellSlot spellSlot)
    {
        LogConsole("Starting coroutine > " + "IsPlayerCharging : " + IsPlayerCharging + " | readyToUseSpell : " + ReadyToUseSpell[spellSlot] + " | IsReadyToShoot : " + IsReadyToShoot, "SPELL_CHARGE");
        while ((IsPlayerCharging == true)
            && ((ReadyToUseSpell[spellSlot] == false) || (IsReadyToShoot == false)))
        {
            LogConsole("IsPlayerCharging : " + IsPlayerCharging + " | readyToUseSpell : " + ReadyToUseSpell[spellSlot] + " | IsReadyToShoot : " + IsReadyToShoot, "SPELL_CHARGE");
            yield return new WaitForEndOfFrame();
        }

        if (IsPlayerCharging == true)
        {
            SlowDownPlayer(speed / 2.0f);
            myAnimator.SetBool("charging", true);
            int counter = 0;

            // Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 90, 1);
            Vector3 position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            GameObject wandEffect = Instantiate(chargingWandEnergy, position, transform.rotation);
            wandEffect.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
            GameObject bodyEffect = Instantiate(chargingBodyEnergy, transform.position, transform.rotation);
            bodyEffect.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
            bodyEffect.transform.parent = this.transform;
            wandEffect.transform.parent = this.transform;
            ParticleSystem particleSystemWandEffect = wandEffect.GetComponent<ParticleSystem>();
            ParticleSystem particleSystemBodyEffect = bodyEffect.GetComponent<ParticleSystem>();

            spellChargingLVL = 0;
            while (IsPlayerCharging)
            {
                var mainPS = particleSystemBodyEffect.main;
                mainPS.maxParticles = counter;
                mainPS.maxParticles = counter;


                yield return new WaitForSeconds(0.0f);
                counter += 4;       // TODO: Add how an attribute in spell to influence how fast second level is charged
                if (counter > 100)
                {
                    if (spellChargingLVL != 2)
                    {
                        spellChargingLVL = 2;
                        LogConsole("Spell upgraded to lvl 2", "SPELL_CHARGE");
                    }
                }
                else
                {
                    if (spellChargingLVL != 1)
                    {

                    spellChargingLVL = 1;
                    LogConsole("Spell upgraded to lvl 1", "SPELL_CHARGE");
                    }

                }
                yield return new WaitForEndOfFrame();
            }
            LogConsole("spellCharging function Done!!", "SPELL_CHARGE");

            myAnimator.SetBool("charging", false);
            StopSlowDownPlayer();

            particleSystemBodyEffect.Stop();
            particleSystemWandEffect.Stop();

            LogConsole("counter = " + counter, "SPELL_CHARGE");
        }
    }
    #endregion


    private void SetDirectionalSpritesVisible(bool isVisible)
    {
        if (myDirectionalSprites.Length == 0)
        {
            Debug.LogError("PLayer's directional sprites are empty!");
            myDirectionalSprites = GetComponentsInChildren<DirectionalSprite>();
        }

        foreach (DirectionalSprite directionalSprite in myDirectionalSprites)
        {
            if (isVisible == true)
            {
                directionalSprite.ShowSprite();
            }
            else
            {
                directionalSprite.HideSprite();
            }
        }
    }

    public void DoShockwaveAnimation()
    {
        myAnimator.SetTrigger("shockwave");
    }

    public void DoTeleportAnimation()
    {
        myAnimator.SetTrigger("teleport");
    }

    public void DoProjectileAttackAnimation()
    {
        myAnimator.SetTrigger("projectileAttack");
    }


    public void SetPlayerFrozen(bool isFrozen)
    {
        if (isFrozen == true)
        {
            currentSpeed = 0;
        }
        else
        {
            currentSpeed = speed;
        }
    }

    public void SetPlayerStunned(bool isStunned)
    {
        if (isStunned == true)
        {
            currentSpeed = 0;
        }
        else
        {
            currentSpeed = speed;
        }
    }

    public void SetPlayerParalyzed(bool isParalyzed , int effectStrenght)
    {
        if (isParalyzed == true)
        {
            currentSpeed = speed / effectStrenght;
        }
        else
        {
            currentSpeed = speed;
        }
    }

    public void SetPlayerSpeedBoost(int speedBoost)
    {
        currentSpeed = speed * speedBoost;
    }

    public void resetSpellChargingLVL()
    {
        spellChargingLVL = 1;
    }

    public void PushPlayer(Vector3 velocity, float duration)
    {
        pushVelocity = velocity;

        if (duration <= 0.0f)
        {
            duration = 0.1f;
        }
        StartNewCoroutine(ref UpdatePushVelocityEnumerator, UpdatePushVelocityCoroutine(duration));
    }

    
  

    private IEnumerator UpdatePushVelocityCoroutine(float duration)
    {
        float startTime = Time.time;
        Vector3 startVelocity = pushVelocity;

        float progress = (Time.time - startTime) / duration;
        while (progress < 1.0f)
        {
            progress = (Time.time - startTime) / duration;

            pushVelocity = Vector3.Lerp(startVelocity, Vector3.zero, progress);

            yield return new WaitForEndOfFrame();
        }

    }

   
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Ground"))
        {
            GravityVelocity = new Vector3(0, -9.81f, 0);
            IsDead = true;
            StopAllCoroutines();
            IsReadyToShoot = false;

            StartCoroutine(FallingCoroutine());
        }
    }

    private IEnumerator FallingCoroutine()
    {

        yield return new WaitForSeconds(fallingTime);
        PlayerManager.Instance.OnPlayerDead(PlayerID);

    }

    public void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    private Transform GetGrandFatherTransfrom()
    {
        Transform myGrandParentTransform = transform;
        while (myGrandParentTransform.parent != null)
        {
            myGrandParentTransform = myGrandParentTransform.parent;
        }

        return myGrandParentTransform;
    }

    public void ActivateShield(float duration)
    {
        StartCoroutine(ShieldActivatedCoroutine(duration));
    }
    private IEnumerator ShieldActivatedCoroutine(float duration)
    {
        this.tag  = Maleficus.MaleficusConsts.TAG_PLAYER_SHIELDED;
        yield return new WaitForSeconds(duration);
        this.tag = Maleficus.MaleficusConsts.TAG_PLAYER;
    }
   private IEnumerator UnhittableCoroutine()
    {
        this.tag = "Unhittable";
       
        GameObject effectPrefab = Resources.Load<GameObject>(Maleficus.MaleficusConsts.PATH_EFFECT_UNHITTABLE);
        GameObject effect = Instantiate(effectPrefab, transform.position, transform.rotation);
        effect.transform.parent = this.transform;
        SpellTimer spellTimer = effect.GetComponent<SpellTimer>();
        if (IS_NOT_NULL(spellTimer))
        {
            spellTimer.time = unhittableTime;
        }
        yield return new WaitForSeconds(unhittableTime);

        this.tag = "Player";
        IsReadyToShoot = true;
    }
}
