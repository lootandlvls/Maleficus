using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptActivator : MonoBehaviour {


    // this script will reactivate  the player control script if it has beed deactivated

    PlayerControl playerControl;
   public  PsController  p1;
   public Ps2Controller p2;
  public  Ps3Controller p3;
   public  Ps4Controller p4;
    // Use this for initialization
    void Start()
    {
        if (this.tag == "Player2")
        { 
        p2 = this.GetComponent<Ps2Controller>();
          }

        if (this.tag == "Player1")
        {
            p1 = this.GetComponent<PsController>();
        }
        if (this.tag == "Player3")
        {
            p3 = this.GetComponent<Ps3Controller>();
        }
        if (this.tag == "Player4")
        {
            p4 = this.GetComponent<Ps4Controller>();
        }
    }
	


	// Update is called once per frame
	void Update () {

        if (this.tag == "Player2" && p2.enabled == false)
        {
            StartCoroutine(wait2());
        }

        if (this.tag == "Player1" && p1.enabled == false)
        {
            StartCoroutine(wait());
        }
        if (this.tag == "Player3" && p3.enabled == false)
        {
            StartCoroutine(wait3());
        }
        if (this.tag == "Player4" && p4.enabled == false)
        {
            StartCoroutine(wait4());
        }

    }


    IEnumerator wait() {
        Debug.Log("script false");
        
        yield return new WaitForSeconds(0.5f);
        p1.enabled = true;
        Debug.Log("script true");
    }
    IEnumerator wait2()
    {
        Debug.Log("script false");

        yield return new WaitForSeconds(0.5f);
        p2.enabled = true;
        Debug.Log("script true");
    }
    IEnumerator wait3()
    {
        Debug.Log("script false");

        yield return new WaitForSeconds(0.5f);
        p3.enabled = true;
        Debug.Log("script true");
    }
    IEnumerator wait4()
    {
        Debug.Log("script false");

        yield return new WaitForSeconds(0.5f);
        p4.enabled = true;
        Debug.Log("script true");
    }

}
