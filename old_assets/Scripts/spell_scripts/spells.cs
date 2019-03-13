using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spells : MonoBehaviour {

    public GameObject fireball;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if (Input.GetKeyDown("a"))
        {
            Instantiate(fireball , transform.position,transform.rotation ,this.transform);

        }
	}
}
