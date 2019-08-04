using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireBall : MonoBehaviour {
	IEnumerator time(){
		yield return new WaitForSeconds (8f);
		Destroy (gameObject);
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		StartCoroutine (time ());
	}

void	OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" ) {
			other.SendMessage ("ApplyDamage", 10f, SendMessageOptions.DontRequireReceiver);
			Destroy (gameObject);
		}
	}
}
