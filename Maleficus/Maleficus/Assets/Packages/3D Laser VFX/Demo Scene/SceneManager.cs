using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
	public TextMesh text_fx_name;
	public GameObject[] lasers_prefabs;
	public GameObject[] hits_prefabs;
	public int index_fx = 0;


	private GameObject current_laser_animation;
	private GameObject current_hit_animation;
	private Ray ray;
	private RaycastHit ray_cast_hit;
	private bool FIRING = false;
	private IEnumerator coroutine;


	void Start () {
		text_fx_name.text = "[" + (index_fx + 1) + "] " + lasers_prefabs[ index_fx ].name;
		Destroy(GameObject.Find("Instructions"), 10.5f);
	}


	void Update () {
		if ( Input.GetMouseButtonDown(0) ) // Player wants to fire
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (ray.origin, ray.direction, out ray_cast_hit, 1000f)) 
			{
				if (FIRING == false && ray_cast_hit.transform.name == "Invisible Ground")
					FIRING = true;
				StartLaserAnimation ();
			}

		}
		if ( Input.GetMouseButtonUp(0) ) // Player wants to STROP the firing
		{
			if (FIRING == true)
				FIRING = false;
			StopLaserAnimation ();
		}
		if (FIRING) 
		{
			Aim ();
		}
		//Change-FX Keyboard	
		if ( Input.GetKeyDown("z") || Input.GetKeyDown("left") ){
			index_fx--;
			if(index_fx <= -1)
				index_fx = lasers_prefabs.Length - 1;
			text_fx_name.text = "[" + (index_fx + 1) + "] " + lasers_prefabs[ index_fx ].name;
			if (FIRING) 
			{
				StopLaserAnimation ();
				StartLaserAnimation ();
				StopHitAnimation ();
			}
		}

		if ( Input.GetKeyDown("x") || Input.GetKeyDown("right")){
			index_fx++;
			if(index_fx >= lasers_prefabs.Length)
				index_fx = 0;
			text_fx_name.text = "[" + (index_fx + 1) + "] " + lasers_prefabs[ index_fx ].name;
			if (FIRING) 
			{
				StopLaserAnimation ();
				StartLaserAnimation ();
				StopHitAnimation ();
			}
		}

		if (Input.GetKeyDown ("space"))
			Debug.Break ();
	}


	void Aim()
	{
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if ( Physics.Raycast (ray.origin, ray.direction, out ray_cast_hit, 1000f) ){
			current_laser_animation.transform.LookAt(new Vector3(ray_cast_hit.point.x, 1.0f, ray_cast_hit.point.z));
			current_laser_animation.transform.eulerAngles = new Vector3(current_laser_animation.transform.eulerAngles.x, current_laser_animation.transform.eulerAngles.y - 90.0f, current_laser_animation.transform.eulerAngles.z);
			transform.eulerAngles = current_laser_animation.transform.eulerAngles;
		}
	}


	void StartLaserAnimation()
	{
		current_laser_animation = Instantiate(lasers_prefabs[ index_fx ], transform.position, Quaternion.identity);	
	}


	void StopLaserAnimation()
	{
		current_laser_animation.SendMessage("StopAnimation");
	}


	void StartHitAnimation(Vector3 pos)
	{
		if( current_hit_animation == null )			
			current_hit_animation = Instantiate(hits_prefabs[ index_fx ], pos, Quaternion.identity);	
	}


	void StopHitAnimation()
	{
		if( current_hit_animation != null )
			current_hit_animation.SendMessage("StopAnimation");
		current_hit_animation = null;
	}


	void OnTriggerEnter(Collider other) {
		if (FIRING)
			StartHitAnimation (other.transform.position);
	}


	void OnTriggerStay(Collider other) {
		if (FIRING)
			StartHitAnimation (other.transform.position);
		else
			StopHitAnimation ();
	}


	void OnTriggerExit(Collider other) {
		if (FIRING)
			StopHitAnimation ();
	}
}
