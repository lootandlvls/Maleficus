using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer                                                
{
    public EPlayerID PlayerID { get; set; }
    public ETeamID TeamID { get; set; }
    public Vector3 Position { get { return transform.position; } }
    public Quaternion Rotation { get { return transform.rotation; } }
    public bool IsARPlayer { get; set; }
    public bool IsDead { get { return isDead; } }                                            // TODO: Define when player is dead
    public Dictionary<ESpellSlot, bool> ReadyToUseSpell { get { return readyToUseSpell; } }
    public Dictionary<ESpellSlot, float> SpellCooldown { get { return spellCooldown; } }
    public Dictionary<ESpellSlot, float> SpellDuration { get { return spellDuration; } }
    public bool IsReadyToShoot { get; set; }
    public bool IsPlayerCharging { get; set; }

    public Vector3 SpellInitPosition { get { return spellInitPosition.position; } }
    public Vector3 SpellEndPosition { get { return spellEndPosition.position; } }
    public int SpellChargingLVL { get { return spellChargingLVL; } }

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


    private void Awake()
    {
        myDirectionalSprites = GetComponentsInChildren<DirectionalSprite>();

        IsReadyToShoot = true;
        IsPlayerCharging = false;
    }

    private void Start()
    {
        InitializeDictionaries();

        //myRigidBody = this.GetComponent<Rigidbody>();      
        myAnimator = this.GetComponent<Animator>();
        myAnimator.SetBool("idle", true);

        if (IsARPlayer == true)
        {
            speed *= ARManager.Instance.SizeFactor;
        }
        currentSpeed = speed;

    }

    private void Update()
    {
        if (true) //AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {
            ControllerInput playerInput = PlayerManager.Instance.GetPlayerInput(PlayerID);

            float Move_X = playerInput.JoystickValues[EInputAxis.MOVE_X];
            float Move_Y = playerInput.JoystickValues[EInputAxis.MOVE_Y];
            float Rotate_X = playerInput.JoystickValues[EInputAxis.ROTATE_X];
            float Rotate_Y = playerInput.JoystickValues[EInputAxis.ROTATE_Y];
            Move(Move_X, Move_Y);
            Rotate(Rotate_X, Rotate_Y);

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
                    LookAtMovingDirection(playerInput);
                }
            }
        }
    }

    
    #region INPUT
    private void Move(float axis_X, float axis_Z)
    {
        movingDirection = new Vector3(axis_X, 0.0f, axis_Z).normalized * Mathf.Max(Mathf.Abs(axis_X), Mathf.Abs(axis_Z));

        Vector3 movemetVelocity = movingDirection * currentSpeed * 0.1f;
        transform.position += (movemetVelocity + pushVelocity + GravityVelocity) * Time.deltaTime;
    }

    private void Rotate(float axis_X, float axis_Z)
    {
        DebugManager.Instance.Log(4, " PLAYER ROTATE ");
        if (axis_X != 0.0f || axis_Z != 0.0f)
        {
            Vector3 lookDirection = new Vector3(axis_X, 0.0f, -axis_Z).normalized;
            Vector3 lookAtFictifPosition = transform.position + lookDirection;
            transform.LookAt(lookAtFictifPosition);
        }
    }



    public void StartChargingSpell(ISpell spell, ESpellSlot spellSlot)
    {
        if (spell.MovementType == ESpellMovementType.LINEAR_HIT)
        {
            IsPlayerCharging = true;
            
            Debug.Log("Player started Charging");
            StartNewCoroutine(SpellChargingEnumerator, SpellChargingCoroutine(spellSlot));
            //StartCoroutine(SpellChargingCoroutine(spellSlot));
        }
        else if (spell.MovementType == ESpellMovementType.LINEAR_LASER)
        {
            StartCoroutine(SlowDownPlayerCoroutine(speed / 2.0f, spell.CastingDuration));
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
            GameObject wandEffect = Instantiate(chargingWandEnergy, position, chargingBodyEnergy.transform.rotation);
            GameObject bodyEffect = Instantiate(chargingBodyEnergy, transform.position, chargingBodyEnergy.transform.rotation);
            if (MotherOfManagers.Instance.IsARGame == true)
            {
                wandEffect.transform.localScale *= ARManager.Instance.SizeFactor;
                bodyEffect.transform.localScale *= ARManager.Instance.SizeFactor;
            }

            bodyEffect.transform.parent = this.transform;
            wandEffect.transform.parent = this.transform;
            ParticleSystem particleSystemWandEffect = wandEffect.GetComponent<ParticleSystem>();
            ParticleSystem particleSystemBodyEffect = bodyEffect.GetComponent<ParticleSystem>();

            Debug.Log("34563463463 spellCharging function working...");
            while (IsPlayerCharging)
            {
                particleSystemBodyEffect.maxParticles = counter;
                particleSystemWandEffect.maxParticles = counter;


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
        if (MotherOfManagers.Instance.IsARGame == true)
        {
            pushVelocity *= ARManager.Instance.SizeFactor;
        }

        Debug.Log("§$%§$% Player pushed : " + velocity);

        if (duration <= 0.0f)
        {
            duration = 0.1f;
        }
        StartNewCoroutine(UpdatePushVelocityEnumerator, UpdatePushVelocityCoroutine(duration));
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
            if (MotherOfManagers.Instance.IsARGame == true)
            {
                GravityVelocity *= ARManager.Instance.SizeFactor;
            }
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

    private void LookAtMovingDirection(ControllerInput playerInput)
    {
        float axis_X = playerInput.JoystickValues[EInputAxis.MOVE_X];
        float axis_Z = playerInput.JoystickValues[EInputAxis.MOVE_Y];
        //MaleficusUtilities.TransformAxisToCamera(ref axis_X, ref axis_Z, Camera.main.transform.forward);
        transform.LookAt(transform.position + new Vector3(axis_X, 0.0f, axis_Z));
    }

    private void StartNewCoroutine(IEnumerator enumerator, IEnumerator coroutine)
    {
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
        }
        enumerator = coroutine;
        StartCoroutine(enumerator);
    }
}
