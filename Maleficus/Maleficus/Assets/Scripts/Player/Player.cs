using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    public EPlayerID PlayerID { get; set; }


    public Vector3 Position { get { return transform.position; } }
    public Quaternion Rotation { get { return transform.rotation; } }
  

    public bool IsARPlayer { get; set; }

    public String playerVerticalInput;
    public String playerHorizontalInput;

    [SerializeField] private Transform SpellInitPosition;
    [SerializeField] private Transform SpellEndPosition;
    public float speed ;
    [SerializeField] float angularSpeed;

    [SerializeField] private AbstractSpell spellSlot_1;
    [SerializeField] private AbstractSpell spellSlot_2;
    [SerializeField] private AbstractSpell spellSlot_3;

    private bool readyToShoot = true;

    private bool readyToUseSpell_1 = true;
    private bool readyToUseSpell_2 = true;
    private bool readyToUseSpell_3 = true;

    private float spellCooldown_1;
    private float spellCooldown_2;
    private float spellCooldown_3;

    private Vector3 movingDirection;
    private Rigidbody myRigidBody;
    private DirectionalSprite myDirectionalSprite;

    private Dictionary<int, AbstractSpell> spellsSlot;


    private void Awake()
    {
        myDirectionalSprite = GetComponentInChildren<DirectionalSprite>();
    }

    private void Start()
    {
        // this will be then late change to setSpell function
        spellsSlot = new Dictionary<int, AbstractSpell>();
        spellsSlot[1] = spellSlot_1;
        spellsSlot[2] = spellSlot_2;
        spellsSlot[3] = spellSlot_3;
        spellCooldown_1 = spellSlot_1.cooldown;
        spellCooldown_2 = spellSlot_2.cooldown;
        spellCooldown_3 = spellSlot_3.cooldown;

        myRigidBody = this.GetComponent<Rigidbody>();
        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;

        if (IsARPlayer == true)
        {
            speed *= transform.parent.localScale.x;
        }

    }


    private void On_SPELLS_SpellHitPlayer(HitInfo hitInfo)
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

        //if (Mathf.Abs(axis_X) >= DIRECTIONAL_BUTTON_THRESHOLD || Mathf.Abs(axis_Z) >= DIRECTIONAL_BUTTON_THRESHOLD)
        //{
        //    Vector3 movingDirection = new Vector3(axis_X  * speed * Time.deltaTime, 0, -axis_Z * speed * Time.deltaTime);
        //    // Vector3 faceDirection = transform.TransformDirection(Vector3.forward);
        //    rigidBody.velocity = new Vector3(movingDirection.x, rigidBody.velocity.y, movingDirection.z);
        //    DebugManager.Instance.Log(4, "MovingDirection.X: " + movingDirection.x + "/ MovingDirection.Y: " + movingDirection.y + " / MovingDirection.Z: " + movingDirection.z);
        //}
        //// transform.Rotate(new Vector3(-1.0f * axis_Y, -1.0f * axis_X, 0.0f));


       

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

    public void CastSpell_1()
    {
        Debug.Log("readyToShoot = " + readyToShoot);
        Debug.Log("readyToUseSpell_1 = " + readyToUseSpell_1);
        if (readyToUseSpell_1 && readyToShoot)
        {
            readyToShoot = false;
            readyToUseSpell_1 = false;
            CastSpell(spellsSlot[1]);
            StartCoroutine(ReadyToUseSpell(spellsSlot[1].spellDuration,  0));
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
            StartCoroutine(ReadyToUseSpell(spellsSlot[2].spellDuration,  0));
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
            StartCoroutine(ReadyToUseSpell(spellsSlot[3].cooldown,  3));
        }
         else
        {
            return;
        }
    }

    private void CastSpell(AbstractSpell spellToCast)
    {    
     
        
        if (spellToCast.GetComponent<AOE>() != null)
        
        {
           
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            AbstractSpell spell = Instantiate(spellToCast, pos, transform.rotation);
            spell.CastingPlayerID = PlayerID;
            spell.transform.parent = this.transform;
            Debug.Log("AOE SPELL CASTED");
           
           
         //   spell.parabolicSpell_EndPosition = SpellEndPosition;
        }
        else if (spellToCast.GetComponent<Linear_Instant>() != null)
       
        {
            Quaternion rotation = new Quaternion(transform.rotation.x, transform.position.y, transform.rotation.z,1);
            AbstractSpell spell = Instantiate(spellToCast, SpellInitPosition.position, rotation);
            spell.transform.rotation = this.transform.rotation;
            spell.transform.parent = this.transform;
            spell.CastingPlayerID = PlayerID;
            Debug.Log("LINEAR INSTANT SPELL CASTED");
           

        }
        else if (spellToCast.GetComponent<Teleport>() != null)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            AbstractSpell spell = Instantiate(spellToCast, pos, transform.rotation);
            spell.CastingPlayerID = PlayerID;
          
        }
        else if (spellToCast.GetComponent<Linear_Laser>() != null)
        {
          
            Quaternion rotation = new Quaternion(transform.rotation.x, transform.position.y, transform.rotation.z, 1);
            AbstractSpell spell = Instantiate(spellToCast, SpellInitPosition.position, rotation);
            spell.transform.rotation = this.transform.rotation;
            spell.transform.parent = this.transform;
            spell.CastingPlayerID = PlayerID;
            StartCoroutine(PlayerCantMove());
          
        }
        else
        {
           
            AbstractSpell spell = Instantiate(spellToCast, SpellInitPosition.position, transform.rotation);
            spell.CastingPlayerID = PlayerID;
            spell.parabolicSpell_EndPosition = SpellEndPosition;
           
        }
        
                                                                        // TODO: Not working here
        // Deactivate Directional Sprite
        myDirectionalSprite.HideSprite();       
       
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



    private IEnumerator SpellTestCoroutine()
    {
        GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
    }

    IEnumerator PlayerCantMove()
    {
        speed = 0;
        yield return new WaitForSeconds(2.5f);
        speed = 75;
    }
    //Spell Cooldowns
    IEnumerator ReadyToUseSpell(float time , int id)
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
  
    #endregion





}
