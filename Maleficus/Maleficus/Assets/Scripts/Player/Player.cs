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


    public String playerVerticalInput;
    public String playerHorizontalInput;

    public Transform SpellInitPosition;
    public Transform SpellEndPosition;

    public float angularSpeed;
    public float speed;

    [SerializeField] private AbstractSpell spellSlot_1;
    [SerializeField] private AbstractSpell spellSlot_2;
    [SerializeField] private AbstractSpell spellSlot_3;
    [Header("Charging Spell Effects")] 
    [SerializeField] private GameObject chargingBodyEnergy;
    [SerializeField] private GameObject chargingWandEnergy;

    //Spell Manager will initialize the variables for this Dictionary
    Dictionary<int, GameObject> ChargingEffects = new Dictionary<int, GameObject>();

    
    private Vector3 movingDirection;
    private Rigidbody myRigidBody;
    private DirectionalSprite myDirectionalSprite;
    private Dictionary<int, AbstractSpell> spellsSlot;
    public Animator animator;

    public bool readyToShoot = true;

    public bool playerCharging = false;

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
        myDirectionalSprite = GetComponentInChildren<DirectionalSprite>();
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

        if (IsARPlayer == true)
        {
            speed *= transform.parent.localScale.x;
        }

       EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;
    }
    private void Update()
    {
        PlayerInput playerInput = PlayerManager.Instance.GetPlayerInput(PlayerID);

        animator.SetFloat("run", playerInput.Move_X + playerInput.Move_Y);
        if (Math.Abs(playerInput.Move_X) + Math.Abs(playerInput.Move_Y) == 0)
        {
            animator.SetBool("idle", true);
        }
        else
        {
            animator.SetBool("idle", false);
        }

       


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

    // TODO : finish implementing it (input working now)
    public void Move(float axis_X, float axis_Z)
    {
  
        if (IsARPlayer == true)
        {
            Vector2 coordinateForward = new Vector2(0.0f, 1.0f);
            Vector2 coordinateRight = new Vector2(1.0f, 0.0f);
            Vector2 cameraForward = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z).normalized;
            Vector2 controllerAxis = new Vector2(axis_X, axis_Z).normalized;
            Vector2 thibautVector = (controllerAxis + cameraForward).normalized;
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
            float angle = Mathf.Acos(Vector2.Dot(coordinateForward, cameraForward)) * sign;
            DebugManager.Instance.Log(68, "X : " + controllerAxis.x + " | Y : " + controllerAxis.y + " | A : " + angle * Mathf.Rad2Deg);

            axis_X = controllerAxis.x * Mathf.Cos(angle) - controllerAxis.y * Mathf.Sin(angle);
            axis_Z = controllerAxis.y * Mathf.Cos(angle) + controllerAxis.x * Mathf.Sin(angle);
            controllerAxis = new Vector2(axis_X, axis_Z).normalized;

            axis_X = controllerAxis.x;
            axis_Z = controllerAxis.y;

            DebugManager.Instance.Log(69, "X : " + axis_X + " | Y : " + axis_Z + " | A : " + angle * Mathf.Rad2Deg);

        }
       
       
        movingDirection = new Vector3(axis_X, 0.0f, axis_Z).normalized * Mathf.Max(Mathf.Abs(axis_X), Mathf.Abs(axis_Z));
        transform.position += movingDirection * speed * 0.1f * Time.deltaTime;
    }

    public void Rotate(float axis_X, float axis_Z)
    {
        DebugManager.Instance.Log(4, " PLAYER ROTATE ");
        if ((axis_X != 0.0f || axis_Z != 0.0f) && (Mathf.Abs(axis_X) + Mathf.Abs(axis_Z) > MaleficusTypes.ROTATION_THRESHOLD))
        {
            Vector3 CurrentRotationVector = transform.rotation.eulerAngles;
            Quaternion CurrentRotation = Quaternion.Euler(CurrentRotationVector);
            Vector3 targetRotationVector = new Vector3(CurrentRotationVector.x, Mathf.Atan2(axis_X, -axis_Z) * Mathf.Rad2Deg , CurrentRotationVector.z);
            Quaternion targetRotation = Quaternion.Euler(targetRotationVector);
            DebugManager.Instance.Log(3, "CurrrentRotation : " + CurrentRotation.y + " TargerRotation : " + targetRotation.y);

            //  transform.rotation = targetRotation;
            transform.rotation = Quaternion.Lerp(CurrentRotation, targetRotation, Time.deltaTime * angularSpeed);

            // Update sprite
            if ((Mathf.Abs(axis_X) > MaleficusTypes.SPELL_BUTTON_THRESHOLD) || (Mathf.Abs(axis_Z) > MaleficusTypes.SPELL_BUTTON_THRESHOLD))
            {
                myDirectionalSprite.ShowSprite();
            }
            else
            {
                myDirectionalSprite.HideSprite();
            }
        }
    }

    /* public void CastSpell_1()
    {
       

        if (readyToUseSpell_1 && readyToShoot)
        {

            
            readyToShoot = false;
            readyToUseSpell_1 = false;
            CastSpell(spellsSlot[1]);
            StartCoroutine(ReadyToUseSpell(spellsSlot[1].spellDuration, 0));
            StartCoroutine(ReadyToUseSpell(spellsSlot[1].cooldown, 1));
        }
        else
        {
            return;
        }
      
    }

    public void CastSpell_2()
    {
        if (readyToUseSpell_2 && readyToShoot)
        {
           
            readyToShoot = false;
            readyToUseSpell_2 = false;
            CastSpell(spellsSlot[2]);
            StartCoroutine(ReadyToUseSpell(spellsSlot[2].spellDuration, 0));
            StartCoroutine(ReadyToUseSpell(spellsSlot[2].cooldown, 2));
        }
        else
        {
            return;
        }
       
    }

    public void CastSpell_3()
    {
        if (readyToUseSpell_3 && readyToShoot)
        {
            
            readyToShoot = false;
            readyToUseSpell_3 = false;
            CastSpell(spellsSlot[3]);
            StartCoroutine(ReadyToUseSpell(spellsSlot[3].spellDuration, 0));
            StartCoroutine(ReadyToUseSpell(spellsSlot[3].cooldown, 3));
        }
        else
        {
            return;
        }
        
    }*/

    //private void CastSpell(int SpellSlot , MovementType movementType)
    //{
    //    if (movementType == MovementType.AOE)
    //    //if (spellToCast.MovementType == MovementType.AOE)
    //    {
    //        animator.SetTrigger("shockwave");
    //        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
    //        AbstractSpell spell = Instantiate(spellToCast, pos, transform.rotation);
    //        spell.CastingPlayerID = PlayerID;
    //        spell.transform.parent = this.transform;
    //        Debug.Log("AOE SPELL CASTED");
    //        //   spell.parabolicSpell_EndPosition = SpellEndPosition;
    //    }
    //    else if (movementType == MovementType.LINEAR_INSTANT)
    //    //else if (spellToCast.MovementType == MovementType.LINEAR_INSTANT)
    //    {

    //        Quaternion rotation = new Quaternion(transform.rotation.x, transform.position.y, transform.rotation.z, 1);
    //    /  AbstractSpell spell = Instantiate(spellToCast, SpellInitPosition.position, rotation);
    //        spell.transform.rotation = this.transform.rotation;
    //        spell.transform.parent = this.transform;
    //        spell.CastingPlayerID = PlayerID;
    //        Debug.Log("LINEAR INSTANT SPELL CASTED");
    //    }
    //    else if (movementType == MovementType.TELEPORT)
    //    {
    //        animator.SetTrigger("teleport");
    //        StartCoroutine(animationDelay(spellToCast, 2));
    //        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
    //         AbstractSpell spell = Instantiate(spellToCast, pos, transform.rotation);
    //         spell.CastingPlayerID = PlayerID;
    //    }
    //    else if (movementType == MovementType.LINEAR_LASER)
    //    {
    //        animator.SetBool("channeling", true);
    //        Quaternion rotation = new Quaternion(transform.rotation.x, transform.position.y, transform.rotation.z, 1);
    //      /*  AbstractSpell spell = Instantiate(spellToCast, SpellInitPosition.position, rotation);
    //        spell.transform.rotation = this.transform.rotation;
    //        spell.transform.parent = this.transform;
    //        spell.CastingPlayerID = PlayerID;*/
    //        StartCoroutine(PlayerCantMove());
    //    }
    //    else
    //    {
    //        StartCoroutine(spellCharging(spellToCast));
    //         /* int counter = 1;
    //          while (counter < 4)
    //           {
    //               if (Input.GetButton("CastSpell_1_A"))
    //               {
    //                  StartCoroutine(spellCharging());
    //                  counter++;
    //               }
    //               else
    //               {
    //                   return;
    //               }


    //         /*  AbstractSpell spell = Instantiate(spellToCast, SpellInitPosition.position, transform.rotation);
    //            spell.CastingPlayerID = PlayerID;
    //            spell.parabolicSpell_EndPosition = SpellEndPosition;
    //        }
            
    //         AbstractSpell spellUpgrade = spellToCast;
    //         foreach(AbstractSpell spell in SpellManager.Instance.spellsUpgrade)
    //        {
                
    //            if (spell.SpellName.Equals(spellToCast.SpellName + counter))
    //            {
    //                Debug.Log(spellToCast.SpellName + counter + "has been chosen");
    //                spellUpgrade = spell;
    //            }
    //        }
    //        animator.SetTrigger("projectileAttack");
    //        StartCoroutine(animationDelay(spellUpgrade, 1));*/
    //    }
        
    //                                                                    // TODO: Not working here
    //    // Deactivate Directional Sprite
    //    myDirectionalSprite.HideSprite();
    //}


    public void StartChargingSpell_1(MovementType movementType)
    {
        if (readyToUseSpell_1 && readyToShoot)
        {
           
            StartChargingSpell(1, movementType);
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
          
            StartChargingSpell(2, movementType);
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
          
            StartChargingSpell(3, movementType);
         //   StartCoroutine(ReadyToUseSpell(spellDuration_3, 0));
         //   StartCoroutine(ReadyToUseSpell(spellCooldown_3, 3));
        }
        else
        {
            return;
        }
    }
  
    
    public void StopChargingSpell_1()
    {
        Debug.Log("player stopped charging spell 1");
       
    }
    public void StopChargingSpell_2()
    {
        Debug.Log("player stopped charging spell 2 ");
        
    }
    public void StopChargingSpell_3()
    {
        Debug.Log("player stopped charging spell 3");
     
    }


    public void StartChargingSpell(int spellSlot, MovementType movementType)
    {
       
         if (movementType == MovementType.LINEAR_HIT)
          {
             // spellCharging(spellSlot);
          }
          else if (movementType == MovementType.LINEAR_LASER)
          {
              StartCoroutine(PlayerCantMove());
          }
          // TODO: Not working here
          // Deactivate Directional Sprite
          myDirectionalSprite.HideSprite();
        Debug.Log("spell started charging");
    }
    /// Set the spells chosen  by the player
    public void SetSpells(AbstractSpell spell_1, AbstractSpell spell_2, AbstractSpell spell_3)
    {
        spellSlot_1 = spell_1;
        spellSlot_2 = spell_2;
        spellSlot_3 = spell_3;

        spellsSlot = new Dictionary<int, AbstractSpell>();

        spellsSlot[1] = spell_1;
        spellsSlot[2] = spell_2;
        spellsSlot[3] = spell_3;
       
    }


   private void InitializeChargingEffects()
    {

    }
    private void InitializeSpellsCooldownsAndDurations()
    {

        //change to SpellManager.Instance.Player_Spells[playerID][1]
        spellCooldown_1 = SpellManager.Instance.Player_1_Spells[0].cooldown;
        spellCooldown_2 = SpellManager.Instance.Player_1_Spells[1].cooldown;
        spellCooldown_3 = SpellManager.Instance.Player_1_Spells[2].cooldown;
        spellDuration_1 = SpellManager.Instance.Player_1_Spells[0].spellDuration;
        spellDuration_2 = SpellManager.Instance.Player_1_Spells[1].spellDuration;
        spellDuration_3 = SpellManager.Instance.Player_1_Spells[2].spellDuration;
    }
        IEnumerator PlayerCantMove()
    {
        animator.SetBool("channeling", true);
        speed = 0;
        yield return new WaitForSeconds(2.5f);
        speed = 75;
        animator.SetBool("channeling", false);
    }

    IEnumerator ReadyToUseSpell(float time, int id)
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

    private void spellCharging(int spellSlot)
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

        while (Input.GetButton("CastSpell_1_A"))
        {
            if (counter == 10 )
            {
                animator.SetBool("charging", true);
                particleSystemBodyEffect.maxParticles = 1 ;
                particleSystemWandEffect.maxParticles = 1;
            }
            if (  counter  == 50 || counter == 100)
            {
                particleSystemBodyEffect.maxParticles = counter;
                particleSystemWandEffect.maxParticles = counter;
            }
           
              
                counter++;
                    
        }
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
        animator.SetTrigger("projectileAttack");
       // StartCoroutine(animationDelay(spellUpgrade, 1));

    }
    #endregion





}
