using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpellManager : AbstractSingletonManager<SpellManager>
{
    Dictionary<EPlayerID, Player> activePlayers = new Dictionary<EPlayerID, Player>();





    public List<AbstractSpell> SpellsUpgrade { get { return spellsUpgrade; } }
    public List<GameObject> ChargingSpells_Effects { get { return chargingSpells_Effects; } }
    public List<AbstractSpell> All_Spells { get { return all_Spells; } }

    public Dictionary<EPlayerID, Dictionary<ESpellSlot, AbstractSpell>> Player_Spells = new Dictionary<EPlayerID, Dictionary<ESpellSlot, AbstractSpell>>();

    [SerializeField] private List<AbstractSpell> spellsUpgrade = new List<AbstractSpell>();
    [SerializeField] private GameObject frozenEffect;
    [SerializeField] private float friction;

    private List<GameObject> chargingSpells_Effects = new List<GameObject>();
    private List<AbstractSpell> all_Spells = new List<AbstractSpell>();

    //  public Dictionary<EPlayerID, List<AbstractSpell>> Player_Spells = new Dictionary<EPlayerID, List<AbstractSpell>>();



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

    public override void OnSceneStartReinitialize()
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

        ControllerInput playerInput = PlayerManager.Instance.GetPlayerInput(castingPlayerID);

        float InputH = playerInput.JoystickValues[EInputAxis.MOVE_X];
        float InputV = playerInput.JoystickValues[EInputAxis.MOVE_Y];
        Debug.Log(InputH + " and " + InputV);
        Vector3 TeleportDirection = new Vector3();
        TeleportDirection.x = InputH * Time.deltaTime * 10;
        TeleportDirection.z = InputV * Time.deltaTime * 10;
        activePlayers[castingPlayerID].transform.position += TeleportDirection * 50;



    }
    private void On_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {
        if (hitInfo.HasPushPower)
        {
            if (activePlayers[hitInfo.HitPlayerID].PlayerID == hitInfo.HitPlayerID)
            {
                PushPlayer(hitInfo);
            }
        }

        foreach (ESpellEffects debuffeffect in hitInfo.DebuffEffects)
        {
            ApplyDebuff(debuffeffect, hitInfo.HitPlayerID);
        }

        foreach (ESpellEffects buffeffect in hitInfo.DebuffEffects)
        {
            ApplyBuff(buffeffect, hitInfo.HitPlayerID);
        }
    }

    private void PushPlayer(SHitInfo hitInfo)
    {
        activePlayers[hitInfo.HitPlayerID].PushPlayer(hitInfo.HitVelocity, hitInfo.CastedSpell.PushDuration);
    }

    private void ApplyDebuff(ESpellEffects debuff, EPlayerID playerID)
    {

        switch (debuff)
        {
            case ESpellEffects.FROZEN:
                StartCoroutine(PlayerFrozen(playerID));
                break;
            case ESpellEffects.STUN:


                break;

            case ESpellEffects.SLOWDOWN:
                Debug.Log("Player SLOWED DOWN");

                break;

            case ESpellEffects.CHARM:

                break;

        }




    }
    private void ApplyBuff(ESpellEffects buff, EPlayerID playerID)
    {
        Debug.Log("Apply Buff on player " + playerID);
        switch (buff)
        {
            case ESpellEffects.INCREACE_SPEED:

                break;

            case ESpellEffects.INCREASE_CASTING_SPEED:

                Debug.Log("INCREASE_CASTING_SPEED");
                break;

            case ESpellEffects.INCREASE_DAMAGE:
                Debug.Log("IINCREASE_DAMAGE");
                break;

            case ESpellEffects.INCREASE_OFFENSIVE_SPELL_SIZE:
                Debug.Log("INCREASE_OFFENSIVE_SPELL_SIZE");
                break;
            case ESpellEffects.PROTECT:
                Debug.Log("PROTECT");
                break;
            case ESpellEffects.REMOVE_DEBUFF:
                Debug.Log("REMOVE_DEBUF");
                break;
        }

    }

    public void CastSpell(EPlayerID playerID, ESpellSlot spellID , int spellChargingLVL)
    {
        AbstractSpell spellToCast ;
        float spellCooldown = Player_Spells[playerID][spellID].Cooldown;      
        spellToCast = Player_Spells[playerID][spellID];
    
        if (spellChargingLVL == 2)
        {
            foreach (AbstractSpell upgrade in spellsUpgrade)
            {
                if (upgrade.SpellID == spellToCast.SpellID && upgrade.SpellLevel == 2)
                {
                    spellToCast = upgrade;
                    break;
                }
            }
        }

        InstantiateSpell(spellToCast, playerID);

        //spellJoysticks[ETouchJoystickType.SPELL_1].ReloadJoystick(spellCooldown);         // TODO [Bnjmo]: Readapt this
        EventManager.Instance.Invoke_SPELLS_SpellSpawned(spellToCast, playerID);
    }


    private void InstantiateSpell(AbstractSpell spellToCast, EPlayerID playerID)
    {

                                                                            //TODO [Nassim]: instantiate the spell object here and give it the playerID

        if (spellToCast.GetComponent<AOE>() != null)

        {
            activePlayers[playerID].DoShockwaveAnimation();
            Vector3 position = new Vector3(activePlayers[playerID].transform.position.x, activePlayers[playerID].transform.position.y + 0.01f, activePlayers[playerID].transform.position.z);
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            AbstractSpell spell = Instantiate(spellToCast, position, rotation);

            if ((MotherOfManagers.Instance.IsARGame == true))
            {
                spell.transform.localScale *= ARManager.Instance.SizeFactor;
            }

            spell.CastingPlayerID = playerID;
            spell.transform.parent = activePlayers[playerID].transform;
            Debug.Log("AOE SPELL CASTED");
            //spell.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;
        }
        else if (spellToCast.GetComponent<Linear_Instant>() != null)

        {

            Vector3 position = activePlayers[playerID].SpellInitPosition;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            AbstractSpell spell = Instantiate(spellToCast, position, rotation);
            if ((MotherOfManagers.Instance.IsARGame == true))
            {
                spell.transform.localScale *= ARManager.Instance.SizeFactor;
                spell.speed *= ARManager.Instance.SizeFactor;
            }

            spell.transform.rotation = activePlayers[playerID].transform.rotation;
            spell.transform.parent = activePlayers[playerID].transform;
            spell.CastingPlayerID = playerID;
            Debug.Log("LINEAR INSTANT SPELL CASTED");
        }
        else if (spellToCast.GetComponent<Teleport>() != null)
        {
            activePlayers[playerID].DoTeleportAnimation();

            Quaternion rotation = activePlayers[playerID].transform.rotation;
            StartCoroutine(animationDelay(spellToCast, playerID, 2));
            Vector3 position = new Vector3(activePlayers[playerID].transform.position.x, activePlayers[playerID].transform.position.y + 0.1f, activePlayers[playerID].transform.position.z);
            AbstractSpell spell = Instantiate(spellToCast, position, transform.rotation);
            if ((MotherOfManagers.Instance.IsARGame == true))
            {
                spell.transform.localScale *= ARManager.Instance.SizeFactor;
                spell.speed *= ARManager.Instance.SizeFactor;
            }

            spell.CastingPlayerID = playerID;
        }
        else if (spellToCast.GetComponent<Linear_Laser>() != null)
        {
            // activePlayers[playerID].animator.SetBool("channeling", true);
            Vector3 position = activePlayers[playerID].SpellInitPosition;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            AbstractSpell spell = Instantiate(spellToCast, position, rotation);

            if ((MotherOfManagers.Instance.IsARGame == true))
            {
                spell.transform.localScale = new Vector3(spell.transform.localScale.x * 0.05f, spell.transform.localScale.y * 0.05f, spell.transform.localScale.z * 0.05f);
                spell.speed *= ARManager.Instance.SizeFactor;
            }

            spell.transform.rotation = activePlayers[playerID].transform.rotation;
            spell.transform.parent = activePlayers[playerID].transform;
            spell.CastingPlayerID = playerID;
          //  StartCoroutine(PlayerCantMove());
        }
        else
        {
            Vector3 position = activePlayers[playerID].transform.position;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            activePlayers[playerID].DoProjectileAttackAnimation();
            StartCoroutine(animationDelay(spellToCast, playerID, 1));
        }

        activePlayers[playerID].resetSpellChargingLVL();

    }
    private void LoadSpellResources()
    {
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusConsts.PATH_SPELL_AOE_EXPLOSION_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusConsts.PATH_SPELL_FIREBALL_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusConsts.PATH_SPELL_FIREBALL_LVL_2));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusConsts.PATH_SPELL_FIRE_SHOCKBLAST_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusConsts.PATH_SPELL_FIRE__LASER_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusConsts.PATH_SPELL_ICEBALL_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusConsts.PATH_SPELL_PARABOLIC_ENERGY_BALL_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(MaleficusConsts.PATH_SPELL_TELEPORT_LVL_1));

    }

    private void LoadEffectsResources()
    {
        ChargingSpells_Effects.Add(Resources.Load<GameObject>(MaleficusConsts.PATH_EFFECT_CHARGING_BODYENERGY));
        ChargingSpells_Effects.Add(Resources.Load<GameObject>(MaleficusConsts.PATH_EFFECT_CHARGING_WANDENERGY));
        ChargingSpells_Effects.Add(Resources.Load<GameObject>(MaleficusConsts.PATH_EFFECT_FROZEN));
    }


    private IEnumerator PlayerFrozen(EPlayerID playerID)
    {
        activePlayers[playerID].SetPlayerFrozen(true);
        GameObject snowman = Instantiate(frozenEffect, activePlayers[playerID].transform.position, activePlayers[playerID].transform.rotation);
        activePlayers[playerID].transform.GetChild(2).gameObject.SetActive(false);
        snowman.transform.parent = activePlayers[playerID].transform;

        yield return new WaitForSeconds(1.5f);

        activePlayers[playerID].transform.GetChild(2).gameObject.SetActive(true);
        activePlayers[playerID].SetPlayerFrozen(false);
    }

    private IEnumerator PlayerStunned(EPlayerID playerID)
    {
        activePlayers[playerID].SetPlayerStunned(true);

        yield return new WaitForSeconds(2.5f);

        activePlayers[playerID].SetPlayerStunned(false);
    }

    

    IEnumerator animationDelay(AbstractSpell spellToCast, EPlayerID playerID, int animationID)
    {
        switch (animationID)
        {
            case 1:
                activePlayers[playerID].DoProjectileAttackAnimation();
                break;

            case 2:
                activePlayers[playerID].DoTeleportAnimation();
                break;
        }

        yield return new WaitForSeconds(0.3f);

        AbstractSpell spell = Instantiate(spellToCast, activePlayers[playerID].SpellInitPosition, activePlayers[playerID].transform.rotation);
        spell.CastingPlayerID = playerID;
        if ((MotherOfManagers.Instance.IsARGame == true))
        {
            spell.transform.localScale *= ARManager.Instance.SizeFactor;
            spell.speed *= ARManager.Instance.SizeFactor;
        }

        spell.CastingPlayerID = playerID;
        spell.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;
    }

    private void InitializeSpells()
    {
        Player_Spells[EPlayerID.PLAYER_1] = new Dictionary<ESpellSlot, AbstractSpell>();
        Player_Spells[EPlayerID.PLAYER_2] = new Dictionary<ESpellSlot, AbstractSpell>();
        Player_Spells[EPlayerID.PLAYER_3] = new Dictionary<ESpellSlot, AbstractSpell>();
        Player_Spells[EPlayerID.PLAYER_4] = new Dictionary<ESpellSlot, AbstractSpell>();


        for (int j = 0; j < 3; j++)
        {
            ESpellSlot spellID = MaleficusUtilities.IntToSpellID(j + 1);

            Player_Spells[EPlayerID.PLAYER_1][spellID] = Player_1_SpellsList[j];
            Player_Spells[EPlayerID.PLAYER_2][spellID] = Player_2_SpellsList[j];
            Player_Spells[EPlayerID.PLAYER_3][spellID] = Player_3_SpellsList[j];
            Player_Spells[EPlayerID.PLAYER_4][spellID] = Player_4_SpellsList[j];
        }
    }
}
