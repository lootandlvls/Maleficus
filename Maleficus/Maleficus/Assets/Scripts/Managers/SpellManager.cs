using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpellManager : AbstractSingletonManager<SpellManager>
{
    Dictionary<EPlayerID, Player> activePlayers = new Dictionary<EPlayerID, Player>();

    [SerializeField] private GameObject FrozenEffect;
    [SerializeField] private float friction;

    public List<AbstractSpell> SpellsUpgrade = new List<AbstractSpell>();
    public List<GameObject> ChargingSpells_Effects = new List<GameObject>();
    public List<AbstractSpell> All_Spells = new List<AbstractSpell>();

    public Dictionary<EPlayerID, Dictionary<int, AbstractSpell>> Player_Spells = new Dictionary<EPlayerID, Dictionary<int, AbstractSpell>>();
    //  public Dictionary<EPlayerID, List<AbstractSpell>> Player_Spells = new Dictionary<EPlayerID, List<AbstractSpell>>();
    public Dictionary<int, AbstractSpell> Player_1_Spells = new Dictionary<int, AbstractSpell>();
    public Dictionary<int, AbstractSpell> Player_2_Spells = new Dictionary<int, AbstractSpell>();
    public Dictionary<int, AbstractSpell> Player_3_Spells = new Dictionary<int, AbstractSpell>();
    public Dictionary<int, AbstractSpell> Player_4_Spells = new Dictionary<int, AbstractSpell>();
    public List<AbstractSpell> Player_1_SpellsList = new List<AbstractSpell>();
    public List<AbstractSpell> Player_2_SpellsList = new List<AbstractSpell>();
    public List<AbstractSpell> Player_3_SpellsList = new List<AbstractSpell>();
    public List<AbstractSpell> Player_4_SpellsList = new List<AbstractSpell>();

    private Dictionary<ETouchJoystickType, MaleficusJoystick> spellJoysticks = new Dictionary<ETouchJoystickType, MaleficusJoystick>();

    protected override void Awake()
    {
        base.Awake();

        LoadSpellResources();
        LoadEffectsResources();
        InitializeSpells();
    }

    private void Start()
    {
        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;
        EventManager.Instance.SPELLS_Teleport += On__SPELLS_Teleport;


        activePlayers = PlayerManager.Instance.ActivePlayers;
    }

    public override void Initialize()
    {
        // Find touch joysticks
        spellJoysticks.Clear();
        foreach (MaleficusJoystick joystick in FindObjectsOfType<MaleficusJoystick>())
        {
            if (spellJoysticks.ContainsKey(joystick.JoystickType) == false)
            {
                spellJoysticks.Add(joystick.JoystickType, joystick);
            }
        }
    }

    private void On__SPELLS_Teleport(ISpell castedSpell, EPlayerID castingPlayerID)
    {
        Debug.Log("Teleportation spell executed");
        float InputH = Input.GetAxis(activePlayers[castingPlayerID].playerHorizontalInput);
        float InputV = Input.GetAxis(activePlayers[castingPlayerID].playerVerticalInput);
        Debug.Log(InputH + " and " + InputV);
        Vector3 TeleportDirection = new Vector3();
        TeleportDirection.x = InputH * Time.deltaTime * 10;
        TeleportDirection.z = InputV * Time.deltaTime * 10;
        activePlayers[castingPlayerID].transform.position += TeleportDirection * 50;



    }
    private void On_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {
        if (hitInfo.HasPower)
        {
            if (activePlayers[hitInfo.HitPlayerID].PlayerID == hitInfo.HitPlayerID)
            {

                StartCoroutine(PushPlayer(hitInfo));


            }
        }

        foreach (SpellEffects debuffeffect in hitInfo.DebuffEffects)
        {
            ApplyDebuff(debuffeffect, hitInfo.HitPlayerID);
        }

        foreach (SpellEffects buffeffect in hitInfo.DebuffEffects)
        {
            ApplyBuff(buffeffect, hitInfo.HitPlayerID);
        }
    }

    private IEnumerator PushPlayer(SHitInfo hitInfo)
    {

        Rigidbody rgb = activePlayers[hitInfo.HitPlayerID].GetComponent<Rigidbody>();
        Transform playerTransform = activePlayers[hitInfo.HitPlayerID].GetComponent<Transform>();
        rgb.isKinematic = false;
        //  transform.position = Vector3.MoveTowards(activePlayers[hitInfo.HitPlayerID].transform.position, hitInfo.HitVelocity, Time.deltaTime * 2);
        Debug.Log(hitInfo.HitVelocity);
        Vector3 endPushPosition = new Vector3(hitInfo.HitVelocity.x, 0, hitInfo.HitVelocity.z);

        endPushPosition = transform.TransformDirection(endPushPosition);
        playerTransform.position = Vector3.Lerp(playerTransform.position, endPushPosition, Time.deltaTime * friction);
        yield return new WaitForSeconds(0.1f);
        rgb.isKinematic = true;
    }

    private void ApplyDebuff(SpellEffects debuff, EPlayerID playerID)
    {

        switch (debuff)
        {
            case SpellEffects.FROZEN:
                StartCoroutine(PlayerFrozen(playerID));
                break;
            case SpellEffects.STUN:


                break;

            case SpellEffects.SLOWDOWN:
                Debug.Log("Player SLOWED DOWN");

                break;

            case SpellEffects.CHARM:

                break;

        }




    }
    private void ApplyBuff(SpellEffects buff, EPlayerID playerID)
    {
        Debug.Log("Apply Buff on player " + playerID);
        switch (buff)
        {
            case SpellEffects.INCREACE_SPEED:

                break;

            case SpellEffects.INCREASE_CASTING_SPEED:

                Debug.Log("INCREASE_CASTING_SPEED");
                break;

            case SpellEffects.INCREASE_DAMAGE:
                Debug.Log("IINCREASE_DAMAGE");
                break;

            case SpellEffects.INCREASE_OFFENSIVE_SPELL_SIZE:
                Debug.Log("INCREASE_OFFENSIVE_SPELL_SIZE");
                break;
            case SpellEffects.PROTECT:
                Debug.Log("PROTECT");
                break;
            case SpellEffects.REMOVE_DEBUFF:
                Debug.Log("REMOVE_DEBUF");
                break;


        }




    }

    public void CastSpell(EPlayerID playerID, int spell_Input_ID)
    {
        AbstractSpell spellToCast;

        switch (playerID)                                                       // TODO: Switch unecessary. Same code in every case
        {
            case EPlayerID.PLAYER_1:
                                                                                // clean this mess!
                float spellCooldown = Player_Spells[playerID][spell_Input_ID].cooldown;

                switch (spell_Input_ID)
                {
                    case 0:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        spellJoysticks[ETouchJoystickType.SPELL_1].ReloadJoystick(spellCooldown);


                        break;
                    case 1:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        spellJoysticks[ETouchJoystickType.SPELL_2].ReloadJoystick(spellCooldown);

                        break;
                    case 2:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        spellJoysticks[ETouchJoystickType.SPELL_3].ReloadJoystick(spellCooldown);
                        break;
                }
                break;

            case EPlayerID.PLAYER_2:
                switch (spell_Input_ID)
                {
                    case 0:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        break;
                    case 1:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        break;
                    case 2:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        break;
                }
                break;

            case EPlayerID.PLAYER_3:
                switch (spell_Input_ID)
                {
                    case 0:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        break;
                    case 1:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        break;
                    case 2:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        break;
                }
                break;
            case EPlayerID.PLAYER_4:
                switch (spell_Input_ID)
                {
                    case 0:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        break;
                    case 1:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        break;
                    case 2:
                        spellToCast = Player_Spells[playerID][spell_Input_ID];
                        InstantiateSpell(spellToCast, playerID);
                        break;
                }
                break;
        }

    }

    private void InstantiateSpell(AbstractSpell spellToCast, EPlayerID playerID)
    {

        

        //TODO instantiate the spell object here and give it the playerID

        if (spellToCast.GetComponent<AOE>() != null)

        {
            activePlayers[playerID].animator.SetTrigger("shockwave");
            Vector3 position = new Vector3(activePlayers[playerID].transform.position.x, activePlayers[playerID].transform.position.y + 0.01f, activePlayers[playerID].transform.position.z);
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            AbstractSpell spell = Instantiate(spellToCast, position, rotation);
            if ((MotherOfManagers.Instance.IsARGame == true))
            {
                spell.transform.localScale *= ARManager.Instance.SizeFactor;
                spell.hitPower *= (int)spell.transform.localScale.x; ;

            }
            spell.CastingPlayerID = playerID;
            spell.transform.parent = activePlayers[playerID].transform;
            Debug.Log("AOE SPELL CASTED");
            //spell.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;
        }
        else if (spellToCast.GetComponent<Linear_Instant>() != null)

        {

            Transform position = activePlayers[playerID].SpellInitPosition;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            AbstractSpell spell = Instantiate(spellToCast, position.position, rotation);
            if ((MotherOfManagers.Instance.IsARGame == true))
            {
                spell.transform.localScale *= ARManager.Instance.SizeFactor;
                spell.speed *= ARManager.Instance.SizeFactor;
                spell.hitPower *= (int)ARManager.Instance.SizeFactor;
            }
            spell.transform.rotation = activePlayers[playerID].transform.rotation;
            spell.transform.parent = activePlayers[playerID].transform;
            spell.CastingPlayerID = playerID;
            Debug.Log("LINEAR INSTANT SPELL CASTED");
        }
        else if (spellToCast.GetComponent<Teleport>() != null)
        {
            activePlayers[playerID].animator.SetTrigger("teleport");

            Quaternion rotation = activePlayers[playerID].transform.rotation;
            StartCoroutine(animationDelay(spellToCast, playerID, 2));
            Vector3 position = new Vector3(activePlayers[playerID].transform.position.x, activePlayers[playerID].transform.position.y + 0.1f, activePlayers[playerID].transform.position.z);
            AbstractSpell spell = Instantiate(spellToCast, position, transform.rotation);
            if ((MotherOfManagers.Instance.IsARGame == true))
            {
                spell.transform.localScale *= ARManager.Instance.SizeFactor;
                spell.speed *= ARManager.Instance.SizeFactor;
                spell.hitPower *= (int)ARManager.Instance.SizeFactor;
            }
            spell.CastingPlayerID = playerID;
        }
        else if (spellToCast.GetComponent<Linear_Laser>() != null)
        {
            // activePlayers[playerID].animator.SetBool("channeling", true);
            Transform position = activePlayers[playerID].SpellInitPosition;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            AbstractSpell spell = Instantiate(spellToCast, position.position, rotation);

            if ((MotherOfManagers.Instance.IsARGame == true))
            {
                spell.transform.localScale = new Vector3(spell.transform.localScale.x * 0.05f, spell.transform.localScale.y * 0.05f, spell.transform.localScale.z * 0.05f);
                spell.speed *= ARManager.Instance.SizeFactor;
                spell.hitPower *= (int)ARManager.Instance.SizeFactor;
            }
            spell.transform.rotation = activePlayers[playerID].transform.rotation;
            spell.transform.parent = activePlayers[playerID].transform;
            spell.CastingPlayerID = playerID;
            // StartCoroutine(PlayerCantMove());
        }
        else
        {
            Debug.Log("Spell spawned");
            Vector3 position = activePlayers[playerID].transform.position;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            activePlayers[playerID].animator.SetTrigger("projectileAttack");
            StartCoroutine(animationDelay(spellToCast, playerID, 1));
            //   StartCoroutine(spellCharging(spellToCast));
            /* int counter = 1;
             while (counter < 4)
              {
                  if (Input.GetButton("CastSpell_1_A"))
                  {
                     StartCoroutine(spellCharging());
                     counter++;
                  }
                  else
                  {
                      return;
                  }
                  */

            /* AbstractSpell spell = Instantiate(spellToCast, activePlayers[playerID].SpellInitPosition.position, transform.rotation);
               spell.CastingPlayerID = playerID;
               spell.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;
           }*/

            /* AbstractSpell spellUpgrade = spellToCast;
             foreach(AbstractSpell spell in SpellManager.Instance.spellsUpgrade)
            {

                if (spell.SpellName.Equals(spellToCast.SpellName + counter))
                {
                    Debug.Log(spellToCast.SpellName + counter + "has been chosen");
                    spellUpgrade = spell;
                }
            }*/

        }



    }
    private void LoadSpellResources()
    {
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusTypes.PATH_SPELL_AOE_EXPLOSION_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusTypes.PATH_SPELL_FIREBALL_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusTypes.PATH_SPELL_FIREBALL_LVL_2));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusTypes.PATH_SPELL_FIRE_SHOCKBLAST_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusTypes.PATH_SPELL_FIRE__LASER_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusTypes.PATH_SPELL_ICEBALL_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusTypes.PATH_SPELL_PARABOLIC_ENERGY_BALL_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusTypes.PATH_SPELL_TELEPORT_LVL_1));

    }

    private void LoadEffectsResources()
    {
        ChargingSpells_Effects.Add(Resources.Load<GameObject>(MaleficusTypes.PATH_EFFECT_CHARGING_BODYENERGY));
        ChargingSpells_Effects.Add(Resources.Load<GameObject>(MaleficusTypes.PATH_EFFECT_CHARGING_WANDENERGY));
        ChargingSpells_Effects.Add(Resources.Load<GameObject>(MaleficusTypes.PATH_EFFECT_FROZEN));
    }


    private IEnumerator PlayerFrozen(EPlayerID playerID)

    {
        activePlayers[playerID].speed = 0;
        GameObject snowman = Instantiate(FrozenEffect, activePlayers[playerID].transform.position, activePlayers[playerID].transform.rotation);
        activePlayers[playerID].transform.GetChild(2).gameObject.SetActive(false);
        snowman.transform.parent = activePlayers[playerID].transform;
        yield return new WaitForSeconds(1.5f);
        activePlayers[playerID].transform.GetChild(2).gameObject.SetActive(true);
        activePlayers[playerID].speed = 75;
    }

    private IEnumerator PlayerStunned(EPlayerID playerID)
    {
        activePlayers[playerID].speed = 0;
        yield return new WaitForSeconds(2.5f);
        activePlayers[playerID].speed = 75;
    }

    IEnumerator animationDelay(AbstractSpell spellToCast, EPlayerID playerID, int animationID)
    {
        switch (animationID)
        {
            case 1:
                activePlayers[playerID].animator.SetTrigger("projectileAttack");
                break;
            case 2:
                activePlayers[playerID].animator.SetTrigger("teleport");
                break;
        }

        yield return new WaitForSeconds(0.3f);
        AbstractSpell spell = Instantiate(spellToCast, activePlayers[playerID].SpellInitPosition.position, activePlayers[playerID].transform.rotation);
        spell.CastingPlayerID = playerID;
        if ((MotherOfManagers.Instance.IsARGame == true))
        {
            spell.transform.localScale *= ARManager.Instance.SizeFactor;
            spell.speed *= ARManager.Instance.SizeFactor;
            spell.hitPower *= (int)spell.transform.localScale.x; ;
        }

        spell.CastingPlayerID = playerID;
        spell.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;
    }

    private void InitializeSpells()
    {
        for (int i = 0; i < 3; i++)
        {
            Player_1_Spells[i] = Player_1_SpellsList[i];
            Player_2_Spells[i] = Player_2_SpellsList[i];
            Player_3_Spells[i] = Player_3_SpellsList[i];
            Player_4_Spells[i] = Player_4_SpellsList[i];
        }

        Player_Spells[EPlayerID.PLAYER_1] = Player_1_Spells;
        Player_Spells[EPlayerID.PLAYER_2] = Player_2_Spells;
        Player_Spells[EPlayerID.PLAYER_3] = Player_3_Spells;
        Player_Spells[EPlayerID.PLAYER_4] = Player_4_Spells;
    }
}
