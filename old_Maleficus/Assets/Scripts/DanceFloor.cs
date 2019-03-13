using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceFloor : MonoBehaviour {


    [SerializeField]
    private Transform trans;
    public float limit = 1500;
    public float scale;
    public float rate ;
    private int rand;
    public GameObject frozenFloor;
    private bool readyToShrink = false;
    public float timeToshrink ;
    private bool inCoroutine = true;
    // Use this for initialization
    void Start () {
        

        trans = GetComponent<Transform>();
        scale = trans.transform.localScale.x;

        EventManager.Instance.ItemTaken += OnItemTaken;
	}

    private void OnItemTaken(ItemBehavior item, int playerID)
    {
        Debug.Log("I'm the floor! The item " + item.name + " has been taken by player " + playerID);
        if (item.name.Equals("inkling(Clone)"))
        {
            Debug.Log("Frozen Floor activated");
            StartCoroutine(cd());
        }
       if (item.name.Equals("knack(Clone)"))
        {
            rand = 0;
          //  rand = Random.Range(0, 2);
            Debug.Log("random number is :" + rand);
            switch (rand)
            {
                case 0:
                    limit -= 200;
                    break;
                case 1:
                    limit += 200;
                    break;
            }
            Debug.Log("Knack item taken");
            inCoroutine = false;
            
        }

    }

    IEnumerator ShrinkGround()
    {
        inCoroutine = true;
        Debug.Log("not shrinking");
      //  yield return new WaitForSeconds(10);
        Debug.Log("shrinking");
        readyToShrink = true;
        
        yield return new WaitForSeconds(timeToshrink);
        Debug.Log("not shrinking");
        readyToShrink = false;
        inCoroutine = false;

    }
    IEnumerator cd()
    {
        frozenFloor.SetActive(true);
        yield return new WaitForSeconds(7);
        frozenFloor.SetActive(false);
    }
    // Update is called once per frame
    void Update () {
       
		if ( readyToShrink)
        {

            switch (rand) {
                case 0:
                    if (scale > limit)
                    { 
                    scale -= rate;
                   }
                    break;
                case 1:
                    if (scale < limit)
                    {
                        scale += rate;
                    }
                    break;                  
        }
            trans.transform.localScale = new Vector3(scale, scale, trans.localScale.z);



        }
        if (!inCoroutine)
        {
            StartCoroutine(ShrinkGround());
        }
	}
}
