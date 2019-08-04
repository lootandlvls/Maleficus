using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour {
	public bool INFINITE_LOOP = false; //if true, then duration is ignored
	public bool DESTROY_ON_END = false;
	public float duration = 5.5f;
	public ParticleSystem[] all_particles;
	private float start_time = -100.0f;
	private bool GO = true;

	// Use this for initialization
	void Start () 
	{
		start_time = Time.time;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!INFINITE_LOOP && duration + start_time <= Time.time && GO)
			StopAnimation ();
	}

	void StopAnimation()
	{
		for(int i = 0; i < all_particles.Length; i++)
		{
			all_particles [i].Stop ();
		}
		GO = false;
		if (DESTROY_ON_END) 
			Destroy (gameObject, 0.175f); //a little time before destroying, to let remaining particles die first.				
	}

	void PlayAnimation()
	{
		for(int i = 0; i < all_particles.Length; i++)
		{
			all_particles [i].Play ();
		}
		start_time = Time.time;	
		GO = true;
	}
}