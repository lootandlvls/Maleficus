using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSelectionUIAction : AbstractUIAction
{
    public EPlayerID PLayerID { get; set; }

    public AbstractSpell Spell          { get { return GetComponentWithCheck<SpellSelectionButton>().Spell; } }

    public override void OnHighlighted()
    {
        base.OnHighlighted();

        Debug.Log(gameObject.name + " highlighted!");
    }
}
