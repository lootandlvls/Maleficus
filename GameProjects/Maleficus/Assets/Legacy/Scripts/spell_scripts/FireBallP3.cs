using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallP3 : MonoBehaviour
{

    public float speed;
    public float radius = 5.0F;
    public float power = 10.0F;
    private Vector3 movingDirection;
    public Rigidbody rb;
    Vector3 pushvec;
    public Vector3 dirVector;
    public GameObject explosion;
    private bool reflected = false;
    public string shooterTag;


    private bool ShieldCollision = false;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!ShieldCollision)
        {
            movingDirection.z = speed * Time.deltaTime;

            dirVector = transform.TransformDirection(movingDirection);
            rb.velocity = new Vector3(dirVector.x, dirVector.y, dirVector.z);
        }
        else
        {
            movingDirection.z = -speed * Time.deltaTime;

            dirVector = transform.TransformDirection(movingDirection);
            rb.velocity = new Vector3(dirVector.x, dirVector.y, dirVector.z);
        }
        //movingDirection.z = speed * Time.deltaTime;
        //movingDirection.x = transform.rotation.x;
        //movingDirection.y = transform.rotation.y;
        //dirVector = transform.TransformDirection(movingDirection);
        //rb.velocity = new Vector3(dirVector.x, dirVector.y, dirVector.z);
    }

    private void FixedUpdate()
    {

    }


    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.layer == 11)
        {
            Debug.Log("Hit an object from Layer PLAYER");
            // ADD && FOR EACH PLAYER TAG 
            if (other.gameObject.tag != shooterTag && other.gameObject.tag == "Player1")
            {
                PsController script = other.GetComponent<PsController>();
                if (!script.shieldUp)
                {
                    Debug.Log("HIT PLAYER 1");
                    //apply forces
                    Instantiate(explosion, transform.position, transform.rotation);
                    pushvec.z = power * Time.deltaTime;

                    Vector3 dir = transform.TransformDirection(pushvec);

                    Rigidbody rgb = other.GetComponent<Rigidbody>();

                    if (other.GetComponent<PsController>().enabled == true)
                    {
                        other.GetComponent<PsController>().enabled = false;
                        rgb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        rgb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                        Destroy(this.gameObject);
                    }
                }
                else
                {
                    Debug.Log("Hit a shield");
                    //Inverse Direction
                    pushvec.z = power * Time.deltaTime;
                    reflected = true;
                    ShieldCollision = !ShieldCollision;

                }
            }
            if (other.gameObject.tag != shooterTag && other.gameObject.tag == "Player2")
            {
                Ps2Controller script = other.GetComponent<Ps2Controller>();
                if (!script.shieldUp)
                {
                    Debug.Log("HIT PLAYER 2");
                    //apply forces
                    Instantiate(explosion, transform.position, transform.rotation);
                    pushvec.z = power * Time.deltaTime;

                    Vector3 dir = transform.TransformDirection(pushvec);

                    Rigidbody rgb = other.GetComponent<Rigidbody>();

                    if (other.GetComponent<Ps2Controller>().enabled == true)
                    {
                        other.GetComponent<Ps2Controller>().enabled = false;
                        rgb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        rgb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);

                        Destroy(this.gameObject);
                    }
                }
                else
                {
                    Debug.Log("Hit a shield");
                    //Inverse Direction
                    pushvec.z = power * Time.deltaTime;
                    reflected = true;
                    ShieldCollision = !ShieldCollision;

                }
            }
            if (reflected && other.gameObject.tag == "Player3")
            {
                Ps3Controller script = other.GetComponent<Ps3Controller>();
                if (!script.shieldUp)
                {
                    Debug.Log("HIT PLAYER 3");
                    //apply forces
                    Instantiate(explosion, transform.position, transform.rotation);
                    pushvec.z = -power * Time.deltaTime;

                    Vector3 dir = transform.TransformDirection(pushvec);

                    Rigidbody rgb = other.GetComponent<Rigidbody>();

                    if (other.GetComponent<Ps3Controller>().enabled == true)
                    {
                        other.GetComponent<Ps3Controller>().enabled = false;
                        rgb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        rgb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                        Destroy(this.gameObject);
                    }
                }
                else
                {
                    Debug.Log("Hit a shield");
                    //Inverse Direction
                    reflected = true;
                    pushvec.z = power * Time.deltaTime;
                    ShieldCollision = !ShieldCollision;

                }
            }
            if (other.gameObject.tag != shooterTag && other.gameObject.tag == "Player4")
            {
                Ps4Controller script = other.GetComponent<Ps4Controller>();
                if (!script.shieldUp)
                {
                    Debug.Log("HIT PLAYER 4");
                    //apply forces
                    Instantiate(explosion, transform.position, transform.rotation);
                    pushvec.z = power * Time.deltaTime;

                    Vector3 dir = transform.TransformDirection(pushvec);

                    Rigidbody rgb = other.GetComponent<Rigidbody>();

                    if (other.GetComponent<Ps4Controller>().enabled == true)
                    {
                        other.GetComponent<Ps4Controller>().enabled = false;
                        rgb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        rgb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                        Destroy(this.gameObject);
                    }
                }
                else
                {
                    Debug.Log("Hit a shield");
                    //Inverse Direction
                    pushvec.z = power * Time.deltaTime;
                    reflected = true;
                    ShieldCollision = !ShieldCollision;

                }
            }
        }



    }

}
