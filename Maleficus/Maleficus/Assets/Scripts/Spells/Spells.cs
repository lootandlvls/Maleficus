using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour
{

    public float speed = 1000;
    private Vector3 movingDirection;
    public Rigidbody rb;
    public Vector3 dirVector;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    //this function will be over written by the spells children classes
    

    void SpellAbility()
    {

    }
    public void move()
    {
        movingDirection.z = speed * Time.deltaTime;

        dirVector = transform.TransformDirection(movingDirection);
        rb.velocity = new Vector3(dirVector.x, dirVector.y, dirVector.z);
    }

}
