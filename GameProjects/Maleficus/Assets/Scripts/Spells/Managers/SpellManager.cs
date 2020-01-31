
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maleficus.Consts;
using static Maleficus.Utils;


public class SpellManager : AbstractSingletonManager<SpellManager>
{
    public List<AbstractSpell> ActiveMovingSpells         { get; private set; } = new List<AbstractSpell>();
    public List<AbstractSpell> SpellsUpgrade        { get { return spellsUpgrade; } }
    public List<GameObject> ChargingSpells_Effects  { get; } = new List<GameObject>();
    public List<AbstractSpell> All_Spells           { get; } = new List<AbstractSpell>();

    [SerializeField] private List<AbstractSpell> spellsUpgrade = new List<AbstractSpell>();
    [SerializeField] private GameObject frozenEffect;
    [SerializeField] private GameObject paralyzeEffect;
    [SerializeField] private GameObject Slash_Effect;

    public Dictionary<EPlayerID, Dictionary<ESpellSlot, AbstractSpell>> playersChosenSpells = new Dictionary<EPlayerID, Dictionary<ESpellSlot, AbstractSpell>>();
    // PlayerManager's activePlayers reference
    private Dictionary<EPlayerID, Player> activePlayers = new Dictionary<EPlayerID, Player>();

    public List<AbstractSpell> DebugSpells_Player1 = new List<AbstractSpell>();
    public List<AbstractSpell> DebugSpells_Player2 = new List<AbstractSpell>();
    public List<AbstractSpell> DebugSpells_Player3 = new List<AbstractSpell>();
    public List<AbstractSpell> DebugSpells_Player4 = new List<AbstractSpell>();

    private Dictionary<ETouchJoystickType, MaleficusJoystick> spellJoysticks = new Dictionary<ETouchJoystickType, MaleficusJoystick>();


    protected override void Awake()
    {
        base.Awake();

        LoadSpellResources();
        LoadEffectsResources();
        InitializeSpellsDictionnary();
    }

    protected override void Start()
    {
        base.Start();

        activePlayers = PlayerManager.Instance.ActivePlayers;
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.SPELLS_SpellHitPlayer             += On_SPELLS_SpellHitPlayer;
        EventManager.Instance.UI_SpellChosen                    += On_UI_SpellChosen;
        EventManager.Instance.UI_SpellRemoved                   += On_UI_SpellRemoved;
        EventManager.Instance.INPUT_ControllerConnected.Event   += On_INPUT_ControllerConnected;
    }

    protected override void OnReinitializeManager()
    {
        base.OnReinitializeManager();

        FinTouochJoysticks();

    }

    protected override void Update()
    {
        base.Update();

        foreach(AbstractSpell spell in ActiveMovingSpells)
        {
            LogCanvas(10, spell.CastingPlayerID + " : " + spell.SpellName);
        }
    }

    private void On_UI_SpellRemoved(EPlayerID playerID, ESpellSlot spellSlot)
    {
        if ((IS_KEY_CONTAINED(playersChosenSpells, playerID))
            && (IS_KEY_CONTAINED(playersChosenSpells[playerID], spellSlot)))
        {
            playersChosenSpells[playerID][spellSlot] = null;
        }
    }

    private void On_UI_SpellChosen( EPlayerID playerID , AbstractSpell chosenSpell , ESpellSlot spellSlot)
    {
        if ((IS_KEY_CONTAINED(playersChosenSpells, playerID))
            && (IS_KEY_CONTAINED(playersChosenSpells[playerID], spellSlot)))
        {
            playersChosenSpells[playerID][spellSlot] = chosenSpell;
        }
    }

    public void On_SPELLS_SpellHitPlayer(SHitInfo hitInfo)
    {
        AbstractSpell spell =  (AbstractSpell) hitInfo.CastedSpell;
        if (IS_NOT_NULL(spell))
        {
            if (spell.HasPushPower)
            {
                if (activePlayers[hitInfo.HitPlayerID].PlayerID == hitInfo.HitPlayerID)
                {
                    PushPlayer(hitInfo);
                }
            }

            foreach (ESpellEffects debuffeffect in spell.DebuffEffects)
            {
                ApplyDebuff(debuffeffect, hitInfo.HitPlayerID , spell.DebuffDuration);
            }

            foreach (ESpellEffects buffeffect in spell.BuffEffects)
            {
                ApplyBuff(buffeffect, hitInfo.HitPlayerID, hitInfo.CastingPlayerID , spell.BuffDuration);
            }
        }
    }

    private void On_INPUT_ControllerConnected(Event_GenericHandle<EControllerID, EPlayerID> eventHandle)
    {
        EControllerID controllerID = eventHandle.Arg1;
        EPlayerID playerID = eventHandle.Arg2;
        if (controllerID.ContainedIn(AI_CONTROLLERS))
        {
            LogConsole("Adding debug spells for AI");
            for (int j = 0; j < 3; j++)
            {
                ESpellSlot spellID = GetSpellSlotFrom(j + 1);
                switch (playerID)
                {
                    case EPlayerID.PLAYER_1:
                        playersChosenSpells[EPlayerID.PLAYER_1][spellID] = DebugSpells_Player1[j];
                        break;

                         case EPlayerID.PLAYER_2:
                        playersChosenSpells[EPlayerID.PLAYER_2][spellID] = DebugSpells_Player2[j];
                        break;

                         case EPlayerID.PLAYER_3:
                        playersChosenSpells[EPlayerID.PLAYER_3][spellID] = DebugSpells_Player3[j];
                        break;

                         case EPlayerID.PLAYER_4:
                        playersChosenSpells[EPlayerID.PLAYER_4][spellID] = DebugSpells_Player4[j];
                        break;
                }
            }
        }
    }


    private void On_SpellNotActiveAnymore(AbstractSpell spell)
    {
        spell.SpellNotActiveAnymore -= On_SpellNotActiveAnymore;

        if (IS_VALUE_CONTAINED(ActiveMovingSpells, spell))
        {
            ActiveMovingSpells.Remove(spell);
        }
    }

    private void PushPlayer(SHitInfo hitInfo)
    {
        if ( hitInfo.CastedSpell.SpellID == ESpellID.BLACK_HOLE)
        {
            activePlayers[hitInfo.HitPlayerID].PushPlayer(-hitInfo.HitVelocity, hitInfo.CastedSpell.PushDuration);
        }
        else
        {
            activePlayers[hitInfo.HitPlayerID].PushPlayer(hitInfo.HitVelocity, hitInfo.CastedSpell.PushDuration);
        }
        

    }

    private void ApplyDebuff(ESpellEffects debuff, EPlayerID playerID , float debuffDuration)
    {
        switch (debuff)
        {
            case ESpellEffects.FROZEN:
                Debug.Log("Player Frozen");
                StartCoroutine(PlayerFrozen(playerID , debuffDuration));
                break;

            case ESpellEffects.STUN:

                break;

            case ESpellEffects.SLOWDOWN:
                Debug.Log("Player Paralyzed");
                StartCoroutine(PlayerParalyzed(playerID , 3 , debuffDuration));
                break;

            case ESpellEffects.CHARM:

                break;

        }
    }

    private void ApplyBuff(ESpellEffects buff, EPlayerID playerID, EPlayerID CastingPlayerID , float buffDuration)
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
        float spellCooldown = playersChosenSpells[playerID][spellID].Cooldown;      
        spellToCast = playersChosenSpells[playerID][spellID];
    
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

        SpawnSpell(spellToCast, playerID);

        //spellJoysticks[ETouchJoystickType.SPELL_1].ReloadJoystick(spellCooldown);         // TODO [Bnjmo]: Readapt this
        EventManager.Instance.Invoke_SPELLS_SpellSpawned(spellToCast, playerID , spellID);
    }

    private void SpawnSpell(AbstractSpell spellToCast, EPlayerID playerID)
    {
        AbstractSpell spawnedSpell = null;
        if (spellToCast.GetComponent<AOE>() != null)
        {
            activePlayers[playerID].DoShockwaveAnimation();
            Vector3 position = new Vector3(activePlayers[playerID].transform.position.x, activePlayers[playerID].transform.position.y + 0.5f, activePlayers[playerID].transform.position.z);
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            spawnedSpell = Instantiate(spellToCast, position, rotation);

            spawnedSpell.CastingPlayerID = playerID;
            spawnedSpell.transform.parent = activePlayers[playerID].transform;
            Debug.Log("AOE SPELL CASTED");
            //spell.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;
        }
        else if (spellToCast.GetComponent<Linear_Explosive>())
        {
            Vector3 position = activePlayers[playerID].transform.position + new Vector3(0, 0.5f, 0);
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            activePlayers[playerID].DoProjectileAttackAnimation();
            StartCoroutine(AnimationDelayCoroutine(spellToCast, playerID, 1));
        }
        else if (spellToCast.GetComponent<Linear_Instant>() != null)
        {
            Vector3 position = activePlayers[playerID].SpellInitPosition;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            activePlayers[playerID].DoShockwaveAnimation();
            spawnedSpell = Instantiate(spellToCast, position, rotation);

            spawnedSpell.transform.rotation = activePlayers[playerID].transform.rotation;
            spawnedSpell.transform.parent = activePlayers[playerID].transform;
            spawnedSpell.CastingPlayerID = playerID;
            Debug.Log("LINEAR INSTANT SPELL CASTED");
        }
        else if (spellToCast.GetComponent<Linear_Wave>() != null)
        {
            Vector3 position = activePlayers[playerID].transform.position;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            activePlayers[playerID].DoProjectileAttackAnimation();
            StartCoroutine(AnimationDelayCoroutine(spellToCast, playerID, 1));
        }
        else if (spellToCast.GetComponent<Teleport>() != null)
        {
            activePlayers[playerID].DoTeleportAnimation();
            Quaternion rotation = activePlayers[playerID].transform.rotation;
         //   StartCoroutine(AnimationDelayCoroutine(spellToCast, playerID, 2));
            Vector3 position = new Vector3(activePlayers[playerID].transform.position.x, activePlayers[playerID].transform.position.y + 0.1f, activePlayers[playerID].transform.position.z);
            AbstractSpell spell = Instantiate(spellToCast, position, transform.rotation);
            spell.CastingPlayerID = playerID;
            
        }
        else if (spellToCast.GetComponent<Linear_Laser>() != null)
        {
            // activePlayers[playerID].animator.SetBool("channeling", true);
            Vector3 position = activePlayers[playerID].SpellInitPosition;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            spawnedSpell = Instantiate(spellToCast, position, rotation);

            spawnedSpell.transform.rotation = activePlayers[playerID].transform.rotation;
            spawnedSpell.transform.parent = activePlayers[playerID].transform;
            spawnedSpell.CastingPlayerID = playerID;
            //  StartCoroutine(PlayerCantMove());
        }
        else if (spellToCast.GetComponent<Traps>() != null)
        {
            Vector3 position = activePlayers[playerID].SpellInitPosition;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            spawnedSpell = Instantiate(spellToCast, position, rotation);
            spawnedSpell.CastingPlayerID = playerID;
        }
        else if (spellToCast.GetComponent<Shield>() != null)
        {
            Vector3 position = activePlayers[playerID].transform.position + new Vector3(0, 0.5f, 0);
            Quaternion rotation = activePlayers[playerID].transform.rotation;
            spawnedSpell = Instantiate(spellToCast, position, rotation);
            spawnedSpell.CastingPlayerID = playerID;
            spawnedSpell.transform.parent = activePlayers[playerID].transform;
            activePlayers[playerID].ActivateShield(spawnedSpell.SpellDuration);
        }
        else if (spellToCast.GetComponent<Linear_Hit>())
        {
            if (spellToCast.SpellID == ESpellID.RAPID_FIRE_PLASMA)
            {
                StartCoroutine(DelayBetweenMultipleSpellCastingCoroutine(0.05f, playerID, spellToCast));
            }
            else if (spellToCast.SpellID == ESpellID.AIR_SLASH)
            {
                Vector3 position = activePlayers[playerID].SpellInitPosition;
                Quaternion rotation = activePlayers[playerID].transform.rotation;
                activePlayers[playerID].DoShockwaveAnimation();
                Instantiate(Slash_Effect, position, transform.rotation);
                spawnedSpell = Instantiate(spellToCast, position, rotation);

                spawnedSpell.transform.rotation = activePlayers[playerID].transform.rotation;
                //  spawnedSpell.transform.parent = activePlayers[playerID].transform;
                spawnedSpell.CastingPlayerID = playerID;
                Debug.Log("LINEAR WAVE SPELL CASTED");
            }
            else
            {
                Vector3 position = activePlayers[playerID].transform.position;
                Quaternion rotation = activePlayers[playerID].transform.rotation;
                activePlayers[playerID].DoProjectileAttackAnimation();
                StartCoroutine(AnimationDelayCoroutine(spellToCast, playerID, 1));
            }
        }

        activePlayers[playerID].ResetSpellChargingLVL();
    }

    private void AddSpellToActiveMovingSpells(AbstractSpell spawnedSpell)
    {
        if (spawnedSpell != null)
        {
            ActiveMovingSpells.Add(spawnedSpell);
            spawnedSpell.SpellNotActiveAnymore += On_SpellNotActiveAnymore;
        }
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
        if (IS_KEY_CONTAINED(playersChosenSpells, playerID))
        {
            return !playersChosenSpells[playerID].ContainsValue(chosenSpell);
        }
        return true;
    }

    //BUFFS
    private IEnumerator PlayerSpeedBoost(EPlayerID playerID , int buffDuration)
    {
        activePlayers[playerID].SetPlayerSpeedBoost(buffDuration);

        yield return new WaitForSeconds(buffDuration);

        activePlayers[playerID].SetPlayerSpeedBoost(1);
    }

    //DEBUFFS

    private IEnumerator PlayerFrozen(EPlayerID playerID , float debuffDuration)
    {
        if (activePlayers.ContainsKey(playerID))
        {
            activePlayers[playerID].SetPlayerFrozen(true);
            GameObject snowman = Instantiate(frozenEffect, activePlayers[playerID].transform.position, activePlayers[playerID].transform.rotation);
            activePlayers[playerID].transform.GetChild(2).gameObject.SetActive(false);
            snowman.transform.parent = activePlayers[playerID].transform;

            yield return new WaitForSeconds(debuffDuration);
            Destroy(snowman);
            activePlayers[playerID].SetPlayerFrozen(false);
            activePlayers[playerID].transform.GetChild(2).gameObject.SetActive(true);
           
        }

            
    }

    private IEnumerator PlayerStunned(EPlayerID playerID)
    {
        if (activePlayers.ContainsKey(playerID))
        {
            activePlayers[playerID].SetPlayerStunned(true);

            yield return new WaitForSeconds(2.5f);

            activePlayers[playerID].SetPlayerStunned(false);
        }
    }

    private IEnumerator PlayerParalyzed(EPlayerID playerID , float effectStrenght , float duration)
    {
        if (activePlayers.ContainsKey(playerID))
        {
            activePlayers[playerID].SetPlayerParalyzed(true , effectStrenght);
        Vector3 position = new Vector3(activePlayers[playerID].transform.position.x, activePlayers[playerID].transform.position.y +1 , activePlayers[playerID].transform.position.z);
        GameObject ParalyzeEffect = Instantiate(paralyzeEffect, position, activePlayers[playerID].transform.rotation);
        ParalyzeEffect.transform.parent = activePlayers[playerID].transform;
        yield return new WaitForSeconds(duration);
            Destroy(ParalyzeEffect);
            if (activePlayers.ContainsKey(playerID))
            {
                activePlayers[playerID].SetPlayerParalyzed(false, effectStrenght);
            }
        }
        
    }
    //USED FOR RAPIDFIRE SPELLS
    IEnumerator DelayBetweenMultipleSpellCastingCoroutine(float delay , EPlayerID playerID , AbstractSpell spell)
    {
        activePlayers[playerID].DoProjectileAttackAnimation();
        for (int i = 0; i <= spell.SpellDuration; i++)
        {
            Vector3 position = activePlayers[playerID].transform.position;
            Quaternion rotation = activePlayers[playerID].transform.rotation;
           
            AbstractSpell spellToCast = Instantiate(spell, activePlayers[playerID].SpellInitPosition, activePlayers[playerID].transform.rotation);
            spellToCast.CastingPlayerID = playerID;
            AddSpellToActiveMovingSpells(spellToCast);

            yield return new WaitForSeconds(delay);
            


        }
    }

    IEnumerator AnimationDelayCoroutine(AbstractSpell spellToCast, EPlayerID playerID, int animationID)
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
            AddSpellToActiveMovingSpells(spell);
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
        AddSpellToActiveMovingSpells(cast_1);

        AbstractSpell cast_2 = Instantiate(spellToCast, activePlayers[playerID].SpellInitPosition, rotation_2);
        cast_2.CastingPlayerID = playerID;
        cast_2.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;
        AddSpellToActiveMovingSpells(cast_2);

        AbstractSpell cast_3 = Instantiate(spellToCast, activePlayers[playerID].SpellInitPosition, rotation_3);
        cast_3.CastingPlayerID = playerID;
        cast_3.parabolicSpell_EndPosition = activePlayers[playerID].SpellEndPosition;
        AddSpellToActiveMovingSpells(cast_3);
    }

    private void InitializeSpellsDictionnary()
    {
        // Initialize dictionnary with keys
        playersChosenSpells = new Dictionary<EPlayerID, Dictionary<ESpellSlot, AbstractSpell>>();
        foreach (EPlayerID playerID in Enum.GetValues(typeof(EPlayerID)))
        {
            if (playerID != EPlayerID.NONE)
            {
                playersChosenSpells.Add(playerID, new Dictionary<ESpellSlot, AbstractSpell>());
                foreach (ESpellSlot spellSlot in Enum.GetValues(typeof(ESpellSlot)))
                {
                    if (spellSlot != ESpellSlot.NONE)
                    {
                        playersChosenSpells[playerID].Add(spellSlot, null);
                    }
                }
            }
        }

        // Load debug spells
        if (MotherOfManagers.Instance.IsLoadDebugSpells == true)
        {
            for (int j = 0; j < 3; j++)
            {
                ESpellSlot spellID = GetSpellSlotFrom(j + 1);

                playersChosenSpells[EPlayerID.PLAYER_1][spellID] = DebugSpells_Player1[j];
                playersChosenSpells[EPlayerID.PLAYER_2][spellID] = DebugSpells_Player2[j];
                playersChosenSpells[EPlayerID.PLAYER_3][spellID] = DebugSpells_Player3[j];
                playersChosenSpells[EPlayerID.PLAYER_4][spellID] = DebugSpells_Player4[j];
            }
        }
    }

    private void FinTouochJoysticks()
    {
        spellJoysticks.Clear();
        foreach (MaleficusJoystick joystick in FindObjectsOfType<MaleficusJoystick>())
        {
            if (spellJoysticks.ContainsKey(joystick.JoystickType) == false)
            {
                spellJoysticks.Add(joystick.JoystickType, joystick);
            }
        }
    }

    /// <summary>
    /// Gets the chosen spell for the given PlayerID and spellSlot. Returns null if none is chosen!
    /// </summary>
    public AbstractSpell GetChosenSpell(EPlayerID playerID, ESpellSlot spellSlot)
    {
        if ((IS_KEY_CONTAINED(playersChosenSpells, playerID))
            && (IS_KEY_CONTAINED(playersChosenSpells[playerID], spellSlot)))
        {
            return playersChosenSpells[playerID][spellSlot];
        }
        return null;
    }
}
