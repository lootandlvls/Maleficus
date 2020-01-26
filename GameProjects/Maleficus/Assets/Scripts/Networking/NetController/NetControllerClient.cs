using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using static Maleficus.Utils;

public class NetControllerClient : BNJMOBehaviour
{
    NetworkClient client;

    protected override void OnGUI()
    {
        base.OnGUI();

        string ipAddress = GetLocalIPAddress();
        GUI.Box(new Rect(10, Screen.height - 50, 100, 50), ipAddress);
        GUI.Label(new Rect(20, Screen.height - 30, 100, 20), "Status : " + client.isConnected);

        if (client.isConnected == false)
        {
            if (GUI.Button(new Rect(10, 10, 60, 50), "Connect"))
            {
                Connect();
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        client = new NetworkClient();
    }

    private void Connect()
    {
        client.Connect("192.168.1.97", 25000);
    }

}
