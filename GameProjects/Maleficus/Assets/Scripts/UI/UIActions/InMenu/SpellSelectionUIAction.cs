using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSelectionUIAction : AbstractUIAction
{
    public override void OnHighlighted()
    {
        base.OnHighlighted();

        Debug.Log(gameObject.name + " highlighted!");
    }
}
