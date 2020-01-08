using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.MaleficusConsts;
using static Maleficus.MaleficusUtilities;


public class SpellManager : AbstractSingletonManager<SpellManager>
{

    public List<AbstractSpell> SpellsUpgrade { get { return spellsUpgrade; } }
    public List<GameObject> ChargingSpells_Effects { get; } = new List<GameObject>();
    public List<AbstractSpell> All_Spells { get; } = new List<AbstractSpell>();

    public Dictionary<EPlayerID, Dictionary<ESpellSlot, AbstractSpell>> Player_Spells = new Dictionary<EPlayerID, Dictionary<ESpellSlot, AbstractSpell>>();
    [SerializeField] private List<AbstractSpell> spellsUpgrade = new List<AbstractSpell>();
    [SerializeField] private float friction;
    //Spell Effects

    [SerializeField] private GameObject frozenEffect;
    [SerializeField] private GameObject paralyzeEffect;

    private Dictionary<EPlayerID, Player> activePlayers = new Dictionary<EPlayerID, Player>();

    //  public Dictionary<EPlayerID, List<AbstractSpell>> Player_Spells = new Dictionary<EPlayerID, List<AbstractSpell>>();



    public List<AbstractSpell> Player_1_SpellsList = new List<AbstractSpell>();
    public List<AbstractSpell> Player_2_SpellsList = new List<AbstractSpell>();
    public List<AbstractSpell> Player_3_SpellsList = new List<AbstractSpell>();
    public List<AbstractSpell> Player_4_SpellsList = new List<AbstractSpell>();

    private int Counter = 0;
    private bool temp = false;
    private Dictionary<ETouchJoystickType, MaleficusJoystick> spellJoysticks = new Dictionary<ETouchJoystickType, MaleficusJoystick>();



    protected override void Awake()
    {
        base.Awake();

        LoadSpellResources();
        LoadEffectsResources();

        if (MotherOfManagers.Instance.IsClearDebugSpellLists == true)
        {
            CleaDebugSpellLists();
        }
    }



    protected override void Start()
    {
        base.Start();

        activePlayers = PlayerManager.Instance.ActivePlayers;
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.SPELLS_SpellHitPlayer += On_SPELLS_SpellHitPlayer;
        EventManager.Instance.SPELLS_Teleport       += On__SPELLS_Teleport;
        EventManager.Instance.UI_SpellChosen += On_UI_SpellChosen;
        EventManager.Instance.UI_SpellRemoved += On_UI_SpellRemoved;
    }



    private void On_UI_SpellRemoved(EPlayerID playerID, ESpellSlot spellSlot)
    {
        switch (playerID)
        {
            case EPlayerID.PLAYER_1:
                Player_1_SpellsList[GetIntFrom(spellSlot) - 1] = null;
                break;
            case EPlayerID.PLAYER_2:
                Player_2_SpellsList[GetIntFrom(spellSlot) -1] = null;
                break;
            case EPlayerID.PLAYER_3:
                Player_3_SpellsList[GetIntFrom(spellSlot) -1] = null;
                break;
            case EPlayerID.PLAYER_4:
                Player_4_SpellsList[GetIntFrom(spellSlot) -1] = null;
                break;
        }
    }

    public override void OnSceneStartReinitialize()
    {
        InitializeSpells();

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

    private void On_UI_SpellChosen( EPlayerID playerID , AbstractSpell chosenSpell , ESpellSlot spellSlot)
    {
           if (playerID == EPlayerID.PLAYER_1)
           {           
            Player_1_SpellsList[GetIntFrom(spellSlot)-1] = chosenSpell;
           }
           else if  (playerID == EPlayerID.PLAYER_2)
           {
            Player_2_SpellsList[GetIntFrom(spellSlot)-1] = chosenSpell;
           }
           else if (playerID == EPlayerID.PLAYER_3)
           {
            Player_3_SpellsList[GetIntFrom(spellSlot)-1] = chosenSpell;
           }
           else if (playerID == EPlayerID.PLAYER_4)
           {
            Player_4_SpellsList[GetIntFrom(spellSlot)-1] = chosenSpell;
            }

    }
    private void On__SPELLS_Teleport(ISpell castedSpell, EPlayerID castingPlayerID)
    {
        Debug.Log("Teleportation spell executed");

        JoystickInput playerInput = PlayerManager.Instance.GetPlayerInput(castingPlayerID);

        float InputH = playerInput.JoystickValues[EInputAxis.ROTATE_X];
        float InputV = playerInput.JoystickValues[EInputAxis.ROTATE_Y];

        Vector3 TeleportDirection = activePlayers[castingPlayerID].transform.forward;



        activePlayers[castingPlayerID].transform.position += TeleportDirection * 5;



    }
    private void On_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {
        if (hitInfo.HasPushPower)
        {
            LogConsole("player hit he will be pushed");
            if (activePlayers[hitInfo.HitPlayerID].PlayerID == hitInfo.HitPlayerID )
            {

                PushPlayer(hitInfo);
            }
        }

 
        foreach (ESpellEffects debuffeffect in hitInfo.DebuffEffects)
        {
            Debug.Log("Spell has debuffeffect");
            ApplyDebuff(debuffeffect, hitInfo.HitPlayerID);
        }

        foreach (ESpellEffects buffeffect in hitInfo.BuffEffects)
        {
            ApplyBuff(buffeffect, hitInfo.HitPlayerID , hitInfo.CastingPlayerID);
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
                Debug.Log("Player Frozen");
                StartCoroutine(PlayerFrozen(playerID));

                break;
            case ESpellEffects.STUN:


                break;

            case ESpellEffects.SLOWDOWN:
                Debug.Log("Player Paralyzed");
                StartCoroutine(PlayerParalyzed(playerID , 3));

                break;

            case ESpellEffects.CHARM:

                break;

        }




    }
    private void ApplyBuff(ESpellEffects buff, EPlayerID playerID, EPlayerID CastingPlayerID)
    {
        Debug.Log("Apply Buff on player " + playerID);
        switch (buff)
        {
            case ESpellEffects.INCREACE_SPEED:
                StartCoroutine(PlayerSpeedBoost(CastingPlayerID , 2));
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
        EventManager.Instance.Invoke_SPELLS_SpellSpawned(spellToCast, playerID , spellID);
    }


    private void InstantiateSpell(AbstractSpell spellToCast, EPlayerID playerID)
    {

                                                                            //TODO [Nassim]: instantiate the spell object here and give it the playerID

        if (spellToCast.GetComponent<AOE>() != null)

        {
            activePlayers[playerID].DoShockwaveAnimation();
            Vector3 position = new Vector3(activePlayers[playerID].transform.position.x, activePlayers[playerID].transform.position.y + 0.5f, activePlayers[playerID].transform.position.z);
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            AbstractSpell spell = Instantiate(spellToCast, position, rotation);

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
          //  AbstractSpell spell = Instantiate(spellToCast, position, transform.rotation);
        //    spell.CastingPlayerID = playerID;
        }
        else if (spellToCast.GetComponent<Linear_Laser>() != null)
        {
            // activePlayers[playerID].animator.SetBool("channeling", true);
            Vector3 position = activePlayers[playerID].SpellInitPosition;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            AbstractSpell spell = Instantiate(spellToCast, position, rotation);

            spell.transform.rotation = activePlayers[playerID].transform.rotation;
            spell.transform.parent = activePlayers[playerID].transform;
            spell.CastingPlayerID = playerID;
          //  StartCoroutine(PlayerCantMove());
        }
       else if (spellToCast.GetComponent<Traps>() != null)
        {
            Vector3 position = activePlayers[playerID].SpellInitPosition + new Vector3(0,0,2);
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            AbstractSpell spell = Instantiate(spellToCast, position, rotation);               
            spell.CastingPlayerID = playerID;

            
        }
        else if (spellToCast.GetComponent<Shield>() != null)
        {
            Vector3 position = activePlayers[playerID].transform.position + new Vector3(0, 2, 0);
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            AbstractSpell spell = Instantiate(spellToCast, position, rotation);
            spell.CastingPlayerID = playerID;
            spell.transform.parent = activePlayers[playerID].transform;
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
        All_Spells.Add(Resources.Load<AbstractSpell>(PATH_SPELL_AOE_EXPLOSION_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(PATH_SPELL_FIREBALL_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(PATH_SPELL_FIREBALL_LVL_2));
        All_Spells.Add(Resources.Load<AbstractSpell>(PATH_SPELL_FIRE_SHOCKBLAST_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(PATH_SPELL_FIRE__LASER_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(PATH_SPELL_ICEBALL_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(PATH_SPELL_PARABOLIC_ENERGY_BALL_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(PATH_SPELL_TELEPORT_LVL_1));
        All_Spells.Add(Resources.Load<AbstractSpell>(PATH_SPELL_PLASMA_FISSION_BALLS));
    }

    private void LoadEffectsResources()
    {
        ChargingSpells_Effects.Add(Resources.Load<GameObject>(PATH_EFFECT_CHARGING_BODYENERGY));
        ChargingSpells_Effects.Add(Resources.Load<GameObject>(PATH_EFFECT_CHARGING_WANDENERGY));
        ChargingSpells_Effects.Add(Resources.Load<GameObject>(PATH_EFFECT_FROZEN));
    }

    public bool CheckPlayerSpells(EPlayerID playerID , AbstractSpell chosenSpell)
    {
        switch (playerID)
        {
            case EPlayerID.PLAYER_1:
                return !Player_1_SpellsList.Contains(chosenSpell);
            case EPlayerID.PLAYER_2:
                return !Player_2_SpellsList.Contains(chosenSpell);              
            case EPlayerID.PLAYER_3:
                return !Player_3_SpellsList.Contains(chosenSpell);              
            case EPlayerID.PLAYER_4:
                return !Player_4_SpellsList.Contains(chosenSpell);
                
        }
        
        
        return true;
    }

    //BUFFS
    private IEnumerator PlayerSpeedBoost(EPlayerID playerID , int SpeedBoost)
    {
        activePlayers[playerID].SetPlayerSpeedBoost(SpeedBoost);

        yield return new WaitForSeconds(2.5f);

        activePlayers[playerID].SetPlayerSpeedBoost(1);
    }

    //DEBUFFS

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

    private IEnumerator PlayerParalyzed(EPlayerID playerID , int effectStrenght)
    {
        activePlayers[playerID].SetPlayerParalyzed(true , effectStrenght);
        Vector3 position = new Vector3(activePlayers[playerID].transform.position.x, activePlayers[playerID].transform.position.y +1 , activePlayers[playerID].transform.position.z);
        GameObject ParalyzeEffect = Instantiate(paralyzeEffect, position, activePlayers[playerID].transform.rotation);
        ParalyzeEffect.transform.parent = activePlayers[playerID].transform;
        yield return new WaitForSeconds(2f);
        activePlayers[playerID].SetPlayerParalyzed(false , effectStrenght);
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
        if (spellToCast.IsTripleCast)
        {
            TripleCastSpell(spellToCast, playerID);
        }
      else
        {
             AbstractSpell spell = Instantiate(spellToCast, activePlayers[playerID].SpellInitPosition, activePlayers[playerID].transform.rotation);
             spell.CastingPlayerID = playerID;       
             spell.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;
        }

    }
    //CAST A TRIPLE VERSION OF THE SPELL 
    private void TripleCastSpell(AbstractSpell spellToCast, EPlayerID playerID)
    {
        Vector3 rot1 = activePlayers[playerID].transform.rotation.eulerAngles;
        Vector3 rot2euler = rot1 + new Vector3(0, 30, 0);
        Vector3 rot3euler = rot1 - new Vector3(0, 30, 0);
        Quaternion rotation_2 = Quaternion.Euler(rot2euler);
        Quaternion rotation_3 = Quaternion.Euler(rot3euler);

        AbstractSpell cast_1 = Instantiate(spellToCast, activePlayers[playerID].SpellInitPosition, activePlayers[playerID].transform.rotation);
        cast_1.CastingPlayerID = playerID;
        cast_1.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;

        AbstractSpell cast_2 = Instantiate(spellToCast, activePlayers[playerID].SpellInitPosition, rotation_2);
        cast_2.CastingPlayerID = playerID;
        cast_2.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;

        AbstractSpell cast_3 = Instantiate(spellToCast, activePlayers[playerID].SpellInitPosition, rotation_3);
        cast_3.CastingPlayerID = playerID;
        cast_3.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;
    }

    private void InitializeSpells()
    {
        Player_Spells[EPlayerID.PLAYER_1] = new Dictionary<ESpellSlot, AbstractSpell>();
        Player_Spells[EPlayerID.PLAYER_2] = new Dictionary<ESpellSlot, AbstractSpell>();
        Player_Spells[EPlayerID.PLAYER_3] = new Dictionary<ESpellSlot, AbstractSpell>();
        Player_Spells[EPlayerID.PLAYER_4] = new Dictionary<ESpellSlot, AbstractSpell>();


        for (int j = 0; j < 3; j++)
        {
            ESpellSlot spellID = GetSpellSlotFrom(j + 1);

            Player_Spells[EPlayerID.PLAYER_1][spellID] = Player_1_SpellsList[j];
            Player_Spells[EPlayerID.PLAYER_2][spellID] = Player_2_SpellsList[j];
            Player_Spells[EPlayerID.PLAYER_3][spellID] = Player_3_SpellsList[j];
            Player_Spells[EPlayerID.PLAYER_4][spellID] = Player_4_SpellsList[j];
        }
    }

    private void CleaDebugSpellLists()
    {
        Player_1_SpellsList = new List<AbstractSpell> { null, null, null };
        Player_2_SpellsList = new List<AbstractSpell> { null, null, null };
        Player_3_SpellsList = new List<AbstractSpell> { null, null, null };
        Player_4_SpellsList = new List<AbstractSpell> { null, null, null };
    }
}
