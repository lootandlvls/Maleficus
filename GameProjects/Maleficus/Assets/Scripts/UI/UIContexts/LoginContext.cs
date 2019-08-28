using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginContext : MonoBehaviour
{
    public static LoginContext Instance { set; get; }

    [SerializeField] protected TMP_InputField loginUsernameOrEmail;
    [SerializeField] protected TMP_InputField loginPassword;

    //[SerializeField] protected TextMeshProUGUI authenticationMessageText;

    private void Start()
    {
        Instance = this;
    }

    public void OnClickLoginRequest()
    {
        DisableInputs();

        string usernameOrEmail = loginUsernameOrEmail.text;
        string password = loginPassword.text;

        NetworkManager.Instance.SendLoginRequest(usernameOrEmail, password);
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
