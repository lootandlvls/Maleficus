using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoAccountContext : MonoBehaviour
{
    public static AutoAccountContext Instance { set; get; }

    [SerializeField] public TMP_InputField user_name_input_field;
    [SerializeField] public TMP_InputField password_input_field;
    [SerializeField] public TMP_InputField email_input_field;

    //[SerializeField] protected TextMeshProUGUI authenticationMessageText;

    private void Start()
    {
        Instance = this;
    }

    public void OnClickCreateAccount()
    {
        DisableInputs();
        NetworkManager.Instance.SendCreateAccount(true);
    }
    public void ChangeAuthenticationMessage(string msg)
    {
        //authenticationMessageText.text = msg;
    }
    public void OnClickSaveCredentials()
    {
        DisableInputs();

        string user_name = user_name_input_field.text;
        string password = password_input_field.text;
        string email = email_input_field.text;

        NetworkManager.Instance.SendUpdateAccount(true, user_name, "", password, email);
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
