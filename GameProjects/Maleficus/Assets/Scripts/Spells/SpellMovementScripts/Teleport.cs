using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : AbstractSpell
{
  
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

          
        EventManager.Instance.Invoke_SPELLS_Teleport(HitPower, CastingPlayerID);
       
    }
}
