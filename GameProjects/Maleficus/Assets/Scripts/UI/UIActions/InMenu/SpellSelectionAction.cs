using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSelectionAction : AbstractUIAction
{
    public AbstractSpell SpellSelection { get { return spellSelection; } }
    Image spellImage;
    [SerializeField] private AbstractSpell spellSelection;

    private void Start()
    {
        base.Awake();
        spellImage = this.GetComponent<Image>();
        if (spellSelection.SpellIcon != null)
        {
            spellImage.sprite = spellSelection.SpellIcon;
        }
    }

} 
