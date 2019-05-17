using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    public PlayerID PlayerID { get { return myPlayerID; } }
    public bool IsConnected { get { return isConnected; } }

    private const float DIRECTIONAL_BUTTON_THRESHOLD = 0.5f;
    private const float ROTATION_THRESHOLD = 0f;

    [SerializeField] float speed ;
    [SerializeField] float angularSpeed;

    private Rigidbody rigidBody;

    private bool isConnected;

    private PlayerID myPlayerID = PlayerID.NONE;

      Dictionary<int, AbstractSpell> spellsSlot;

    [SerializeField] private AbstractSpell spellSlot_1;
    [SerializeField] private AbstractSpell spellSlot_2;
    [SerializeField] private AbstractSpell spellSlot_3;




    private void Start()
    {
        spellsSlot = new Dictionary<int, AbstractSpell>();
        spellsSlot[1] = spellSlot_1;
        spellsSlot[2] = spellSlot_2;
        spellsSlot[3] = spellSlot_3;
        rigidBody = this.GetComponent<Rigidbody>();
    }


    private void Update()
    {
        //float x = Input.GetAxis("Horizontal_L_A");
        //float z = Input.GetAxis("Vertical_L_A");

        //if (myPlayerID == PlayerID.PLAYER_1)
        //    DebugManager.Instance.Log(5, "Player " + PlayerID + " | X : " + x + " | Z : " + z);

        //Vector3 movingDirection = new Vector3(x, 0.0f, -z).normalized * Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
        //transform.Translate(movingDirection * speed * Time.deltaTime);
    }

    #region INPUT
    public void Connect(PlayerID playerID)
    {
        myPlayerID = playerID;
        isConnected = true;
        GetComponent<MeshRenderer>().material.color = Color.blue;
        
    }
    // TODO : finish implementing it when the inputs are working properly
    public void Move(float axis_X, float axis_Z)
    {

        if (Mathf.Abs(axis_X) >= DIRECTIONAL_BUTTON_THRESHOLD || Mathf.Abs(axis_Z) >= DIRECTIONAL_BUTTON_THRESHOLD)
        {
            Vector3 movingDirection = new Vector3(axis_X * speed * Time.deltaTime, 0, -axis_Z * speed * Time.deltaTime);
            // Vector3 faceDirection = transform.TransformDirection(Vector3.forward);
            rigidBody.velocity = new Vector3(movingDirection.x, rigidBody.velocity.y, movingDirection.z);
            DebugManager.Instance.Log(4, "MovingDirection.X: " + movingDirection.x + "/ MovingDirection.Y: " + movingDirection.y + " / MovingDirection.Z: " + movingDirection.z);
        }
        // transform.Rotate(new Vector3(-1.0f * axis_Y, -1.0f * axis_X, 0.0f));


        //Vector3 movingDirection = new Vector3(axis_X, 0.0f, -axis_Z).normalized * Mathf.Max(Mathf.Abs(axis_X), Mathf.Abs(axis_Z));
        //transform.Translate(movingDirection * speed * Time.deltaTime);
    }

    public void Rotate(float axis_X, float axis_Z)
    {
        Debug.Log(axis_X + " : " + axis_Z);
          DebugManager.Instance.Log(4, " PLAYER ROTATE ");
        if ((axis_X != 0.0f || axis_Z != 0.0f) && (Mathf.Abs(axis_X) + Mathf.Abs(axis_Z) > ROTATION_THRESHOLD))
        {
            Vector3 CurrentRotationVector = transform.rotation.eulerAngles;
            Quaternion CurrentRotation = Quaternion.Euler(CurrentRotationVector);
            Vector3 targetRotationVector = new Vector3(CurrentRotationVector.x, Mathf.Atan2(axis_X, -axis_Z) * Mathf.Rad2Deg , CurrentRotationVector.z);
           Quaternion targetRotation = Quaternion.Euler(targetRotationVector);
             DebugManager.Instance.Log(3, "CurrrentRotation : " + CurrentRotation.y + " TargerRotation : " + targetRotation.y);

          //  transform.rotation = targetRotation;
          transform.rotation = Quaternion.Lerp(CurrentRotation, targetRotation, Time.deltaTime * angularSpeed);
          
            
        }
    }

    public void CastSpell_1()
    {
       // StartCoroutine(SpellTestCoroutine());
       Instantiate(spellsSlot[1],transform.position, transform.rotation);
        
    }

    public void CastSpell_2()
    {
        Instantiate(spellsSlot[2], transform.position, transform.rotation);
      
    }

    public void CastSpell_3()
    {
        Instantiate(spellsSlot[3], transform.position, transform.rotation);
       
    }


    //set the spells chosen  by the player
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
        GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
    #endregion



}
