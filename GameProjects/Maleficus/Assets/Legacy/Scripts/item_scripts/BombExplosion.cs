using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour {

    public GameObject bombExplosion;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag.Equals("Ground"))
        {
            Quaternion rotation = new Quaternion(0,0,0,0);
            Instantiate(bombExplosion, transform.position,rotation );
            Debug.Log("Ground hit");
            Destroy(this.gameObject);
        }
    }
}
