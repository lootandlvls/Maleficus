using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Maleficus.Utils;

public class Player : BNJMOBehaviour, IPlayer
{
    public EPlayerID PlayerID { get; set; }
    public ETeamID TeamID { get; set; }
    public Vector3 Position { get { return transform.position; } }
    public Quaternion Rotation { get { return transform.rotation; } }
    public bool IsDead { get; private set; } = false;
    public Dictionary<ESpellSlot, bool> ReadyToUseSpell { get; } = new Dictionary<ESpellSlot, bool>();
    public Dictionary<ESpellSlot, float> SpellCooldown { get; } = new Dictionary<ESpellSlot, float>();
    public Dictionary<ESpellSlot, float> SpellCastDuration { get; } = new Dictionary<ESpellSlot, float>();
    public bool IsReadyToShoot { get; set; }
    public bool IsPlayerCharging { get; set; }
    public bool IsUnhittable { get; set; } = false;
    public Vector3 SpellInitPosition { get { return spellInitPosition.position; } }
    public Vector3 SpellEndPosition { get { return spellEndPosition.position; } }
    public int SpellChargingLVL { get; private set; } = 1;
    public bool HasCastedSpell { get; set; } = false;
    public Vector3 PushVelocity { get; private set; }
    public float MaxPushVelocity { get { return maximumPushVelocity; } }

    [Header("Charging Spell Effects")]
    [SerializeField] private GameObject chargingBodyEnergy;
    [SerializeField] private GameObject chargingWandEnergy;
    [SerializeField] private float speed;
    [SerializeField] private float maximumPushVelocity = 25.0f;
    [Range(0.1f, 3.0f)]
    [SerializeField] private float fallingTime = 0.3f;
    [SerializeField] private float unhittableTime = 1.0f;
    [SerializeField] private Transform spellInitPosition;
    [SerializeField] private Transform spellEndPosition;

    private Animator myAnimator;
    private DirectionalSprite[] myDirectionalSprites;

    private float lastTimeSinceRotated;
    private float currentSpeed;
    private IEnumerator UpdatePushVelocityEnumerator;
    private IEnumerator SpellChargingEnumerator;
    private Vector3 movingDirection;
    private Vector3 GravityVelocity;
    private JoysticksInput joysticksInput = new JoysticksInput();

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myDirectionalSprites = GetComponentsInChildren<DirectionalSprite>();
        myAnimator = GetComponent<Animator>();

    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.INPUT_JoystickMoved.Event += On_INPUT_JoystickMoved_Event;
        EventManager.Instance.SPELLS_Teleport += On_SPELLS_Teleport;

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EventManager.IsInstanceSet)
        {
            EventManager.Instance.INPUT_JoystickMoved.Event -= On_INPUT_JoystickMoved_Event;
            EventManager.Instance.SPELLS_Teleport -= On_SPELLS_Teleport;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        //TODO: only activate after respawn
        StartCoroutine(UnhittableCoroutine());


    }
    protected override void Start()
    {
        base.Start();

        IsReadyToShoot = false;
        IsPlayerCharging = false;

        InitializeDictionaries();

        myAnimator.SetBool("idle", true);

        currentSpeed = speed;
    }

    protected override void Update()
    {
        base.Update();

        if (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {
            UpdateMovementAndRotation();
        }
    }

    private void On_INPUT_JoystickMoved_Event(NetEvent_JoystickMoved eventHandle)
    {
        EJoystickType joystickType = eventHandle.JoystickType;
        float joystick_X = eventHandle.Joystick_X;
        float joystick_Y = eventHandle.Joystick_Y;
        EPlayerID playerID = Maleficus.Utils.GetPlayerIDFrom(eventHandle.SenderID);


        if (PlayerID == playerID)
        {
            if (joystickType == EJoystickType.MOVEMENT)
            {
                joysticksInput.JoystickValues[EInputAxis.MOVE_X] = joystick_X;
                joysticksInput.JoystickValues[EInputAxis.MOVE_Y] = joystick_Y;
            }
            else if (joystickType == EJoystickType.ROTATION)
            {
                joysticksInput.JoystickValues[EInputAxis.ROTATE_X] = joystick_X;
                joysticksInput.JoystickValues[EInputAxis.ROTATE_Y] = joystick_Y;
            }
        }
    }

    private void On_SPELLS_Teleport(float distance, EPlayerID castingPlayerID)
    {
        if ((PlayerID == castingPlayerID)
         && (IS_NOT_NULL(joysticksInput)))
        {
            float InputH = joysticksInput.JoystickValues[EInputAxis.ROTATE_X];
            float InputV = joysticksInput.JoystickValues[EInputAxis.ROTATE_Y];

            Vector3 TeleportDirection = transform.forward;
            transform.position += TeleportDirection * distance;
            GameObject teleportEffect = Resources.Load<GameObject>(Maleficus.Consts.PATH_EFFECT_TELEPORT);
            Instantiate(teleportEffect, transform.position, transform.rotation);

        }
    }

    private void UpdateMovementAndRotation()
    {
        if (IS_NOT_NULL(joysticksInput))
        {
            float Move_X = joysticksInput.JoystickValues[EInputAxis.MOVE_X];
            float Move_Y = joysticksInput.JoystickValues[EInputAxis.MOVE_Y];
            float Rotate_X = joysticksInput.JoystickValues[EInputAxis.ROTATE_X];
            float Rotate_Y = joysticksInput.JoystickValues[EInputAxis.ROTATE_Y];

            Move(Move_X, Move_Y);
            Rotate(Rotate_X, Rotate_Y);

            if (joysticksInput.IsMoving() == true)
            // if moving
            {
                if (myAnimator != null)
                {
                    myAnimator.SetBool("idle", false);
                }
            }
            // if not moving
            else
            {
                if (myAnimator != null)
                {
                    myAnimator.SetBool("idle", true);
                }
            }

            if (joysticksInput.IsRotating() == true)
            // if rotating
            {

                SetDirectionalSpritesVisible(true);
                lastTimeSinceRotated = Time.time;
            }
            else
            // if not Rotating
            {
                SetDirectionalSpritesVisible(false);

                if ((joysticksInput.IsMoving() == true)
                    && (Time.time - lastTimeSinceRotated > 0.5f))
                // if moving for 1 second since last rortation
                {
                    LookAtMovingDirection();
                }
            }
        }
    }


    #region INPUT
    private void Move(float axis_X, float axis_Z)
    {
        movingDirection = new Vector3(axis_X, 0.0f, axis_Z).normalized * (Mathf.Pow(axis_X, 2.0f) + Mathf.Pow(axis_Z, 2.0f));

        Vector3 movementVelocity = movingDirection * currentSpeed * 0.1f;

        Vector3 finalVelocity = movementVelocity + PushVelocity + GravityVelocity;
        transform.localPosition += finalVelocity * Time.deltaTime;
    }

    private void Rotate(float axis_X, float axis_Z)
    {
        DebugManager.Instance.Log(4, " PLAYER ROTATE ");
        if (Mathf.Abs(axis_X) + Mathf.Abs(axis_Z) > 0.0f)
        {
            RotateRelativeToGrandParentRotation(axis_X, -axis_Z);
        }
    }

    private void LookAtMovingDirection()
    {
        float axis_X = joysticksInput.JoystickValues[EInputAxis.MOVE_X];
        float axis_Z = joysticksInput.JoystickValues[EInputAxis.MOVE_Y];

        RotateRelativeToGrandParentRotation(axis_X, axis_Z);
    }

    private void RotateRelativeToGrandParentRotation(float axis_X, float axis_Z)
    {
        Transform myGrandTransform = GetGrandFatherTransfrom();
        Vector3 newForwardDirection = myGrandTransform.TransformDirection(new Vector3(axis_X, 0.0f, axis_Z));
        Quaternion newRotation = Quaternion.LookRotation(newForwardDirection, transform.up);
        transform.rotation = newRotation;
    }


    public void StartChargingSpell(ISpell spell, ESpellSlot spellSlot)
    {
        if (IsPlayerCharging == false)
        {
            if (spell.MovementType == ESpellMovementType.LINEAR_HIT)
            {
                IsPlayerCharging = true;

                //LogConsole("Player started Charging", "SPELL_CHARGE");
                StartNewCoroutine(ref SpellChargingEnumerator, SpellChargingCoroutine(spellSlot));
                //StartCoroutine(SpellChargingCoroutine(spellSlot));
            }

        }
    }

    public void StopChargingSpell(ISpell spell, ESpellSlot spellSlot)
    {
        //LogConsole("player stopped charging " + spellSlot, "SPELL_CHARGE");

        IsPlayerCharging = false;

        if (spell.MovementType == ESpellMovementType.LINEAR_LASER)
        {
            StartCoroutine(SlowDownPlayerCoroutine(0, spell.CastDuration));
        }
        else
        {
            StopSlowDownPlayer();

        }
    }

    public void RotateToClosestPlayer()
    {
        if (joysticksInput.IsRotating() == false)
        {
            lastTimeSinceRotated = Time.time;

            Player closestPlayer = GetClosestPlayer(this);
            if (closestPlayer)
            {
                Vector2 closestPlayerDirection = (Get2DVector(closestPlayer.Position - Position)).normalized;
                Rotate(closestPlayerDirection.x, -closestPlayerDirection.y);
            }
        }
    }



    private void InitializeDictionaries()
    {
        foreach (ESpellSlot spellSlot in Enum.GetValues(typeof(ESpellSlot)))
        {
            if (spellSlot != ESpellSlot.NONE)
            {
                ReadyToUseSpell[spellSlot] = true;
                AbstractSpell chosenSpell = SpellManager.Instance.GetChosenSpell(PlayerID, spellSlot);
                if (IS_NOT_NULL(chosenSpell))
                {
                    SpellCooldown[spellSlot] = chosenSpell.Cooldown;
                    SpellCastDuration[spellSlot] = chosenSpell.CastDuration;
                }
            }
        }
    }

    private IEnumerator SlowDownPlayerCoroutine(float slowDownSpeed, float duration)
    {
        myAnimator.SetBool("channeling", true);
        currentSpeed = slowDownSpeed;
        Debug.Log("PLAYER SLOWED : SPEED = " + currentSpeed);
        yield return new WaitForSeconds(duration);

        currentSpeed = speed;
        myAnimator.SetBool("channeling", false);
    }

    private void SlowDownPlayer(float slowDownSpeed)
    {
        myAnimator.SetBool("channeling", true);
        currentSpeed = slowDownSpeed;
    }

    private void StopSlowDownPlayer()
    {
        myAnimator.SetBool("channeling", false);
        currentSpeed = speed;
    }

    private IEnumerator SetReadyToUseSpellCoroutine(float time, ESpellSlot spellSlot)
    {
        yield return new WaitForSeconds(time);

        IsReadyToShoot = true;
        ReadyToUseSpell[spellSlot] = true;

        //LogConsole("ready to use the spell again", "SPELL_CHARGE");
    }



    private IEnumerator SpellChargingCoroutine(ESpellSlot spellSlot)
    {
        //LogConsole("Starting coroutine > " + "IsPlayerCharging : " + IsPlayerCharging + " | readyToUseSpell : " + ReadyToUseSpell[spellSlot] + " | IsReadyToShoot : " + IsReadyToShoot, "SPELL_CHARGE");
        while ((IsPlayerCharging == true)
            && ((ReadyToUseSpell[spellSlot] == false) || (IsReadyToShoot == false)))
        {
            //LogConsole("IsPlayerCharging : " + IsPlayerCharging + " | readyToUseSpell : " + ReadyToUseSpell[spellSlot] + " | IsReadyToShoot : " + IsReadyToShoot, "SPELL_CHARGE");
            yield return new WaitForEndOfFrame();
        }

        if (IsPlayerCharging == true)
        {
            SlowDownPlayer(speed / 2.0f);
            myAnimator.SetBool("charging", true);
            int counter = 0;

            // Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 90, 1);
            Vector3 position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            GameObject wandEffect = Instantiate(chargingWandEnergy, position, transform.rotation);
            wandEffect.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
            GameObject bodyEffect = Instantiate(chargingBodyEnergy, transform.position, transform.rotation);
            bodyEffect.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
            bodyEffect.transform.parent = this.transform;
            wandEffect.transform.parent = this.transform;
            ParticleSystem particleSystemWandEffect = wandEffect.GetComponent<ParticleSystem>();
            ParticleSystem particleSystemBodyEffect = bodyEffect.GetComponent<ParticleSystem>();

            SpellChargingLVL = 0;
            while (IsPlayerCharging)
            {
                var mainPS = particleSystemBodyEffect.main;
                mainPS.maxParticles = counter;
                mainPS.maxParticles = counter;


                yield return new WaitForSeconds(0.0f);
                counter += 4;       // TODO: Add how an attribute in spell to influence how fast second level is charged
                if (counter > 100)
                {
                    if (SpellChargingLVL != 2)
                    {
                        SpellChargingLVL = 2;
                        //LogConsole("Spell upgraded to lvl 2", "SPELL_CHARGE");
                    }
                }
                else
                {
                    if (SpellChargingLVL != 1)
                    {

                        SpellChargingLVL = 1;
                        //LogConsole("Spell upgraded to lvl 1", "SPELL_CHARGE");
                    }

                }
                yield return new WaitForEndOfFrame();
            }
            //LogConsole("spellCharging function Done!!", "SPELL_CHARGE");

            myAnimator.SetBool("charging", false);
            StopSlowDownPlayer();

            particleSystemBodyEffect.Stop();
            particleSystemWandEffect.Stop();

            //LogConsole("counter = " + counter, "SPELL_CHARGE");
        }
    }
    #endregion


    private void SetDirectionalSpritesVisible(bool isVisible)
    {
        if (myDirectionalSprites.Length == 0)
        {
            Debug.LogError("PLayer's directional sprites are empty!");
            myDirectionalSprites = GetComponentsInChildren<DirectionalSprite>();
        }

        foreach (DirectionalSprite directionalSprite in myDirectionalSprites)
        {
            if (isVisible == true)
            {
                directionalSprite.ShowSprite();
            }
            else
            {
                directionalSprite.HideSprite();
            }
        }
    }

    public void DoLazerAnimation(float spellDuration)
    {
        StartCoroutine(LazerAnimationCoroutine(spellDuration));
    }

    IEnumerator LazerAnimationCoroutine(float spellDuration)
    {
        currentSpeed = 0;
        IsReadyToShoot = false;
        myAnimator.SetBool("channeling", true);
        yield return new WaitForSeconds(spellDuration);
        myAnimator.SetBool("channeling", false);
        currentSpeed = speed;
        IsReadyToShoot = true;

    }


    public void DoShockwaveAnimation()
    {
        myAnimator.SetTrigger("shockwave");
    }

    public void DoTeleportAnimation()
    {
        myAnimator.SetTrigger("teleport");
    }

    public void DoProjectileAttackAnimation()
    {
        myAnimator.SetTrigger("projectileAttack");
    }


    public void SetPlayerFrozen(bool isFrozen)
    {
        if (isFrozen == true)
        {
            IsReadyToShoot = false;
            currentSpeed = 0;
        }
        else
        {
            currentSpeed = speed;
            IsReadyToShoot = true;
        }
    }

    public void SetPlayerStunned(bool isStunned)
    {
        if (isStunned == true)
        {
            currentSpeed = 0;
        }
        else
        {
            currentSpeed = speed;
        }
    }

    public void SetPlayerParalyzed(bool isParalyzed, float effectStrenght)
    {
        if (isParalyzed == true)
        {
            currentSpeed = speed / effectStrenght;
        }
        else
        {
            currentSpeed = speed;
        }
    }

    public void SetPlayerSpeedBoost(int speedBoost)
    {
        currentSpeed = speed * speedBoost;
    }

    public void ResetSpellChargingLVL()
    {
        SpellChargingLVL = 1;
    }

    public void PushPlayer(Vector3 velocity, float duration)
    {
        PushVelocity += velocity;
        if ((MotherOfManagers.Instance.IsLimitMaxPushPower)
            && (PushVelocity.magnitude > maximumPushVelocity))
        {
            PushVelocity = PushVelocity.normalized * maximumPushVelocity;
        }

        if (duration <= 0.0f)
        {
            duration = 0.1f;
        }
        StartNewCoroutine(ref UpdatePushVelocityEnumerator, UpdatePushVelocityCoroutine(duration));
    }




    private IEnumerator UpdatePushVelocityCoroutine(float duration)
    {
        float startTime = Time.time;
        Vector3 startVelocity = PushVelocity;

        float progress = (Time.time - startTime) / duration;
        while (progress < 1.0f)
        {
            progress = (Time.time - startTime) / duration;

            PushVelocity = Vector3.Lerp(startVelocity, Vector3.zero, progress);

            yield return new WaitForEndOfFrame();
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Ground"))
        {
            GravityVelocity = new Vector3(0, -9.81f, 0);
            IsDead = true;
            StopAllCoroutines();
            IsReadyToShoot = false;

            StartCoroutine(FallingCoroutine());
        }
    }

    private IEnumerator FallingCoroutine()
    {

        yield return new WaitForSeconds(fallingTime);
        PlayerManager.Instance.OnPlayerDead(PlayerID);

    }

    public void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    private Transform GetGrandFatherTransfrom()
    {
        Transform myGrandParentTransform = transform;
        while (myGrandParentTransform.parent != null)
        {
            myGrandParentTransform = myGrandParentTransform.parent;
        }

        return myGrandParentTransform;
    }

    public void ActivateShield(float duration)
    {
        StartCoroutine(ShieldActivatedCoroutine(duration));
    }
    private IEnumerator ShieldActivatedCoroutine(float duration)
    {
        this.tag = Maleficus.Consts.TAG_PLAYER_SHIELDED;
        yield return new WaitForSeconds(duration);
        this.tag = Maleficus.Consts.TAG_PLAYER;
    }
    private IEnumerator UnhittableCoroutine()
    {
        this.tag = Maleficus.Consts.TAG_PLAYER_SHIELDED;
        IsUnhittable = true;
        GameObject effectPrefab = Resources.Load<GameObject>(Maleficus.Consts.PATH_EFFECT_UNHITTABLE);
        GameObject effect = Instantiate(effectPrefab, transform.position, transform.rotation);
        effect.transform.parent = this.transform;
        SpellTimer spellTimer = effect.GetComponent<SpellTimer>();
        if (IS_NOT_NULL(spellTimer))
        {
            spellTimer.time = unhittableTime;
        }
        yield return new WaitForSeconds(unhittableTime);

        this.tag = Maleficus.Consts.TAG_PLAYER;
        IsReadyToShoot = true;
        IsUnhittable = false;
    }
}
