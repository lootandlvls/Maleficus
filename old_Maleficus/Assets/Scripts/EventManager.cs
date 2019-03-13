using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : Singleton<EventManager> {

    public event Action<ItemBehavior, int> ItemTaken;


    public void OnItemTaken(ItemBehavior takenItem, int playerID)
    {
        if (ItemTaken != null)
        {
            ItemTaken.Invoke(takenItem, playerID);
        }
    }

}
