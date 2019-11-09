using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MaleficusMonoBehaviour, IPlayer                                                
{
    public EPlayerID PlayerID                                               { get; set; }
    public ETeamID TeamID                                                   { get; set; }
    public Vector3 Position                                                 { get { return transform.position; } }
    public Quaternion Rotation                                              { get { return transform.rotation; } }
    public bool IsDead                                                      { get { return isDead; } }                                            // TODO: Define when player is dead
    public Dictionary<ESpellSlot, bool> ReadyToUseSpell                     { get { return readyToUseSpell; } }
    public Dictionary<ESpellSlot, float> SpellCooldown                      { get { return spellCooldown; } }
    public Dictionary<ESpellSlot, float> SpellDuration                      { get { return spellDuration; } }
    public bool IsReadyToShoot                                              { get; set; }
    public bool IsPlayerCharging                                            { get; set; }

    public Vector3 SpellInitPosition                                        { get { return spellInitPosition.position; } }
    public Vector3 SpellEndPosition                                         { get { return spellEndPosition.position; } }
    public int SpellChargingLVL                                             { get { return spellChargingLVL; } }

    [Header("Charging Spell Effects")]
    [SerializeField] private GameObject chargingBodyEnergy;
    [SerializeField] private GameObject chargingWandEnergy;

    [SerializeField] private float angularSpeed;
    [SerializeField] private float speed;

    [SerializeField] private Transform spellInitPosition;
    [SerializeField] private Transform spellEndPosition;

    private String playerVerticalInput;
    private String playerHorizontalInput;

    private int spellChargingLVL = 1;

    private float lastTimeSinceRotated;

    private float currentSpeed;
    private IEnumerator UpdatePushVelocityEnumerator;
    private IEnumerator SpellChargingEnumerator;

    /// <summary> Spell Manager will initialize the variables for this Dictionary </summary> 
    Dictionary<ESpellSlot, GameObject> ChargingEffects = new Dictionary<ESpellSlot, GameObject>();

    
    private Vector3 movingDirection;
    //private Rigidbody myRigidBody;
    private DirectionalSprite[] myDirectionalSprites;
    private Dictionary<int, AbstractSpell> spellsSlot;
    private Animator myAnimator;

    private Vector3 pushVelocity;
    private Vector3 GravityVelocity;

    private bool isDead = false;


    // TODO [Nassim]: refactor multiple variables into a dictionary
    private Dictionary<ESpellSlot, bool> readyToUseSpell = new Dictionary<ESpellSlot, bool>();

    [Header("SpellsCooldown")]
    private Dictionary<ESpellSlot, float> spellCooldown = new Dictionary<ESpellSlot, float>();

    [Header("SpellsDuration")]
    private Dictionary<ESpellSlot, float> spellDuration = new Dictionary<ESpellSlot, float>();


    protected override void Awake()
    {
        base.Awake();

        myDirectionalSprites = GetComponentsInChildren<DirectionalSprite>();

        IsReadyToShoot = true;
        IsPlayerCharging = false;
    }

    protected override void Start()
    {
        base.Start();

        InitializeDictionaries();

        //myRigidBody = this.GetComponent<Rigidbody>();      
        myAnimator = GetComponent<Animator>();
        myAnimator.SetBool("idle", true);

        currentSpeed = speed;

    }

    protected override void Update()
    {
        base.Update();

        if (true) //AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {
            JoystickInput playerInput = PlayerManager.Instance.GetPlayerInput(PlayerID);
            if (playerInput != null)
            {
                float Move_X = playerInput.JoystickValues[EInputAxis.MOVE_X];
                float Move_Y = playerInput.JoystickValues[EInputAxis.MOVE_Y];
                float Rotate_X = playerInput.JoystickValues[EInputAxis.ROTATE_X];
                float Rotate_Y = playerInput.JoystickValues[EInputAxis.ROTATE_Y];

                //Move(Move_X, Move_Y);
                //Rotate(Rotate_X, Rotate_Y);

                if (playerInput.HasMoved() == true)
                // Moving?
                {
                    myAnimator.SetBool("idle", false);
                }
                // Not moving?
                else
                {
                    myAnimator.SetBool("idle", true);
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
                        //LookAtMovingDirection(playerInput);
                    }
                }
            }
        }
    }

    
    #region INPUT
    private void Move(float axis_X, float axis_Z)
    {
        movingDirection = new Vector3(axis_X, 0.0f, axis_Z).normalized * Mathf.Max(Mathf.Abs(axis_X), Mathf.Abs(axis_Z));

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

                Debug.Log("Player started Charging");
                StartNewCoroutine(ref SpellChargingEnumerator, SpellChargingCoroutine(spellSlot));
                //StartCoroutine(SpellChargingCoroutine(spellSlot));
            }
            else if (spell.MovementType == ESpellMovementType.LINEAR_LASER)
            {
                StartCoroutine(SlowDownPlayerCoroutine(speed / 2.0f, spell.CastingDuration));
            }
        }
    }

    public void StopChargingSpell(ISpell spell, ESpellSlot spellSlot)
    {
        Debug.Log("player stopped charging " + spellSlot);
        IsPlayerCharging = false;

        if (spell.MovementType == ESpellMovementType.LINEAR_LASER)
        {
            StartCoroutine(SlowDownPlayerCoroutine(speed / 10.0f, spell.CastingDuration));
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
        readyToUseSpell[ESpellSlot.SPELL_1] = true;
        readyToUseSpell[ESpellSlot.SPELL_2] = true;
        readyToUseSpell[ESpellSlot.SPELL_3] = true;

        spellCooldown[ESpellSlot.SPELL_1] = SpellManager.Instance.Player_Spells[PlayerID][ESpellSlot.SPELL_1].Cooldown;
        spellCooldown[ESpellSlot.SPELL_2] = SpellManager.Instance.Player_Spells[PlayerID][ESpellSlot.SPELL_2].Cooldown;
        spellCooldown[ESpellSlot.SPELL_3] = SpellManager.Instance.Player_Spells[PlayerID][ESpellSlot.SPELL_3].Cooldown;

        spellDuration[ESpellSlot.SPELL_1] = SpellManager.Instance.Player_Spells[PlayerID][ESpellSlot.SPELL_1].CastingDuration;
        spellDuration[ESpellSlot.SPELL_2] = SpellManager.Instance.Player_Spells[PlayerID][ESpellSlot.SPELL_2].CastingDuration;
        spellDuration[ESpellSlot.SPELL_3] = SpellManager.Instance.Player_Spells[PlayerID][ESpellSlot.SPELL_3].CastingDuration;
    }


    private IEnumerator SlowDownPlayerCoroutine(float slowDownSpeed, float duration)
    {
        myAnimator.SetBool("channeling", true);
        currentSpeed = slowDownSpeed;

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
        readyToUseSpell[spellSlot] = true;

        Debug.Log("ready to use the spell again");
    }



    private IEnumerator SpellChargingCoroutine(ESpellSlot spellSlot)
    {
        Debug.Log("Starting coroutine > " + "IsPlayerCharging : " + IsPlayerCharging + " | readyToUseSpell : " + readyToUseSpell[spellSlot] + " | IsReadyToShoot : " + IsReadyToShoot);
        while ((IsPlayerCharging == true)
            && ((readyToUseSpell[spellSlot] == false) || (IsReadyToShoot == false)))
        {
            Debug.Log("IsPlayerCharging : " + IsPlayerCharging + " | readyToUseSpell : " + readyToUseSpell[spellSlot] + " | IsReadyToShoot : " + IsReadyToShoot);
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

            while (IsPlayerCharging)
            {
                var mainPS = particleSystemBodyEffect.main;
                mainPS.maxParticles = counter;
                mainPS.maxParticles = counter;


                yield return new WaitForSeconds(0.0f);
                counter += 4;       // TODO: Add how an attribute in spell to influence how fast second level is charged
                if (counter > 100)
                {
                    spellChargingLVL = 2;
                    //Debug.Log("Spell upgraded to lvl 2");
                }
                else
                {
                    spellChargingLVL = 1;
                    //Debug.Log("Spell upgraded to lvl 1");
                }
                yield return new WaitForEndOfFrame();
            }
            Debug.Log("spellCharging function Done!!");

            myAnimator.SetBool("charging", false);
            StopSlowDownPlayer();

            particleSystemBodyEffect.Stop();
            particleSystemWandEffect.Stop();

            Debug.Log("counter = " + counter);
        }
    }
    #endregion


    private void SetDirectionalSpritesVisible(bool isVisible)
    {
        if (myDirectionalSprites.Length == 0)
        {
            Debug.Log("directional sprites empty");
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

    public void resetSpellChargingLVL()
    {
        spellChargingLVL = 1;
    }

    public void PushPlayer(Vector3 velocity, float duration)
    {
        pushVelocity = velocity;
        Debug.Log("§$%§$% Player pushed : " + velocity);

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
            isDead = true;
            StopAllCoroutines();
            IsReadyToShoot = false;
            PlayerManager.Instance.OnPlayerOutOfBound(PlayerID);
        }
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
}
