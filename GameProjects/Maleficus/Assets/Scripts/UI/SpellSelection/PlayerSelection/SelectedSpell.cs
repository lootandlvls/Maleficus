using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSpell : BNJMOBehaviour
{
    public ESpellSlot SpellSlot                 { get { return spellSlot; } }
    public bool IsSelected                      { get { return CurrentSelectedSpell != null; } }
    public AbstractSpell CurrentSelectedSpell   { get; private set; }

    [SerializeField] ESpellSlot spellSlot;
    [SerializeField] Text skillPointIndicator;

    private Image spellIcon;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        spellIcon = GetComponentWithCheck<Image>();
    }



    public void SelectSpell(AbstractSpell spell)
    {
        if (IS_NOT_NULL(spell))
        {
            spellIcon.sprite = spell.SpellIcon;
            CurrentSelectedSpell = spell;
            skillPointIndicator.text = spell.SkillPoint.ToString();
        }
    }

    public void RemoveSpell()
    {
        spellIcon.sprite = null;
        CurrentSelectedSpell = null;
        skillPointIndicator.text = "";
    }
}
