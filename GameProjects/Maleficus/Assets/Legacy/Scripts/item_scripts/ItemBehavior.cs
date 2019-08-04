using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    potion_fast,
    potion_mini,
    potion_shield,
    potion_strength,
    flash_gordon,
    knack,
    inkling,
    bomb
}

public class ItemBehavior : MonoBehaviour {

    public ItemType itemType;
    private Rigidbody rby;
    // test if item already on floor
    bool isGrounded = false;

	// Use this for initialization
	void Start () {
        rby = GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void Update()
    {
        
        if(this.gameObject.transform.position.y <= 2.82f)
        {
            this.gameObject.transform.Rotate(new Vector3(0, 1, 0), Space.Self);
           
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag.Equals("Ground"))
        {
            isGrounded = true;
        }

        // Notify event manager 
        if (col.gameObject.tag == "Player1")
        {
            EventManagerOld.Instance.OnItemTaken(this, 1);
        }
        else if (col.gameObject.tag == "Player2")
        {
            EventManagerOld.Instance.OnItemTaken(this, 2);
        }
        else if (col.gameObject.tag == "Player3")
        {
            EventManagerOld.Instance.OnItemTaken(this, 3);
        }
        else if (col.gameObject.tag == "Player4")
        {
            EventManagerOld.Instance.OnItemTaken(this, 4);
        }

        if (col.gameObject.layer == 10)
        {
            Physics.IgnoreLayerCollision(10, 10, true);
            if (isGrounded)
            {
                Destroy(this.gameObject);
            }
        }
        
    }
}

