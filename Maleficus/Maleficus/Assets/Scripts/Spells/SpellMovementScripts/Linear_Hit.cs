using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear_Hit : MonoBehaviour
{
    private Vector3 movingDirection;
    public Rigidbody myRigidBody;
    public Vector3 dirVector;
    [SerializeField] float speed;
    [SerializeField] float hitPower;
    [SerializeField] bool shoot;

    // Start is called before the first frame update
    void Start()
    {
        AbstractSpell abstractSpell = this.GetComponent<AbstractSpell>();
        speed = abstractSpell.Speed;
        myRigidBody = this.GetComponent<Rigidbody>();
    } 

    // Update is called once per frame
    void Update()
    {
        
            Move();
       
        
    }


    public void Move()
    {
          movingDirection.z = speed * Time.deltaTime;

          dirVector = transform.TransformDirection(movingDirection);
          myRigidBody.velocity = new Vector3(dirVector.x, dirVector.y, dirVector.z);
       
         shoot = false;


    }
}
