using UnityEngine;
using System.Collections;

public class KillParticleSystem : MonoBehaviour {

    ParticleSystem myParitlce;
    AudioSource myAudio;

    void OnEnable()
    {
        myParitlce = GetComponent<ParticleSystem>();
        myAudio = GetComponent<AudioSource>();    
    }

	void Update () 
	{
        if ((myParitlce != null) && (myAudio != null))
        {
            if ((myParitlce.IsAlive() == false) && (myAudio.isPlaying == false))
            {
                Destroy(gameObject);
            }
        }
        else if ((myParitlce != null) && (myAudio == null))
        {
            if (myParitlce.IsAlive() == false)
            {
                Destroy(gameObject);
            }
        }
        else if ((myParitlce == null) && (myAudio != null))
        {
            if (myAudio.isPlaying == false)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
	}
}
