using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : AbstractSpell
{
    AbstractSpell castedSpell;
    EPlayerID playerID;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Debug.Log("Teleportation spell casted");
        castedSpell = this.GetComponent<AbstractSpell>();
        playerID = castedSpell.CastingPlayerID;
        EventManager.Instance.Invoke_SPELLS_Teleport(castedSpell, playerID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
