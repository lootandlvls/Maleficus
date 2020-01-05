using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSpell : MonoBehaviour
{
   [SerializeField] ESpellSlot spellSlot;
    [SerializeField] EPlayerID player;

    private Image spellIcon;
    private int counter = 0;
  
    // Start is called before the first frame update
    void Start()
    {
        spellIcon = this.GetComponent<Image>();
        EventManager.Instance.UI_SpellChosen += On_UI_SpellChosen;
    }


    private void On_UI_SpellChosen(EPlayerID playerID, AbstractSpell chosenSpell)
    {
        
            counter++;
            if (counter == 0 && spellSlot == ESpellSlot.SPELL_1 && player == playerID)
            {
                spellIcon.sprite = chosenSpell.SpellIcon;
            }
            if (counter == 1 && spellSlot == ESpellSlot.SPELL_2 && player == playerID)
            {
                spellIcon.sprite = chosenSpell.SpellIcon;
            }
            if (counter == 2 && spellSlot == ESpellSlot.SPELL_3 && player == playerID)
            {
                spellIcon.sprite = chosenSpell.SpellIcon;
            }
           
        
    }
  
}
