using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    public float speed;
    public float radius = 5.0F;
    public float power = 550.0F;
    private Vector3 movingDirection;
    public Rigidbody rb;
    Vector3 pushvec;
    public Vector3 dirVector;
    public GameObject explosion;

    public string shooterTag;


    private bool ShieldCollision = false;
    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

        StartCoroutine(wait());


    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
            
    }
    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.layer == 11)
        {
            Debug.Log("player hit");
            Rigidbody rb = other.GetComponent<Rigidbody>();
            Vector3 dir = (other.transform.position - transform.position) * power;
            // dir.y = 40;
            if (other.tag.Equals("Player1"))
            {
                if (other.GetComponent<PsController>().enabled == true)
                {
                    other.GetComponent<PsController>().enabled = false;
                    rb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                    //Destroy(this.gameObject);
                }
                else
                {
                    rb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                }
            }
            if (other.tag.Equals("Player2"))
            {
                if (other.GetComponent<Ps2Controller>().enabled == true)
                {
                    other.GetComponent<Ps2Controller>().enabled = false;
                    rb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                    //Destroy(this.gameObject);
                }
                else
                {
                    rb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                }
            }
            if (other.tag.Equals("Player3"))
            {
                if (other.GetComponent<Ps3Controller>().enabled == true)
                {
                    other.GetComponent<Ps3Controller>().enabled = false;
                    rb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                    //Destroy(this.gameObject);
                }
                else
                {
                    rb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                }
            }
            if (other.tag.Equals("Player4"))
            {
                if (other.GetComponent<Ps4Controller>().enabled == true)
                {
                    other.GetComponent<Ps4Controller>().enabled = false;
                    rb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                    //Destroy(this.gameObject);
                }
                else
                {
                    rb.AddForceAtPosition(dir, other.GetComponent<Transform>().position, ForceMode.Impulse);
                }
            }


        }

    }
        }
