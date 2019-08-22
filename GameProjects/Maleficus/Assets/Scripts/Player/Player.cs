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
    public Dictionary<ESpellID, bool> ReadyToUseSpell { get { return readyToUseSpell; } }
    public Dictionary<ESpellID, float> SpellCooldown { get { return spellCooldown; } }
    public Dictionary<ESpellID, float> SpellDuration { get { return spellDuration; } }
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
    private IEnumerator PushCoroutine;


    /// <summary> Spell Manager will initialize the variables for this Dictionary </summary> 
    Dictionary<ESpellID, GameObject> ChargingEffects = new Dictionary<ESpellID, GameObject>();

    
    private Vector3 movingDirection;
    //private Rigidbody myRigidBody;
    private DirectionalSprite[] myDirectionalSprites;
    private Dictionary<int, AbstractSpell> spellsSlot;
    private Animator myAnimator;

    private Vector3 pushVelocity;
    private Vector3 GravityVelocity;

    private bool isDead = false;


    // TODO [Nassim]: refactor multiple variables into a dictionary
    private Dictionary<ESpellID, bool> readyToUseSpell = new Dictionary<ESpellID, bool>();

    [Header("SpellsCooldown")]
    private Dictionary<ESpellID, float> spellCooldown = new Dictionary<ESpellID, float>();

    [Header("SpellsDuration")]
    private Dictionary<ESpellID, float> spellDuration = new Dictionary<ESpellID, float>();


    private void Awake()
    {
        myDirectionalSprites = GetComponentsInChildren<DirectionalSprite>();

        currentSpeed = speed;

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
                    && (Time.time - lastTimeSinceRotated > 1.0f))
                // Moving for 1 second since last rortation?
                {
                    LookAtMovingDirection(playerInput);
                }
            }
            
        }

        // Cance all forces
        //myRigidBody.velocity = Vector3.zero;
        //myRigidBody.angularVelocity = Vector3.zero;

    }

    



    //private void On_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    //{
    //    if (hitInfo.HitPlayerID == PlayerID)
    //    {
    //        Rigidbody rgb = GetComponent<Rigidbody>();
    //        rgb.AddForceAtPosition(hitInfo.HitVelocity, transform.position, ForceMode.Impulse);

    //    }
    //}

    #region INPUT

    private void Move(float axis_X, float axis_Z)
    {
        //if (IsARPlayer == true)
        //{
        //    MaleficusUtilities.TransformAxisToCamera(ref axis_X, ref axis_Z, Camera.main.transform.forward);
        //}
   
        movingDirection = new Vector3(axis_X, 0.0f, axis_Z).normalized * Mathf.Max(Mathf.Abs(axis_X), Mathf.Abs(axis_Z));

        Vector3 movemetVelocity = movingDirection * speed * 0.1f;
        transform.position += (movemetVelocity + pushVelocity + GravityVelocity) * Time.deltaTime;
    }

    private void Rotate(float axis_X, float axis_Z)
    {
        DebugManager.Instance.Log(4, " PLAYER ROTATE ");
        if ((axis_X != 0.0f || axis_Z != 0.0f) && (Mathf.Abs(axis_X) + Mathf.Abs(axis_Z) > MaleficusConsts.ROTATION_THRESHOLD))
        {
            //if (IsARPlayer == true)
            //{
            //    MaleficusUtilities.TransformAxisToCamera(ref axis_X, ref axis_Z, Camera.main.transform.forward , true);
            //}

            Vector3 lookDirection = new Vector3(axis_X, 0.0f, -axis_Z).normalized;
            Vector3 lookAtFictifPosition = transform.position + lookDirection;
            transform.LookAt(lookAtFictifPosition);
        }
    }

    private void LookAtMovingDirection(ControllerInput playerInput)
    {
        float axis_X = playerInput.JoystickValues[EInputAxis.MOVE_X];
        float axis_Z = playerInput.JoystickValues[EInputAxis.MOVE_Y];
        //MaleficusUtilities.TransformAxisToCamera(ref axis_X, ref axis_Z, Camera.main.transform.forward);
        transform.LookAt(transform.position + new Vector3(axis_X, 0.0f, axis_Z));
    }

    public void StartChargingSpell(ESpellID spellID, EMovementType movementType)
    {
        if (readyToUseSpell[spellID] && IsReadyToShoot)
        {
            if (movementType == EMovementType.LINEAR_HIT)
            {
                StartCoroutine(PlayerCannotMoveCoroutine());


                IsPlayerCharging = true;
                Debug.Log("Player started Charging");
                StartCoroutine(SpellChargingCoroutine(spellID));
            }
            else if (movementType == EMovementType.LINEAR_LASER)
            {
                StartCoroutine(PlayerCannotMoveCoroutine());
            }

            //   StartChargingSpell(1, movementType);
            //     StartCoroutine(ReadyToUseSpell(spellDuration_1, 0));
            // StartCoroutine(ReadyToUseSpell(spellCooldown_1, 1));


            // TODO: Not working here
            // Deactivate Directional Sprite
        }
    }



    public void StopChargingSpell(ESpellID spellID)
    {
        Debug.Log("player stopped charging " + spellID);
        IsPlayerCharging = false;

    }
 


    private void InitializeChargingEffects()
    {

    }

    private void InitializeDictionaries()
    {
        readyToUseSpell[ESpellID.SPELL_1] = true;
        readyToUseSpell[ESpellID.SPELL_2] = true;
        readyToUseSpell[ESpellID.SPELL_3] = true;

        spellCooldown[ESpellID.SPELL_1] = SpellManager.Instance.Player_Spells[PlayerID][ESpellID.SPELL_1].Cooldown;
        spellCooldown[ESpellID.SPELL_2] = SpellManager.Instance.Player_Spells[PlayerID][ESpellID.SPELL_2].Cooldown;
        spellCooldown[ESpellID.SPELL_3] = SpellManager.Instance.Player_Spells[PlayerID][ESpellID.SPELL_3].Cooldown;

        spellDuration[ESpellID.SPELL_1] = SpellManager.Instance.Player_Spells[PlayerID][ESpellID.SPELL_1].CastingDuration;
        spellDuration[ESpellID.SPELL_2] = SpellManager.Instance.Player_Spells[PlayerID][ESpellID.SPELL_2].CastingDuration;
        spellDuration[ESpellID.SPELL_3] = SpellManager.Instance.Player_Spells[PlayerID][ESpellID.SPELL_3].CastingDuration;
    }


    private IEnumerator PlayerCannotMoveCoroutine()
    {
        myAnimator.SetBool("channeling", true);
        currentSpeed = 0;

        yield return new WaitForSeconds(2.5f);

        currentSpeed = speed;
        myAnimator.SetBool("channeling", false);
    }

    private IEnumerator SetReadyToUseSpellCoroutine(float time, ESpellID spellID)                    
    {
        yield return new WaitForSeconds(time);

        IsReadyToShoot = true;
        readyToUseSpell[spellID] = true;

        Debug.Log("ready to use the spell again");
    }


    private IEnumerator AnimationDelayCoroutine(AbstractSpell spellToCast , int animationID)
    {
        switch (animationID)
        {
            case 1:
                myAnimator.SetTrigger("projectileAttack");
                break;
            case 2:
                myAnimator.SetTrigger("teleport");
                break;
        }
        
        yield return new WaitForSeconds(0.3f);
        AbstractSpell spell = Instantiate(spellToCast, SpellInitPosition, transform.rotation);
        spell.CastingPlayerID = PlayerID;
        spell.parabolicSpell_EndPosition = SpellEndPosition;
    }

    private IEnumerator SpellChargingCoroutine(ESpellID spellID)
    {
        int counter = 0;
        
        // Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 90, 1);
        Vector3 position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        GameObject wandEffect = Instantiate(chargingWandEnergy, position, chargingBodyEnergy.transform.rotation);
        GameObject bodyEffect = Instantiate(chargingBodyEnergy, transform.position, chargingBodyEnergy.transform.rotation);
        bodyEffect.transform.parent = this.transform;
        wandEffect.transform.parent = this.transform;
        ParticleSystem particleSystemWandEffect = wandEffect.GetComponent<ParticleSystem>();
        ParticleSystem particleSystemBodyEffect = bodyEffect.GetComponent<ParticleSystem>();
       
        currentSpeed = speed / 2;
        Debug.Log("spellCharging function working...");
        while (IsPlayerCharging)
        {
            if (counter == 10 )
            {
               
                myAnimator.SetBool("charging", true); 
                particleSystemBodyEffect.maxParticles = 1;                                             // TODO [Nassim]: Fix warnings
                particleSystemWandEffect.maxParticles = 1;
            }
            if (  counter  == 50 || counter == 100)
            {
                particleSystemBodyEffect.maxParticles = counter;
                particleSystemWandEffect.maxParticles = counter;
            }
            
            yield return new WaitForSeconds(0.0f);
            counter++;
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

        }
        Debug.Log("spellCharging function Done!!");
       
        currentSpeed = speed;

        myAnimator.SetBool("charging", false);
        Destroy(wandEffect);
        Destroy(particleSystemBodyEffect);
        Debug.Log("counter = " + counter);
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

    private void TransformAxisToCamera(ref float axis_X, ref float axis_Z, int inversedSign = 1)
    {
        Vector2 coordinateForward = new Vector2(0.0f, 1.0f);
        Vector2 coordinateRight = new Vector2(1.0f, 0.0f);
        Vector2 cameraForward = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z).normalized;
        Vector2 controllerAxis = new Vector2(axis_X, axis_Z).normalized;
        float dotWithRight = Vector2.Dot(coordinateRight, cameraForward);
        int sign;
        if (dotWithRight > 0.0f)
        {
            sign = -1;
        }
        else if (dotWithRight < 0.0f)
        {
            sign = 1;
        }
        else
        {
            sign = 0;
        }
        sign *= inversedSign;

        float angle = Mathf.Acos(Vector2.Dot(coordinateForward, cameraForward)) * sign;
        DebugManager.Instance.Log(68, "X : " + controllerAxis.x + " | Y : " + controllerAxis.y + " | A : " + angle * Mathf.Rad2Deg);


        axis_Z = controllerAxis.y * Mathf.Cos(angle) + controllerAxis.x * Mathf.Sin(angle);
        axis_X = controllerAxis.x * Mathf.Cos(angle) - controllerAxis.y * Mathf.Sin(angle);
        controllerAxis = new Vector2(axis_X, axis_Z).normalized;

        axis_X = controllerAxis.x;
        axis_Z = controllerAxis.y;
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

        if (PushCoroutine != null)
        {
            StopCoroutine(PushCoroutine);
        }
        if (duration <= 0.0f)
        {
            duration = 0.1f;
        }
        PushCoroutine = UpdatePushVelocityCoroutine(duration);
        StartCoroutine(PushCoroutine);
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

}
