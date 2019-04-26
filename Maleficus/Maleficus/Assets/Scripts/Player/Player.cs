using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public bool IsConnected { get { return isConnected; } }

    private char ControllerID;
    private bool isConnected;



    #region INPUT
    public void Connect(char controllerID)
    {
        this.ControllerID = controllerID;
        isConnected = true;
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void Move(float axis_X, float axis_Y)
    {

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
