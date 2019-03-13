using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    public Transform dancefloor;

    //later teleport and item effects
    public enum playerState
    {
        Dead,
        Alive,
        Frozen,
        Spawning
    };


    //Spells
    public Transform spellInitPostion;
    public GameObject thunderStrike;
    public GameObject iceBall;
    public GameObject fireBall;
    public GameObject freezingEffect;
    public GameObject shield;
    //Spells cooldowns 
    public float fireballCD;
    private bool fireballReady;
    private bool shieldUp;
    private bool sizePotion;

    public bool spawnAgain;
    public GameObject teleportationSkill;
    private bool playerFrozen;
    public Rigidbody rbody;

    public float InputX;
    public float InputZ;

    private Vector3 curVel;
    public Vector3 desiredMoveDirection;
    public float teleportationDistance;
    public float desiredRotationSpeed;
    public float rotationSpeed;
    public float Speed;
    public float anglespeed;
    private Quaternion targetRotation;

    //public CharacterController controller;
    public bool isGrounded;
    private float verticalVal;
    private Vector3 moveVector;
    public SkinnedMeshRenderer meshRenderer;
    
    //start Position
    private Vector3 startpos;
    //start Rotation
    private float startyRot;
    [SerializeField] private float rotThreshold = 0.1f;

    //state
    public playerState playerS;

    //Animation 
    public Animator anim;
    public float angle;
    // Use this for initialization
    void Start()
    {
        sizePotion = false;
        fireballCD = 1;
        fireballReady = true;
        shieldUp = false;
        playerFrozen = true;
        playerS = playerState.Alive;
        startpos = this.gameObject.transform.position;
        startyRot = this.gameObject.transform.rotation.y;
        spellInitPostion = this.transform.GetChild(0);
        meshRenderer = this.transform.Find("Toon Wizard").GetComponent<SkinnedMeshRenderer>();
        rbody = this.GetComponent<Rigidbody>();
        targetRotation = new Quaternion();
        anim = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {


      //  Debug.Log("floor :" + dancefloor.rotation.eulerAngles);
        //Debug.Log("player:" + this.transform.rotation.eulerAngles);
        if (playerS == playerState.Alive)
        {
            if (isGrounded && !(playerS == playerState.Frozen))
            {
                PlayerMoveAndRotation();
            }


            // Debug.DrawRay(transform.localPosition, desiredMoveDirection , Color.black);
            rbody.AddForce(Vector3.down * 300);
            shoot();
            Teleportation();

            if (InputZ > 0 )
            {
                anim.SetFloat("Speed", InputZ);
            }
            else
            {
                anim.SetFloat("Speed", 0);
            }

            //  Debug.Log(desiredMoveDirection.z);
        }
        else if (playerS == playerState.Dead)
        {
            if (spawnAgain)
            {
                playerS = playerState.Spawning;
                this.gameObject.transform.SetPositionAndRotation(startpos, Quaternion.Euler(0, startyRot, 0));
                playerS = playerState.Alive;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        else if (playerS == playerState.Frozen)
        {
            if (playerFrozen)
            {
                StartCoroutine(FrozenStateDuration());
            }
        }

    }

    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");
        // Debug.Log(rbody.velocity);
        desiredMoveDirection.z = InputZ * Speed * Time.deltaTime;
        Vector3 tmpVel = transform.TransformDirection(desiredMoveDirection);
      //  curVel = Vector3.Lerp(curVel, tmpVel, 1 * Time.deltaTime);
        rbody.velocity = new Vector3(tmpVel.x, tmpVel.y, tmpVel.z);


        //Jump();
       /* RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down),out hit , 10f ))
        {    
            if (hit.collider.tag == "Ground")
            {
                Debug.Log("Ground hit");
                if (InputX != 0)
                {
                    
                    angle += InputX * anglespeed;
                    Vector3 CurrentRotation = transform.localRotation.eulerAngles;
                    Vector3 FloorRotation = hit.transform.localRotation.eulerAngles;
                    Debug.Log("Current :" + CurrentRotation);
                    Debug.Log("Floor: " +FloorRotation);
                    targetRotation = Quaternion.Euler(new Vector3(- FloorRotation.x  , CurrentRotation.y + angle * Mathf.Deg2Rad, CurrentRotation.z *Mathf.Deg2Rad));
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * anglespeed);
                }
            }
            
        }*/
       //if (InputX != 0)
       // {
       //     Vector3 floorRotation = dancefloor.rotation.eulerAngles;
       //     Vector3 currenRotation = transform.rotation.eulerAngles;
       //     angle += InputX * anglespeed;
       //     Vector3 target = new Vector3((floorRotation.x + 270) + (100 / currenRotation.y) * 0.6f, transform.rotation.y + angle, 0);
       //     targetRotation = Quaternion.Euler(target);
       //     Debug.Log("Target : " + target + "Player : " + currenRotation + "Floor : " + floorRotation);
       //     transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * anglespeed);
       // }
       // Debug.DrawRay(spellInitPostion.transform.position, tmpVel, Color.red);

        if (Mathf.Abs(InputX) > 0)
        {
            anglespeed += rotationSpeed * InputX;
            targetRotation = Quaternion.AngleAxis(anglespeed, Vector3.up);

            //  Debug.Log(targetRotation);
            transform.rotation = targetRotation;
        }



        if (Input.GetKeyDown("x"))
        {
            Debug.Log("R2 pressed");
          
            transform.Rotate(transform.rotation.x, transform.rotation.y - 180, transform.rotation.z);


        }
        
    }
    //   CHANGE TO THIS IF YOU RE USING A PS4 CONTROLLER
    //    void PlayerMoveAndRotation()
    //{

    //    // Movement
    //    InputX = Input.GetAxis("Horizontal");
    //    InputZ = Input.GetAxis("Vertical");
    //    // Debug.Log(rbody.velocity);
    //    desiredMoveDirection.z = InputZ * Speed * Time.deltaTime;
    //    desiredMoveDirection.x = InputX * Speed * Time.deltaTime;
    //    //Vector3 tmpVel = transform.TransformDirection (desiredMoveDirection);

    //    rbody.velocity = new Vector3(desiredMoveDirection.x, rbody.velocity.y, desiredMoveDirection.z);
    //    //Jump();

    //    // Rotation
    //    InputX = Input.GetAxis("HorizontalRot");
    //    InputZ = Input.GetAxis("VerticalRot");


    //    if ((InputX != 0.0f || InputZ != 0.0f) && (Mathf.Abs(InputX) + Mathf.Abs(InputZ) > rotThreshold))
    //    {
    //        Vector3 CurrentRotation = transform.rotation.eulerAngles;
    //        targetRotation = Quaternion.Euler(new Vector3(CurrentRotation.x, Mathf.Atan2(InputX, InputZ) * Mathf.Rad2Deg, CurrentRotation.z));
    //    }



    //    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * anglespeed);



    //    //if (Mathf.Abs(InputX) > 0)
    //    //{
    //    //    anglespeed += rotationSpeed * InputX;
    //    //    targetRotation = Quaternion.AngleAxis(anglespeed, Vector3.up);
    //    //    //  Debug.Log(targetRotation);

    //    //}




    //}




    IEnumerator FrozenStateDuration()
    {
        playerFrozen = false;
        meshRenderer.enabled = false;
        Instantiate(freezingEffect, transform.position, transform.rotation, transform);
        yield return new WaitForSeconds(3);

        meshRenderer.enabled = true;
        playerFrozen = true;
        playerS = playerState.Alive;
    }

    IEnumerator Shield()
    {
        shieldUp = true;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Instantiate(shield, transform.position, transform.rotation, this.transform);
        yield return new WaitForSeconds(5);
        shieldUp = false;

    }

    IEnumerator SpeedUpgrade()
    {
        Speed = 800f;
        yield return new WaitForSeconds(5);
        Speed = 300;
    }
    IEnumerator Mini()
    {
        sizePotion = true;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Collider col = GetComponent<Collider>();
        (col as CapsuleCollider).radius = 0.3f;
        
        yield return new WaitForSeconds(5);
        transform.localScale = new Vector3(1, 1, 1);
        (col as CapsuleCollider).radius = 0.7f;
        sizePotion = false;
    }
    IEnumerator Big() {
        sizePotion = true;
        transform.localScale = new Vector3(3, 3, 3);
        yield return new WaitForSeconds(5);
        transform.localScale = new Vector3(1, 1, 1);
        sizePotion = false;
    }

    IEnumerator FireBallSpeedUpgrade()
    {
        fireballCD = 0;
        yield return new WaitForSeconds(1.5f);
        fireballCD = 1;

    }

    IEnumerator FireBallCooldown()
    {
        fireballReady = false;
        anim.Play("ProjectileAttack");
        Instantiate(fireBall, spellInitPostion.transform.position, transform.rotation);
        yield return new WaitForSeconds(fireballCD);
        fireballReady = true;
    }

    void Teleportation()
    {
        if (Input.GetKeyDown("e"))
        {
            Instantiate(teleportationSkill, this.transform.position, this.transform.rotation);
            Instantiate(teleportationSkill, this.transform.position, this.transform.rotation, this.transform);
            Vector3 teleportVec = Vector3.forward * teleportationDistance;
            transform.Translate(teleportVec);
        }
    }
    // shoot the spells
    void shoot()
    {
        if (Input.GetButtonDown("q") && fireballReady)
        {
            StartCoroutine(FireBallCooldown());

        }
        if (Input.GetKeyDown("w"))
        {
            anim.Play("ProjectileAttack");
            //  Debug.Log("key pressed");
            Instantiate(iceBall, spellInitPostion.transform.position, transform.rotation);
        }
        if (Input.GetKeyDown("r"))
        {
            anim.Play("ProjectileAttack");
            //  Debug.Log("key pressed");
            Instantiate(thunderStrike, transform.position, transform.rotation, this.transform);
        }


    }

    // check if the player is on the ground
    private void OnTriggerEnter(Collider col)
    {

    }

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag.Equals("Ground"))
        {
            isGrounded = true;
        }

        if (col.gameObject.layer == 9)
        {
            playerS = playerState.Dead;
        }

        if (col.gameObject.layer == 10)
        {
            if (col.gameObject.name.Equals("potion_shield(Clone)") && !shieldUp && !sizePotion)
            {
                StartCoroutine(Shield());
                Debug.Log("shield up");
            }
            if (col.gameObject.name.Equals("potion_strength(Clone)"))
            {
                StartCoroutine(FireBallSpeedUpgrade());
                Debug.Log("fireball upgraded");
            }
            if (col.gameObject.name.Equals("Flash_Gordon(Clone)"))
            {
                Debug.Log("lightstrike obtained");
            }
            if (col.gameObject.name.Equals("potion_fast(Clone)"))
            {
                StartCoroutine(SpeedUpgrade());
                Debug.Log("Potion Fast obtained");
            }
            if (col.gameObject.name.Equals("potion_mini(Clone)") && !sizePotion)
            {
                Debug.Log("Potion Mini obtained");
                int rand = Random.Range(0, 2);
                
                switch (rand)
                {
                    case 0:

                        StartCoroutine(Big());
                        break;
                    case 1:
                       StartCoroutine(Mini());
                        break;

                }
                
              
            }

            Destroy(col.gameObject);
        }

    }

}