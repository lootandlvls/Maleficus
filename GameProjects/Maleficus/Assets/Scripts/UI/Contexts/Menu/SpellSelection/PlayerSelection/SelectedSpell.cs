using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSpell : BNJMOBehaviour
{
    public ESpellSlot SpellSlot                 { get { return spellSlot; } }
    public bool IsSelected                      { get { return CurrentSelectedSpell != null; } }
    public AbstractSpell CurrentSelectedSpell   { get; private set; }


    [SerializeField] private ESpellSlot spellSlot;
    [SerializeField] private Text skillPointIndicator;
    [SerializeField] Image spellIcon;
    [SerializeField] Sprite emptySpellIcon;

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
        spellIcon.sprite = emptySpellIcon;
        CurrentSelectedSpell = null;
        skillPointIndicator.text = "";
    }
}
