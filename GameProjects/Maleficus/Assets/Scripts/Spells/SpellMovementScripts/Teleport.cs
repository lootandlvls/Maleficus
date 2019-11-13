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

        
        castedSpell = this.GetComponent<AbstractSpell>();
        playerID = castedSpell.CastingPlayerID;
        Debug.Log("player "+ Maleficus.MaleficusUtilities.PlayerIDToInt(playerID));
        EventManager.Instance.Invoke_SPELLS_Teleport(castedSpell, playerID);
        Debug.Log("Teleportation spell casted");
    }
}
