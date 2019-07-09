using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegisterContext : MonoBehaviour
{
    public static RegisterContext Instance { set; get; }

    [SerializeField] protected TMP_InputField registerUsername;
    [SerializeField] protected TMP_InputField registerEmail;
    [SerializeField] protected TMP_InputField registerPassword;

    [SerializeField] protected TextMeshProUGUI authenticationMessageText;

    private void Start()
    {
        Instance = this;
    }

    public void OnClickCreateAccount()
    {
        DisableInputs();

        string username = registerUsername.text;
        string password = registerPassword.text;
        string email = registerEmail.text;

        NetworkManager.Instance.SendCreateAccount(username, password, email);
    }

    //Todo context should inherit from abstract context
    public void ChangeAuthenticationMessage(string msg)
    {
        authenticationMessageText.text = msg;
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
