using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickLoginAction : AbstractUIAction
{
    [SerializeField] private string userName = "BNJMO#0000";
    [SerializeField] private string password = "SSSsss69!";

    public override void Execute()
    {
        base.Execute();

        NetworkManager.Instance.SendLoginRequest(false, userName, password);
    }
}
