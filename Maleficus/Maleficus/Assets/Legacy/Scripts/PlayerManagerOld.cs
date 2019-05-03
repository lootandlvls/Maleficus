using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerOld : MonoBehaviour {


    List<PsController> assignedPlayerControllers = new List<PsController>();


    public Transform par;

    public GameObject p1;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;

    [SerializeField]
    private PsController player1;
    [SerializeField]
    private Ps2Controller player2;
    [SerializeField]
    private Ps3Controller player3;
    [SerializeField]
    private Ps4Controller player4;


    public bool p1Yes;
    public bool p2Yes;
    public bool p3Yes;
    public bool p4Yes;

    private float p1x = -10f;
    private float p1z = 0f;

    private float p2x = 0f;
    private float p2z = -10f;

    private float p3x = 10f;
    private float p3z = 0f;

    private float p4x = 0f;
    private float p4z = 10f;



	// Use this for initialization
	void Start () {

       





        /* if (p1Yes)
         {
             var p1Instance = Instantiate(p1, new Vector3(p1x, 4f, p1z), Quaternion.Euler(0,90,0)) as GameObject;
             p1Instance.transform.SetParent(par);
             p1Instance.gameObject.transform.Rotate(new Vector3(0,90,0), Space.Self);
         }
         if (p2Yes)
         {
             var p2Instance = Instantiate(p2, new Vector3(p2x, 4f, p2z), Quaternion.Euler(0, 0, 0)) as GameObject;
             p2Instance.transform.SetParent(par);
         }
         if (p3Yes)
         {
             var p3Instance = Instantiate(p3, new Vector3(p3x, 4f, p3z), Quaternion.Euler(0, -90, 0)) as GameObject;
             p3Instance.transform.SetParent(par);
         }
         if (p4Yes) 
         {
             var p4Instance = Instantiate(p4, new Vector3(p4x, 4f, p4z), Quaternion.Euler(0, -180, 0)) as GameObject;
             p4Instance.transform.SetParent(par);
         }*/
    } 
	
	// Update is called once per frame
	void Update () {
        

            if (Input.GetButtonDown("P" + 1 + "X"))
            { 
                Debug.Log("Player " +1  + " has been Added");
            player1.SetControllerNumber(1);
            player1.tag = "Player1";
                //assignedPlayerControllers[i+1].SetControllerNumber(i + 1);
              
            }
        if (Input.GetButtonDown("P" + 2 + "X"))
        {
            Debug.Log("Player " + 2 + " has been Added");
            player2.SetControllerNumber(2);
            player2.tag = "Player2";
            //assignedPlayerControllers[i+1].SetControllerNumber(i + 1);

        }

        if (Input.GetButtonDown("P" + 3 + "X"))
        {
            Debug.Log("Player " + 3 + " has been Added");
            player3.SetControllerNumber(3);
            //assignedPlayerControllers[i+1].SetControllerNumber(i + 1);
            player3.tag = "Player3";
        }

        if (Input.GetButtonDown("P" + 4 + "X"))
        {
            Debug.Log("Player " + 4 + " has been Added");
            player4.SetControllerNumber(4);
            //assignedPlayerControllers[i+1].SetControllerNumber(i + 1);
            player4.tag = "Player4";
        }


    }

    void AddController(int controller)
    {

    }
}
