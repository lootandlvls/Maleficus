using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 9 || col.gameObject.tag.Equals("Player"))
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator Aging (){
        yield return new WaitForSeconds(15f);
        Destroy(this.gameObject);
    }

    void Start()
    {
        StartCoroutine(Aging());
    }
}
