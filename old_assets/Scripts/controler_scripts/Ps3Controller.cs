using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ps3Controller : MonoBehaviour
{

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
    public GameObject paralyzedEffect;
    public GameObject sonicSpeedEffect;
    public GameObject spawningShield;
    //spells images
    public Image fireBallIcon;
    public Image teleportationIcon;
    public Image iceBallIcon;
    //life images
    public Image Life_1;
    public Image Life_2;
    public Image Life_3;
    public Image Life_4;
    public Image Life_5;

    public Image dead;
    //Spells cooldowns 
    public float fireballCD;
    public float iceballCD;
    public float teleportationCD;
    private bool fireballReady;
    private bool iceballReady;
    private bool teleportationReady;
    private bool sizePotion;
    private bool frozenGround;
    private bool playerHuge;
    private bool canTeleport;
    private bool spawning;
    //control buttons
    private bool controllerAssigned;
    private string movHorizontalaxis;
    private string movVerticalaxis;
    private string rotHorizontalaxis;
    private string rotVerticalaxis;
    private string xButton;
    private string triangleButton;
    private string squareButton;
    private string circleButton;
    private string l2Button;
    private string r2Button;
    private string r3Button;
    private string r1Button;

    public int controllerNumber;
    public bool spawnAgain;
    public GameObject teleportationSkill;
    private bool playerFrozen;
    public Rigidbody rbody;
    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public float teleportationDistance;
    public float desiredRotationSpeed;
    public float rotationSpeed;
    public float Speed;
    public float anglespeed;
    public float friction;

    private Quaternion targetRotation;
    public CharacterController controller;
    public bool isGrounded;
    private float verticalVal;
    private Vector3 moveVector;
    private Vector3 curVel;
    public SkinnedMeshRenderer meshRenderer;
    public bool shieldUp;
    //start Position
    private Vector3 startpos;
    //start Rotation
    private float startyRot;
    [SerializeField]
    private float rotThreshold = 0.1f;
   
    public int lifes;
    //state
    public playerState playerS;

    //Animation 
    public Animator anim;
    Quaternion initRotation;
    bool alreadyUP = false;
    bool notInCenter = true;
    public Material grey_texture;
    public Material wizard_mat;
    bool deathAnimationFinished = false;
    // Use this for initialization
    void Start()
    {
        initRotation = transform.rotation;
        spawning = false;
        canTeleport = true;
        playerHuge = false;
        friction = 1;
        frozenGround = false;
        sizePotion = false;
        controllerAssigned = false;
        fireballReady = true;
        if (fireballCD == 0 && iceballCD == 0 && teleportationCD == 0)
        {
            fireballCD = 1;
            iceballCD = 3;
            teleportationCD = 4;
        }
        iceballReady = true;
        shieldUp = false;
        playerFrozen = true;
        playerS = playerState.Alive;
        startpos = new Vector3(-2.12f, 1.78f, -1.6f);
        startyRot = this.gameObject.transform.rotation.y;
        spellInitPostion = this.transform.GetChild(0);
        meshRenderer = this.transform.Find("Toon Wizard").GetComponent<SkinnedMeshRenderer>();
        rbody = this.GetComponent<Rigidbody>();
        targetRotation = new Quaternion();
        anim = GetComponent<Animator>();
        EventManager.Instance.ItemTaken += OnItemTaken;
    }
    private void OnItemTaken(ItemBehavior item, int playerID)
    {
        if (item.itemType == ItemType.inkling)
        {
            StartCoroutine(FrozenGround());
        }
        if (playerID != 3)
        {
            if (item.itemType == ItemType.flash_gordon && !shieldUp)
            {
                StartCoroutine(Electrocuted());
            }
        }
        if (playerID == 3)
        {
            switch (item.itemType)
            {
                case ItemType.flash_gordon:
                    Debug.Log("lightstrike obtained");
                    break;

                case ItemType.inkling:

                    break;

                case ItemType.knack:

                    break;

                case ItemType.potion_fast:
                    StartCoroutine(SpeedUpgrade());
                    Debug.Log("Potion Fast obtained");
                    break;

                case ItemType.potion_mini:
                    if (!sizePotion)
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
                    break;

                case ItemType.potion_shield:
                    if (!sizePotion)
                    {
                        StartCoroutine(Shield());
                        Debug.Log("shield up");
                    }
                    break;

                case ItemType.potion_strength:
                    StartCoroutine(FireBallSpeedUpgrade());
                    Debug.Log("fireball upgraded");
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        checklifes();
        if (playerS == playerState.Alive)
        {
            if (isGrounded && !(playerS == playerState.Frozen) && controllerAssigned)
            {
                PlayerMoveAndRotation();
                shoot();
                Teleportation();
            }


            // Debug.DrawRay(transform.localPosition, desiredMoveDirection , Color.black);

            

            

            //  Debug.Log(desiredMoveDirection.z);
        }
        else if (playerS == playerState.Dead)
        {
            if (spawnAgain  )
            {
                if (lifes == 0)
                {
                    this.gameObject.SetActive(false);
                }

                //this.gameObject.transform.SetPositionAndRotation(startpos, Quaternion.Euler(0, startyRot, 0));
                //playerS = playerState.Spawning;
                initPlayer();
                deathAnimation();
                if (deathAnimationFinished)
                {
                    playerS = playerState.Spawning;
                }
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
        else if (playerS == playerState.Frozen)
        {
            if (playerFrozen)
            {
                StartCoroutine(FrozenStateDuration());
            }
        }
        else if (playerS == playerState.Spawning)
        {
            PlayerMoveAndRotation();
            if (!spawning)
            {
                StartCoroutine(SpawningCD());
            }
        }
        //update the life points of the player
        checklifes();

    }

   
    //   CHANGE TO THIS IF YOU RE USING A PS4 CONTROLLER
    void PlayerMoveAndRotation()
    {

        InputX = Input.GetAxis(movHorizontalaxis);
        InputZ = Input.GetAxis(movVerticalaxis);

        desiredMoveDirection.z = InputZ * Speed * Time.deltaTime;
        desiredMoveDirection.x = InputX * Speed * Time.deltaTime;
        if (!frozenGround)
        {
            // Movement

            // MOVEMENT  ON A FROZEN GROUND
            // curVel = Vector3.Lerp(curVel, desiredMoveDirection, friction * Time.deltaTime);
            // rbody.velocity = new Vector3(curVel.x, curVel.y, curVel.z);
            rbody.velocity = new Vector3(desiredMoveDirection.x, rbody.velocity.y, desiredMoveDirection.z);
        }
        else
        {
            curVel = Vector3.Lerp(curVel, desiredMoveDirection, friction * Time.deltaTime);
            rbody.velocity = new Vector3(curVel.x, curVel.y, curVel.z);
        }
        //gravity force
        rbody.AddForce(Vector3.down * 300);
        // Moving Animation
        if (Mathf.Abs(InputZ) > 0 || Mathf.Abs(InputX) > 0)
        {
            anim.SetFloat("Speed", Mathf.Abs(InputZ) + Mathf.Abs(InputX));
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }
        // Rotation
        InputX = Input.GetAxis(rotHorizontalaxis);
            InputZ = Input.GetAxis(rotVerticalaxis);


            if ((InputX != 0.0f || InputZ != 0.0f) && (Mathf.Abs(InputX) + Mathf.Abs(InputZ) > rotThreshold))
            {
                Vector3 CurrentRotation = transform.rotation.eulerAngles;
                targetRotation = Quaternion.Euler(new Vector3(CurrentRotation.x, Mathf.Atan2(InputX, InputZ) * Mathf.Rad2Deg, CurrentRotation.z));
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * anglespeed);
            }


            if (Input.GetButtonDown(r3Button))
            {
                Debug.Log("R2 pressed");


                transform.Rotate(transform.rotation.x, transform.rotation.y - 180, transform.rotation.z);
            }
        






        //if (Mathf.Abs(InputX) > 0)
        //{
        //    anglespeed += rotationSpeed * InputX;
        //    targetRotation = Quaternion.AngleAxis(anglespeed, Vector3.up);
        //    //  Debug.Log(targetRotation);

        //}




    }
    //remove all status when the player dies
    void initPlayer()
    {
        frozenGround = false;
        Speed = 300;
        if (shieldUp)
        {
            Destroy(GameObject.Find("Shield(Clone)"));
            shieldUp = false;
        }
    }

    public void deathAnimation()
    {
        if (notInCenter)
        {
            if (!alreadyUP)
            {

                transform.Find("Toon Wizard").GetComponent<Renderer>().material = grey_texture;
                Debug.Log("MOVING UP ");
                rbody.velocity = new Vector3(0, Speed * Time.deltaTime, 0);
                if (transform.position.y > 14f)
                {
                    alreadyUP = true;
                }
            }
            else
            {
                Debug.Log("moving to the center");
                Vector3 Arena_Center = new Vector3(0f, 6f, 0f);
                Vector3 movingDir = Arena_Center - transform.position;
                rbody.velocity = new Vector3(movingDir.x * Time.deltaTime * Speed * 2, movingDir.y, movingDir.z * Time.deltaTime * Speed * 2);
                if (transform.position.y < 6.1f)
                {
                    transform.rotation = initRotation;
                    desiredMoveDirection = Vector3.zero;
                    transform.Find("Toon Wizard").GetComponent<Renderer>().material = wizard_mat;
                    notInCenter = false;
                    deathAnimationFinished = true;
                }
            }
        }
    }

    IEnumerator SpawningCD()
    {
        spawning = true;
        this.tag = "Untagged";
        GameObject temp = Instantiate(spawningShield, transform.position, transform.rotation, this.transform);
        yield return new WaitForSeconds(2);
        this.tag = "Player3";
        Destroy(temp);
        spawning = false;
        playerS = playerState.Alive;
    }

    IEnumerator Electrocuted()
    {  
        Instantiate(thunderStrike, transform.position, transform.rotation, transform);
        Instantiate(paralyzedEffect, transform.position, transform.rotation, transform);
        Speed = 100;
        yield return new WaitForSeconds(5);
        Speed = 300;
    }

    IEnumerator FrozenGround()
    {
        friction = 0.5f;
        frozenGround = true;
        Speed = 1500;
        yield return new WaitForSeconds(7);
        frozenGround = false;
        friction = 1;
        Speed = 300;
    }
    IEnumerator TeleportationCD()
    {
        canTeleport = false;
        teleportationIcon.fillAmount = 0;
        Instantiate(teleportationSkill, this.transform.position, this.transform.rotation);
        Instantiate(teleportationSkill, this.transform.position, this.transform.rotation, this.transform);
        //  Vector3 teleportVec = Vector3.forward * teleportationDistance;
        transform.position += desiredMoveDirection.normalized * teleportationDistance;
        //transform.Translate(teleportVec);
        yield return new WaitForSeconds(teleportationCD / 2);
        teleportationIcon.fillAmount += 0.5f;
        yield return new WaitForSeconds(teleportationCD / 2);
        teleportationIcon.fillAmount += 0.5f;
        canTeleport = true;
    }

    IEnumerator IceballCD()
    {
        iceballReady = false;
        iceBallIcon.fillAmount = 0;
        anim.Play("ProjectileAttack");
        //  Debug.Log("key pressed");
        GameObject iceball = Instantiate(iceBall, spellInitPostion.transform.position, transform.rotation);
        IceBallP3 script = iceball.GetComponent<IceBallP3>();
        script.shooterTag = this.tag;
        for (int i = 0; i < iceballCD; i++)
        {
            yield return new WaitForSeconds(i);
            iceBallIcon.fillAmount += 0.334f;
        }
        iceBallIcon.fillAmount = 1;
        iceballReady = true;

    }
    IEnumerator FrozenStateDuration()
    {
        playerFrozen = false;
        meshRenderer.enabled = false;
        Instantiate(freezingEffect, transform.position, transform.rotation, transform);
        yield return new WaitForSeconds(1.5f);

        meshRenderer.enabled = true;
        playerFrozen = true;
        playerS = playerState.Alive;
    }

    IEnumerator Shield()
    {
        shieldUp = true;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        Instantiate(shield, transform.position, transform.rotation, this.transform);
        yield return new WaitForSeconds(5);
        shieldUp = false;

    }

    IEnumerator SpeedUpgrade()
    {
        Instantiate(sonicSpeedEffect, transform.position, transform.rotation, transform);
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
    IEnumerator Big()
    {
        playerHuge = true;
        sizePotion = true;
        transform.localScale = new Vector3(3, 3, 3);
        yield return new WaitForSeconds(5);
        transform.localScale = new Vector3(1, 1, 1);
        sizePotion = false;
        playerHuge = false;
    }

    IEnumerator FireBallSpeedUpgrade()
    {
        fireballCD = 0;
        yield return new WaitForSeconds(2f);
        fireballCD = 1;

    }

    IEnumerator FireBallCooldown()
    {
        fireballReady = false;
        fireBallIcon.fillAmount = 0;
        anim.Play("ProjectileAttack");
        if (!playerHuge)
        {
            GameObject fireball = Instantiate(fireBall, spellInitPostion.transform.position, transform.rotation);
            FireBallP3 FBscript = fireBall.GetComponent<FireBallP3>();
            FBscript.shooterTag = this.tag;
        }
        else
        {
            Vector3 pos = new Vector3(spellInitPostion.transform.position.x, spellInitPostion.transform.position.y - 1.7f, spellInitPostion.transform.position.z);
            GameObject fireball = Instantiate(fireBall, pos, transform.rotation);
            FireBallP3 FBscript = fireBall.GetComponent<FireBallP3>();
            FBscript.shooterTag = this.tag;
        }

        yield return new WaitForSeconds(fireballCD / 2);
        fireBallIcon.fillAmount += fireballCD / 2;
        yield return new WaitForSeconds(fireballCD / 2);
        fireBallIcon.fillAmount += fireballCD / 2;

        fireballReady = true;
    }

    public void checklifes()
    {
        switch (lifes)
        {
            case 0:
                Life_1.SetTransparency(0f);
                dead.SetTransparency(1f);
                break;
            case 1:
                Life_2.SetTransparency(0f);
                break;
            case 2:
                Life_3.SetTransparency(0f);
                break;
            case 3:
                Life_4.SetTransparency(0f);
                break;
            case 4:
                Life_5.SetTransparency(0f);
                break;
        }
    }

    void Teleportation()
    {
        if (Input.GetButtonDown(r1Button) && canTeleport)
        {
            StartCoroutine(TeleportationCD());
        }
    }
    public void SetControllerNumber(int number)
    {
        controllerAssigned = true;

        movHorizontalaxis = "P" + number + "HorizontalMov";
        movVerticalaxis = "P" + number + "VerticalMov";
        rotHorizontalaxis = "P" + number + "HorizontalRot";
        rotVerticalaxis = "P" + number + "VerticalRot";
        xButton = "J" + number + "X";
        triangleButton = "P" + number + " triangleButton";
        squareButton = "P" + number + " squareButton";
        circleButton = "P" + number + " circleButton";
        l2Button = "P" + number + "Fire1";
        r2Button = "P" + number + "Fire2";
        r3Button = "P" + number + "R3";
        r1Button = "P" + number + "Teleport";
    }
    // shoot the spells
    void shoot()
    {
        if (Input.GetButtonDown(l2Button) && fireballReady)
        {
            StartCoroutine(FireBallCooldown());

        }
        if (Input.GetButtonDown(r2Button) && iceballReady)
        {
            StartCoroutine(IceballCD());
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

        if (col.gameObject.layer == 9 && playerS != playerState.Dead)
        {
            playerS = playerState.Dead;
            notInCenter = true;
            alreadyUP = false;
            deathAnimationFinished = false;
            --lifes;
            checklifes();
        }

        if (col.gameObject.layer == 10)
        {
            Destroy(col.gameObject);
        }

    }
}