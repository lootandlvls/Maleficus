using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSpell : BNJMOBehaviour
{

     public  ESpellSlot SpellSlot { get { return spellSlot; } }

    [SerializeField] ESpellSlot spellSlot;
    

    private Image spellIcon;
    private int counter = 0;


    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        spellIcon = GetComponentWithCheck<Image>();
    }


    
    public void ChangeImage(AbstractSpell spell)
    {
        spellIcon.sprite = spell.SpellIcon;
    }
    public void RemoveImage()
    {
        spellIcon.sprite = null;
    }

    //private void OnSpellButtonPressed(EPlayerID playerID, AbstractSpell chosenSpell)
    //{
    //    Debug.Log("EVENT CALLED " + "Counter = " + counter);
           
    //        if (counter == 0 && spellSlot == ESpellSlot.SPELL_1 && player == playerID)
    //        {
    //            spellIcon.sprite = chosenSpell.SpellIcon;
                
    //        EventManager.Instance.Invoke_UI_SpellChosen(playerID, chosenSpell, spellSlot);        
    //        Debug.Log("Spell 1 has been CHosen");
    //    }
    //        if (counter == 1 && spellSlot == ESpellSlot.SPELL_2 && player == playerID)
    //        {
    //            spellIcon.sprite = chosenSpell.SpellIcon;
            
    //        EventManager.Instance.Invoke_UI_SpellChosen(playerID, chosenSpell, spellSlot);
    //        Debug.Log("Spell 1 has been CHosen");
    //    }
    //        if (counter == 2 && spellSlot == ESpellSlot.SPELL_3 && player == playerID)
    //        {
    //            spellIcon.sprite = chosenSpell.SpellIcon;
           
    //        EventManager.Instance.Invoke_UI_SpellChosen(playerID, chosenSpell, spellSlot);
    //        Debug.Log("Spell 1 has been CHosen");
    //    }
       
    //    counter++;
        


   // }
  
}
