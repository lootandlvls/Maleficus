using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManagerOld : AbstractSingletonManager<EventManagerOld> {

    public event Action<ItemBehavior, int> ItemTaken;

    public void OnItemTaken(ItemBehavior takenItem, int playerID)
    {
        if (ItemTaken != null)
        {
            ItemTaken.Invoke(takenItem, playerID);
        }
    }

}
