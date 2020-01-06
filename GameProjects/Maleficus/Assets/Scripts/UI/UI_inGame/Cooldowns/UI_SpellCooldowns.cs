using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_SpellCooldowns : MaleficusMonoBehaviour
{

    private Image SpellIcon;
    public ESpellSlot SpellSlot { get { return spellSlot; } }
    [SerializeField] EPlayerID playerID;
    [SerializeField] ESpellSlot spellSlot;

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();
        EventManager.Instance.SPELLS_SpellSpawned += On_SpellSpawned;
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        
        
    }
    // Start is called before the first frame update


    private void On_SpellSpawned(ISpell spellCasted, EPlayerID playerID)
    {
       
    }
}
