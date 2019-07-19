using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICoins : MonoBehaviour
{


   
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("player triggered");
        Player player = other.GetComponent<Player>();
        if (player!= null && AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {

            EventManager.Instance.Invoke_PLAYERS_PlayerCollectedCoin();
            Destroy(this.gameObject);
        }

        
    }
}
