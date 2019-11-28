using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoAccountContext : MonoBehaviour
{
    public static AutoAccountContext Instance { set; get; }

    [SerializeField] public TMP_Text user_name;
    [SerializeField] public TMP_Text password;

    //[SerializeField] protected TextMeshProUGUI authenticationMessageText;

    private void Start()
    {
        Instance = this;
    }

    public void ChangeAuthenticationMessage(string msg)
    {
        //authenticationMessageText.text = msg;
    }
    public void EnableInputs()
    {
        GameObject.Find("Canvas").GetComponent<CanvasGroup>().interactable = true;
    }
    public void DisableInputs()
    {
        GameObject.Find("Canvas").GetComponent<CanvasGroup>().interactable = false;
    }
}
