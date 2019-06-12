using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    public EPlayerID PlayerID { get; set; }


      public float speed ;
    [SerializeField] float angularSpeed;

    [SerializeField] private AbstractSpell spellSlot_1;
    [SerializeField] private AbstractSpell spellSlot_2;
    [SerializeField] private AbstractSpell spellSlot_3;

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
        spellsSlot = new Dictionary<int, AbstractSpell>();
        spellsSlot[1] = spellSlot_1;
        spellsSlot[2] = spellSlot_2;
        spellsSlot[3] = spellSlot_3;
        myRigidBody = this.GetComponent<Rigidbody>();
        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;

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
        CastSpell(spellsSlot[1]);
    }

    public void CastSpell_2()
    {
        CastSpell(spellsSlot[2]);
    }

    public void CastSpell_3()
    {
        CastSpell(spellsSlot[3]);
    }

    private void CastSpell(AbstractSpell spellToCast)
    {
        Vector3 startPosition = new Vector3(transform.position.x + 1, transform.position.y + 0.5f, transform.position.z + 1);
        AbstractSpell spell = Instantiate(spellToCast, startPosition, transform.rotation);
        spell.CastingPlayerID = PlayerID;
       
        
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
    #endregion


    


}
