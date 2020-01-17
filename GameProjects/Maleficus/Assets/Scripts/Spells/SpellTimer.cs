using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpellTimer : MonoBehaviour {
    public event Action SpellTimerDone;

    public float time;
    // Use this for initialization

    private bool isReadyToDie = false; 

    void Start()
    {
        StartCoroutine(Timer());
    }

    private void Update()
    {
        if (isReadyToDie == true)
        {
            bool canDie = true;
            foreach (ParticleSystem effect in transform.GetComponentsInChildren<ParticleSystem>())
            {
                if (effect.IsAlive() == true)
                {
                    canDie = false; ;
                }
            }
            if (canDie == true)
            {
                if (SpellTimerDone != null)
                {
                    SpellTimerDone.Invoke();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }



    IEnumerator Timer()
    {
        yield return new WaitForSeconds(time);
        isReadyToDie = true;

        foreach(ParticleSystem effect in transform.GetComponentsInChildren<ParticleSystem>())
        {
            effect.Stop();
        }

        foreach (Collider collider in transform.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        
    }

}
