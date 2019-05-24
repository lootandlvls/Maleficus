using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpell : MonoBehaviour, ISpell
{
    private Vector3 movingDirection;
    public Rigidbody myRigidBody;
    public Vector3 dirVector;
   
    public EPlayerID CastingPlayerID  { get; set; }

    public int HitPower { get { return hitPower; }  }

    public float Speed { get { return speed; } }

    public Vector3 Direction { get; set; }

    public Vector3 EndDestination { get; set; }

    public string SpellName { get { return spellName; } }

    public int SpellLevel { get { return spellLevel; }  }

    [SerializeField] private int hitPower;
    [SerializeField] private float speed;
    [SerializeField] private string spellName;
    [SerializeField] private int spellLevel;


    // Start is called before the first frame update
    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    //this function will be over written by the spells children classes
    public void SpellAbility( )
    {
        
      

    }
    public void Move()
    {
        movingDirection.z = speed * Time.deltaTime;

        dirVector = transform.TransformDirection(movingDirection);
        myRigidBody.velocity = new Vector3(dirVector.x, dirVector.y, dirVector.z);
    }






    private void OnTriggerEnter(Collider other)
    {
        IPlayer otherPlayer = other.gameObject.GetComponent<IPlayer>();
        if ((otherPlayer != null) && (CastingPlayerID != otherPlayer.PlayerID)) 
        {
            HitInfo hitInfo = new HitInfo(this, CastingPlayerID, otherPlayer.PlayerID, transform.position);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
        }
    }
}
