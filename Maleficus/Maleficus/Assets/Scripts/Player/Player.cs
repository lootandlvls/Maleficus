using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public bool IsConnected { get { return isConnected; } }

    private bool isConnected;

      Dictionary<int, GameObject> spellsSlot;

    [SerializeField] private GameObject spellSlot_1;
    [SerializeField] private GameObject spellSlot_2;
    [SerializeField] private GameObject spellSlot_3;




    private void Start()
    {
        spellsSlot = new Dictionary<int, GameObject>();
        spellsSlot[1] = spellSlot_1;
        spellsSlot[2] = spellSlot_2;
        spellsSlot[3] = spellSlot_3;
    }


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
       // StartCoroutine(SpellTestCoroutine());
       Instantiate(spellsSlot[1],transform.position, transform.rotation);
        
    }

    public void CastSpell_2()
    {
        Instantiate(spellsSlot[2], transform.position, transform.rotation);
      
    }

    public void CastSpell_3()
    {
        Instantiate(spellsSlot[3], transform.position, transform.rotation);
       
    }


    //set the spells chosen  by the player
    public void SetSpells(GameObject spell_1, GameObject spell_2, GameObject spell_3)
    {
        spellSlot_1 = spell_1;
        spellSlot_2 = spell_2;
        spellSlot_3 = spell_3;

        spellsSlot = new Dictionary<int, GameObject>();

        spellsSlot[1] = spell_1;
        spellsSlot[2] = spell_2;
        spellsSlot[3] = spell_3;
       
    }



    private IEnumerator SpellTestCoroutine()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
    #endregion



}
