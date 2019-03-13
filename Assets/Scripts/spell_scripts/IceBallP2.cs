using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBallP2 : MonoBehaviour {

    public float speed;
    public float radius = 5.0F;

    private Vector3 movingDirection;
    public Rigidbody rb;
    Vector3 pushvec;
    Vector3 dirVector;
    public GameObject iceExplosion;
    public GameObject freezingEffect;
    public string shooterTag;
    private bool ShieldCollision = false;
    private bool reflected = false;
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

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 11)
        {
            Debug.Log("Hit an object from Layer PLAYER");
            // ADD && FOR EACH PLAYER TAG 
            if (other.gameObject.tag != shooterTag && other.gameObject.tag == "Player1" )
            {
                PsController script = other.GetComponent<PsController>();
                if (!script.shieldUp)
                {
                    Instantiate(iceExplosion, transform.position, transform.rotation);
                    PsController player = other.GetComponent<PsController>();
                    player.playerS = PsController.playerState.Frozen;
                    Destroy(this.gameObject);
                }
                else
                {
                    ShieldCollision = !ShieldCollision;
                    reflected = true;
                }


            }
            if (reflected && other.gameObject.tag == "Player2" )
            {
                Ps2Controller script = other.GetComponent<Ps2Controller>();
                if (!script.shieldUp)
                {
                    Instantiate(iceExplosion, transform.position, transform.rotation);
                    Ps2Controller player = other.GetComponent<Ps2Controller>();
                    player.playerS = Ps2Controller.playerState.Frozen;
                    Destroy(this.gameObject);
                }
                else
                {
                    ShieldCollision = !ShieldCollision;
                    reflected = true;
                }
            }
            if (other.gameObject.tag != shooterTag && other.gameObject.tag == "Player3" )
            {
                Ps3Controller script = other.GetComponent<Ps3Controller>();
                if (!script.shieldUp)
                {
                    Instantiate(iceExplosion, transform.position, transform.rotation);
                    Ps3Controller player = other.GetComponent<Ps3Controller>();
                    player.playerS = Ps3Controller.playerState.Frozen;
                    Destroy(this.gameObject);
                }
                else
                {
                    reflected = true;
                    ShieldCollision = !ShieldCollision;
                }
            }
            if (other.gameObject.tag != shooterTag && other.gameObject.tag == "Player4" )
            {
                Ps4Controller script = other.GetComponent<Ps4Controller>();
                if (!script.shieldUp)
                {

                    Instantiate(iceExplosion, transform.position, transform.rotation);
                    Ps4Controller player = other.GetComponent<Ps4Controller>();
                    player.playerS = Ps4Controller.playerState.Frozen;
                    Destroy(this.gameObject);
                }
                else
                {
                    reflected = true;
                    ShieldCollision = !ShieldCollision;
                }
            }
        }



    }

    /*  if (other.gameObject.tag == "Player1")
      {

          Instantiate(iceExplosion, transform.position, transform.rotation);
          PsController player = other.GetComponent<PsController>();
          player.playerS = PsController.playerState.Frozen;


          Debug.Log("Collided");


          Destroy(this.gameObject);

      }
      else if (other.gameObject.tag == "Player2")
      {
          Instantiate(iceExplosion, transform.position, transform.rotation);
          Ps2Controller player = other.GetComponent<Ps2Controller>();
          player.playerS = Ps2Controller.playerState.Frozen;


          Debug.Log("Collided");


          Destroy(this.gameObject);
      }
      else if (other.gameObject.tag == "Player3")
      {
          Instantiate(iceExplosion, transform.position, transform.rotation);
          Ps3Controller player = other.GetComponent<Ps3Controller>();
          player.playerS = Ps3Controller.playerState.Frozen;


          Debug.Log("Collided");


          Destroy(this.gameObject);
      }
      else if (other.gameObject.tag == "Player4")
      {
          Instantiate(iceExplosion, transform.position, transform.rotation);
          Ps4Controller player = other.GetComponent<Ps4Controller>();
          player.playerS = Ps4Controller.playerState.Frozen;


          Debug.Log("Collided");


          Destroy(this.gameObject);
      }
  }
 */
    /*else if (other.gameObject.tag == "Player1")
     {
         Instantiate(iceExplosion, transform.position, transform.rotation);
         PlayerControl player = other.GetComponent<PlayerControl>();
         player.playerS = PlayerControl.playerState.Frozen;

         /*  if (other.GetComponent<MeshRenderer>().enabled == true)
           {
               Vector3 pos = new Vector3(other.transform.position.x, other.transform.position.y - 0.8f, other.transform.position.z);
               Instantiate(freezingEffect, pos, other.transform.rotation, other.transform);
               other.GetComponent<MeshRenderer>().enabled = false;
           }
         Debug.Log("Collided");


         Destroy(this.gameObject);
     }*/

}


