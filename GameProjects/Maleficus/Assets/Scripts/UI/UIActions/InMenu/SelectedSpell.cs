using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSpell : MonoBehaviour
{
   [SerializeField] ESpellSlot spellSlot;

    private Image spellIcon;
    private int counter = 0;
  
    // Start is called before the first frame update
    void Start()
    {
        spellIcon = this.GetComponent<Image>();
        EventManager.Instance.UI_SpellChosen += On_UI_SpellChosen;
    }


    private void On_UI_SpellChosen(AbstractSpell chosenSpell)
    {
        
            counter++;
            if (counter == 1 && spellSlot == ESpellSlot.SPELL_1)
            {
                spellIcon.sprite = chosenSpell.SpellIcon;
            }
            if (counter == 2 && spellSlot == ESpellSlot.SPELL_2)
            {
                spellIcon.sprite = chosenSpell.SpellIcon;
            }
            if (counter == 3 && spellSlot == ESpellSlot.SPELL_3)
            {
                spellIcon.sprite = chosenSpell.SpellIcon;
            }
            if ( spellSlot == ESpellSlot.NONE)
            {
                spellIcon.sprite = chosenSpell.SpellIcon;
            }
        
    }
  
}
