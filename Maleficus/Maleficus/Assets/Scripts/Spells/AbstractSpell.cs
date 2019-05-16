using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpell : MonoBehaviour, ISpell
{

    public float speed = 1000;
    private Vector3 movingDirection;
    public Rigidbody myRigidBody;
    public Vector3 dirVector;

    public PlayerID PlayerID => throw new System.NotImplementedException();

    public int HitPower => throw new System.NotImplementedException();

    public float Speed => throw new System.NotImplementedException();

    public Vector3 Direction => throw new System.NotImplementedException();

    public Vector3 EndDestination => throw new System.NotImplementedException();

    [SerializeField] private PlayerID playerID;


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
    private void SpellAbility()
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
        if (otherPlayer != null)
        {
            HitInfo hitInfo = new HitInfo(this, PlayerID, otherPlayer.PlayerID, transform.position);
            EventManager.Instance.Invoke_SPELLS_SpellHitPlayer(hitInfo);
        }
    }
}
