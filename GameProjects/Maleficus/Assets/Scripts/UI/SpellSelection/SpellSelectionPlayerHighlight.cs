using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSelectionPlayerHighlight : MaleficusMonoBehaviour
{
    public EPlayerID PlayerID { get { return playerID; } }

    [SerializeField] private EPlayerID playerID = EPlayerID.NONE;


}
