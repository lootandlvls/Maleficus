using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour {

    List<int> assignedPlayerControllers = new List<int>();
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 1; i <= 4; i++)
        {
           
           if ( Input.GetButtonDown("J" + i + "X")) {
                AddController(i);
                Debug.Log("Player " + i + " has been Added");
            }

        }


    }



    void AddController (int controller)
    {

    }
}
