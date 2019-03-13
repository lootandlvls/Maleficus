using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour {

    //parent folder
    public Transform par;
    public float force;
    //item list
    public GameObject spawnObject1;
    public GameObject spawnObject2;
    public GameObject spawnObject3;
    public GameObject spawnObject4;
    public GameObject spawnObject5;
    public GameObject spawnObject6;
    public GameObject spawnObject7;
    public GameObject spawnObject8;

    //throwing
    public Rigidbody rb;
    private Vector3 dir;

    IEnumerator SpawnIt()
    {
        while (true)
        {
            SpawnItem();
            //5f just for testing
            yield return new WaitForSeconds(10f);
        }
    }

    void SpawnItem()
    {
        float x = Random.Range(-35, 35);
        while(x <= 25 && x >= -25)
        {
            x = Random.Range(-35, 35);

        }
        float z = Random.Range(-35, 35);
        while (z <= 25 && z >= -25)
        {
            z = Random.Range(-35, 35);
        }

        int rand = Random.Range(0, 8);

        switch (rand)
        {
            case 0:
                var item1 = Instantiate(spawnObject1, new Vector3(x, 20, z), Quaternion.identity) as GameObject;
                item1.transform.SetParent(par);
                rb = item1.gameObject.GetComponent<Rigidbody>();
                break;
            case 1:
                var item2 = Instantiate(spawnObject2, new Vector3(x, 20, z), Quaternion.identity) as GameObject;
                item2.transform.SetParent(par);
                rb = item2.gameObject.GetComponent<Rigidbody>();
                break;
            case 2:
                var item3 = Instantiate(spawnObject3, new Vector3(x, 20, z), Quaternion.identity) as GameObject;
                item3.transform.SetParent(par);
                rb = item3.gameObject.GetComponent<Rigidbody>();
                break;
            case 3:
                var item4 = Instantiate(spawnObject4, new Vector3(x, 20, z), Quaternion.identity) as GameObject;
                item4.transform.SetParent(par);
                rb = item4.gameObject.GetComponent<Rigidbody>();
                break;
            case 4:
                var item5 = Instantiate(spawnObject5, new Vector3(x, 20, z), Quaternion.identity) as GameObject;
                item5.transform.SetParent(par);
                rb = item5.gameObject.GetComponent<Rigidbody>();
                break;
            case 5:
                var item6 = Instantiate(spawnObject6, new Vector3(x, 20, z), Quaternion.identity) as GameObject;
                item6.transform.SetParent(par);
                rb = item6.gameObject.GetComponent<Rigidbody>();
                break;
            case 6:
                var item7 = Instantiate(spawnObject7, new Vector3(x, 20, z), Quaternion.identity) as GameObject;
                item7.transform.SetParent(par);
                rb = item7.gameObject.GetComponent<Rigidbody>();
                break;
            case 7:
                var item8 = Instantiate(spawnObject8, new Vector3(x, 20, z), Quaternion.identity) as GameObject;
                item8.transform.SetParent(par);
                rb = item8.gameObject.GetComponent<Rigidbody>();
                break;
        }
        // float rand_strength = Random.Range(0.87f, 1.14f);
        float rand_strength = Random.Range(0.87f, 1.14f);
        dir = new Vector3(-rb.position.x, 50, -rb.position.z);
        rb.AddForce(dir * force * rand_strength);
    }

    void Start()
    {
        StartCoroutine(SpawnIt());
    }
}
