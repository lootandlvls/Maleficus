using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using static Maleficus.Utils;
public class NetControllerServer : BNJMOBehaviour
{
    protected override void OnGUI()
    {
        base.OnGUI();

        string ipAddress = GetLocalIPAddress();
        GUI.Box(new Rect(10, Screen.height - 50, 100, 50), ipAddress);
        GUI.Label(new Rect(20, Screen.height - 35, 100, 20), "Status : " + NetworkServer.active);
        GUI.Label(new Rect(20, Screen.height - 20, 100, 20), "Connected : " + NetworkServer.connections.Count);
    }

    protected override void Start()
    {
        base.Start();

        NetworkServer.Listen(25000);

    }

    

    public static bool GetIsConnected()
    {
        return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
    }

}
