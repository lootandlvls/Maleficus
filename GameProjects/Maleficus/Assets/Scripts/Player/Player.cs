using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer                                                // TODO [Nassim]: Clean this class
{
    public EPlayerID PlayerID { get; set; }
    public ETeamID TeamID { get; set; }
    public Vector3 Position { get { return transform.position; } }
    public Quaternion Rotation { get { return transform.rotation; } }
    public bool IsARPlayer { get; set; }
    public bool IsDead { get { return false; } }                                            // TODO: Define when player is dead


    public String playerVerticalInput;
    public String playerHorizontalInput;

    public Transform SpellInitPosition;
    public Transform SpellEndPosition;

    public float angularSpeed;
    public float speed;

    private float lastTimeSinceRotated;

  
    [Header("Charging Spell Effects")] 
    [SerializeField] private GameObject chargingBodyEnergy;
    [SerializeField] private GameObject chargingWandEnergy;

                                                                                            // TODO [Bnjmo + Nassim]: Refactor to ESpellID
    //Spell Manager will initialize the variables for this Dictionary
    Dictionary<int, GameObject> ChargingEffects = new Dictionary<int, GameObject>();

    
    private Vector3 movingDirection;
    private Rigidbody myRigidBody;
    private DirectionalSprite[] myDirectionalSprites;
    private Dictionary<int, AbstractSpell> spellsSlot;
    public Animator animator;

    public bool readyToShoot = true;

    public bool playerCharging = false;

                                                                        // TODO [Nassim]: refactor multiple variables into a dictionary
    public bool readyToUseSpell_1 = true;
    public bool readyToUseSpell_2 = true;
    public bool readyToUseSpell_3 = true;
    [Header("SpellsCooldown")]
    public float spellCooldown_1;
    public float spellCooldown_2;
    public float spellCooldown_3;
    [Header("SpellsDuration")]
    public float spellDuration_1;
    public float spellDuration_2;
    public float spellDuration_3;

    private void Awake()
    {
        myDirectionalSprites = GetComponentsInChildren<DirectionalSprite>();
    }

    private void Start()
    {
        /*spellsSlot = new Dictionary<int, AbstractSpell>();
        spellsSlot[1] = spellSlot_1;
        spellsSlot[2] = spellSlot_2;
        spellsSlot[3] = spellSlot_3;*/
       
        InitializeSpellsCooldownsAndDurations();


        myRigidBody = this.GetComponent<Rigidbody>();      
        animator = this.GetComponent<Animator>();
        animator.SetBool("idle", true);

        if (IsARPlayer == true)
        {
            speed *= ARManager.Instance.SizeFactor;
        }

        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;
    }

    private void Update()
    {
        if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {
            PlayerInput playerInput = PlayerManager.Instance.GetPlayerInput(PlayerID);
            if (playerInput.HasMoved() == true)
            // Moving?
            {
                Move(playerInput.Move_X, playerInput.Move_Y);

                animator.SetBool("idle", false);
            }
            // Not moving?
            else
            {
                animator.SetBool("idle", true);

            }

            if (playerInput.HasRotated() == true)
            // Rotating?
            {
                Rotate(playerInput.Rotate_X, playerInput.Rotate_Y);

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
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;

    }

    



    private void On_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {
        if (hitInfo.HitPlayerID == PlayerID)
        {
            Rigidbody rgb = GetComponent<Rigidbody>();
            rgb.AddForceAtPosition(hitInfo.HitVelocity, transform.position, ForceMode.Impulse);

        }
    }

    #region INPUT

    private void Move(float axis_X, float axis_Z)
    {
        if (IsARPlayer == true)
        {
            TransformAxisToCamera(ref axis_X, ref axis_Z);
        }
   
        movingDirection = new Vector3(axis_X, 0.0f, axis_Z).normalized * Mathf.Max(Mathf.Abs(axis_X), Mathf.Abs(axis_Z));
        transform.position += movingDirection * speed * 0.1f * Time.deltaTime;
    }

    private void Rotate(float axis_X, float axis_Z)
    {
        DebugManager.Instance.Log(4, " PLAYER ROTATE ");
        if ((axis_X != 0.0f || axis_Z != 0.0f) && (Mathf.Abs(axis_X) + Mathf.Abs(axis_Z) > MaleficusConsts.ROTATION_THRESHOLD))
        {
            if (IsARPlayer == true)
            {
                TransformAxisToCamera(ref axis_X, ref axis_Z, -1);
            }

            Vector3 lookDirection = new Vector3(axis_X, 0.0f, -axis_Z).normalized;
            Vector3 lookAtFictifPosition = transform.position + lookDirection;
            transform.LookAt(lookAtFictifPosition);
        }
    }

    private void LookAtMovingDirection(PlayerInput playerInput)
    {
        float axis_X = playerInput.Move_X;
        float axis_Z = playerInput.Move_Y;
        TransformAxisToCamera(ref axis_X, ref axis_Z);
        transform.LookAt(transform.position + new Vector3(axis_X, 0.0f, axis_Z));
    }


    public void StartChargingSpell_1(MovementType movementType)
    {
        if (readyToUseSpell_1 && readyToShoot)
        {
            if (movementType == MovementType.LINEAR_LASER)
            {
                StartCoroutine(PlayerCantMove());
            }
            //   StartChargingSpell(1, movementType);
            //     StartCoroutine(ReadyToUseSpell(spellDuration_1, 0));
            // StartCoroutine(ReadyToUseSpell(spellCooldown_1, 1));
        }
        else
        {
            return;
        }

    }
    public void StartChargingSpell_2(MovementType movementType)
    {

        if (readyToUseSpell_2 && readyToShoot)
        {
            if (movementType == MovementType.LINEAR_LASER)
            {
                StartCoroutine(PlayerCantMove());
            }
            //    StartChargingSpell(2, movementType);
            // StartCoroutine(ReadyToUseSpell(spellDuration_2, 0));
            //StartCoroutine(ReadyToUseSpell(spellCooldown_2, 2));
        }
        else
        {
            return;
        }
    }
    public void StartChargingSpell_3(MovementType movementType)
    {
        if (readyToUseSpell_3 && readyToShoot)
        {
            if (movementType == MovementType.LINEAR_LASER)
            {
                StartCoroutine(PlayerCantMove());
            }
            //  StartChargingSpell(3, movementType);
            //   StartCoroutine(ReadyToUseSpell(spellDuration_3, 0));
            //   StartCoroutine(ReadyToUseSpell(spellCooldown_3, 3));
        }
        else
        {
            return;
        }
    }
  
    
    


    public void StartChargingSpell(int spellSlot, MovementType movementType)
    {

        if (movementType == MovementType.LINEAR_HIT)
        {
            playerCharging = true;
            Debug.Log("Player started Charging");
            StartCoroutine(spellCharging(spellSlot));
        }
        else if (movementType == MovementType.LINEAR_LASER)
        {
            StartCoroutine(PlayerCantMove());
        }
        // TODO: Not working here
        // Deactivate Directional Sprite
    }

    public void StopChargingSpell_1()
    {
        Debug.Log("player stopped charging spell 1");
        playerCharging = false;

    }
    public void StopChargingSpell_2()
    {
        Debug.Log("player stopped charging spell 2 ");

    }
    public void StopChargingSpell_3()
    {
        Debug.Log("player stopped charging spell 3");

    }


    private void InitializeChargingEffects()
    {

    }
    private void InitializeSpellsCooldownsAndDurations()
    {

        //change to SpellManager.Instance.Player_Spells[playerID][1]
        spellCooldown_1 = SpellManager.Instance.Player_Spells[PlayerID][0].cooldown;
        spellCooldown_2 = SpellManager.Instance.Player_Spells[PlayerID][1].cooldown;
        spellCooldown_3 = SpellManager.Instance.Player_Spells[PlayerID][2].cooldown;
        spellDuration_1 = SpellManager.Instance.Player_Spells[PlayerID][0].spellDuration;
        spellDuration_2 = SpellManager.Instance.Player_Spells[PlayerID][1].spellDuration;
        spellDuration_3 = SpellManager.Instance.Player_Spells[PlayerID][2].spellDuration;
    }
        IEnumerator PlayerCantMove()
    {
        animator.SetBool("channeling", true);
        speed = 0;
        yield return new WaitForSeconds(2.5f);
        speed = 75;
        animator.SetBool("channeling", false);
    }

    IEnumerator SetReadyToUseSpell(float time, int id)                     // TODO [Nassim]: private before IEnumerator
    {
        switch (id)
        {
            case 0:
                yield return new WaitForSeconds(time);
                readyToShoot = true;

                break;
            case 1:
                yield return new WaitForSeconds(time);
                readyToUseSpell_1 = true;
                            
                break;

            case 2:
                yield return new WaitForSeconds(time);
                readyToUseSpell_2 = true;              
               
                break;

            case 3:
                yield return new WaitForSeconds(time);
                readyToUseSpell_3 = true;
                break;

        }


        Debug.Log("ready to use the spell again");
    }
    IEnumerator animationDelay(AbstractSpell spellToCast , int animationID)
    {
        switch (animationID)
        {
            case 1:
                animator.SetTrigger("projectileAttack");
                break;
            case 2:
                animator.SetTrigger("teleport");
                break;
        }
        
        yield return new WaitForSeconds(0.3f);
        AbstractSpell spell = Instantiate(spellToCast, SpellInitPosition.position, transform.rotation);
        spell.CastingPlayerID = PlayerID;
        spell.parabolicSpell_EndPosition = SpellEndPosition;
    }

    IEnumerator  spellCharging(int spellSlot)
    {


        int counter = 0;
        int spellLVL;
        // Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 90, 1);
        Vector3 position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        GameObject wandEffect = Instantiate(chargingWandEnergy, position, chargingBodyEnergy.transform.rotation);
        GameObject bodyEffect = Instantiate(chargingBodyEnergy, transform.position, chargingBodyEnergy.transform.rotation);
        bodyEffect.transform.parent = this.transform;
        wandEffect.transform.parent = this.transform;
        ParticleSystem particleSystemWandEffect = wandEffect.GetComponent<ParticleSystem>();
        ParticleSystem particleSystemBodyEffect = bodyEffect.GetComponent<ParticleSystem>();
       
        
        speed = 50;
        Debug.Log("spellCharging function working...");
        while (playerCharging)
        {

           
            if (counter == 10 )
            {
               
                animator.SetBool("charging", true); 
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
                    
        }
        Debug.Log("spellCharging function Done!!");


        speed = 75;
        animator.SetBool("charging", false);
        Destroy(wandEffect);
        Destroy(particleSystemBodyEffect);
        Debug.Log("counter = " + counter);
        if (counter > 100)
        {
            spellLVL = 2;
        }
        else
        {
            spellLVL = 1;
        }
     /*   AbstractSpell spellUpgrade = spellToCast;
        foreach (AbstractSpell spell in SpellManager.Instance.SpellsUpgrade)
        {

            if (spell.SpellName.Equals(spellToCast.SpellName + spellLVL))
            {
                Debug.Log(spellToCast.SpellName + counter + "has been chosen");
                spellUpgrade = spell;
            }
        }*/
       // animator.SetTrigger("projectileAttack");
       // StartCoroutine(animationDelay(spellUpgrade, 1));

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

}
