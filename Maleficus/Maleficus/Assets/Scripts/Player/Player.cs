using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public bool IsConnected { get { return isConnected; } }

    private bool isConnected;



    #region INPUT
    public void Connect()
    {
        isConnected = true;
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void Move(float axis_X, float axis_Y)
    {
        transform.Rotate(new Vector3(-1.0f * axis_Y, -1.0f * axis_X, 0.0f));
    }

    public void Rotate(float axis_X, float axis_Y)
    {

    }

    public void CastSpell_1()
    {
        StartCoroutine(SpellTestCoroutine());
    }

    public void CastSpell_2()
    {

    }

    public void CastSpell_3()
    {

    }



    private IEnumerator SpellTestCoroutine()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
    #endregion

}
