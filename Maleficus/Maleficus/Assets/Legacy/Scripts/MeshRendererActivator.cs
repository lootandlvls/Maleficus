using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererActivator : MonoBehaviour {

    public MeshRenderer meshRenderer;



    IEnumerator Timer()
    {
         yield  return new  WaitForSeconds(3);
        meshRenderer.enabled = true;
    }
	// Use this for initialization
	void Start () {
        meshRenderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (meshRenderer.enabled == false)
        {
            StartCoroutine(Timer());
        }
	}
}
