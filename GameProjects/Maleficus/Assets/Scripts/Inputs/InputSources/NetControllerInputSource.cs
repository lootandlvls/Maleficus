using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using static Maleficus.Utils;
using static Maleficus.Consts;

public class NetControllerInputSource : AbstractInputSource
{
    private Dictionary<EControllerID, PlayerNCListener> connectedNetworkControllers = new Dictionary<EControllerID, PlayerNCListener>();

    private float debugX;
    private float debugY;

    protected override void OnGUI()
    {
        base.OnGUI();

        string ipAddress = GetLocalIPAddress();
        GUI.Box(new Rect(10, Screen.height - 50, 100, 30), ipAddress);
        //GUI.Label(new Rect(20, Screen.height - 35, 100, 20), "Status : " + NetworkServer.active);
        //GUI.Label(new Rect(20, Screen.height - 20, 100, 20), "Connected : " + NetworkServer.connections.Count);
    }

    protected override void Start()
    {
        base.Start();

        //NetworkTransport.Init();

        //NetworkServer.Listen(25000);
        //NetworkServer.RegisterHandler(NET_CONTROLLER_MESSAGE_CONNECT, OnRequestControllerID);
        //NetworkServer.RegisterHandler(NET_CONTROLLER_MESSAGE_JOYSTICK_MOVED, OnJoystickMovement);
        //NetworkServer.RegisterHandler(NET_CONTROLLER_MESSAGE_BUTTON_PRESSED, OnReceiveButtonPressed);
        //NetworkServer.RegisterHandler(NET_CONTROLLER_MESSAGE_BUTTON_RELEASED, OnReceiveButtonReleased);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        //NetworkTransport.Shutdown();
    }

    protected override void Update()
    {
        base.Update();

        foreach (var pair in connectedNetworkControllers)
        {
            LogCanvas(82, pair.Value + " : " + pair.Key);
        }

        LogCanvas(82, "X : " + debugX + " - Y : " + debugY);
    }

    /// <summary>
    /// Called from a NCPlayerListener when spawned to connect to the next available ControllerID
    /// </summary>
    /// <param name="playerInput"> New NCPlayerListener </param>
    /// <returns> Assigned ControllerID </returns>
    public EControllerID OnNewNCJoined(PlayerNCListener playerNCListener)
    {
        IS_NOT_NULL(playerNCListener);

 
        // Assign a ControllerID
        EControllerID controllerID = GetNextFreeNetworkControllerID();
        if (controllerID != EControllerID.NONE)
        {
            // Connect controller on Input Manager
            if (InputManager.Instance.ConnectController(controllerID) == true)
            {
                connectedNetworkControllers.Add(controllerID, playerNCListener);

                // Bind Input events
                playerNCListener.ButtonPressed += PlayerNCListener_OnButtonPressed;
                playerNCListener.ButtonReleased += PlayerNCListener_OnButtonReleased;
                playerNCListener.JoystickMoved += PlayerNCListener_OnJoystickMoved;
            }
            else
            {
                return EControllerID.NONE;
            }
        }
        else
        {
            LogConsoleWarning("No free Controller ID found for new connected Net Controller : " + playerNCListener.IpAdress);
        }
        return controllerID;
    }

    private void PlayerNCListener_OnJoystickMoved(EControllerID controllerID, EJoystickType joystickType, float x, float y)
    {
        //if (IS_KEY_CONTAINED(connectedNetworkControllers, controllerID))
        //{
        //    EJoystickType joystickType = (EJoystickType)int.Parse(deltas[1]);
        //    float x = float.Parse(deltas[2], System.Globalization.CultureInfo.InvariantCulture);
        //    float y = float.Parse(deltas[3], System.Globalization.CultureInfo.InvariantCulture);

        //    EControllerID controllerID;
        //    connectedNetworkControllers.TryGetValue(controllerGuid, out controllerID);
        //    if ((IS_NOT_NULL(controllerID))
        //        && (IS_NOT_NULL(joystickType)))
        //    {
        //        InvokeJoystickMoved(controllerID, joystickType, x, y);
        //    }

        //    // Debug
        //    debugX = x;
        //    debugY = y;
        //}
    }

    public void PlayerNCListener_OnButtonPressed(EControllerID controllerID, EInputButton inputButton)
    {
        //StringMessage message = new StringMessage();
        //message.value = receivedMessage.ReadMessage<StringMessage>().value;
        //string[] deltas = message.value.Split('|');
        //string controllerGuid = deltas[0];

        //if (IS_KEY_CONTAINED(connectedNetworkControllers, controllerGuid))
        //{
        //    EInputButton inputButton = (EInputButton)int.Parse(deltas[1]);
        //    EControllerID controllerID;
        //    connectedNetworkControllers.TryGetValue(controllerGuid, out controllerID);
        //    if ((IS_NOT_NULL(controllerID)) 
        //        && (IS_NOT_NULL(inputButton)))
        //    { 
        //        InvokeButtonPressed(controllerID, inputButton);
        //    }
        //}
    }

    public void PlayerNCListener_OnButtonReleased(EControllerID controllerID, EInputButton inputButton)
    {
        //StringMessage message = new StringMessage();
        //message.value = receivedMessage.ReadMessage<StringMessage>().value;
        //string[] deltas = message.value.Split('|');
        //string controllerGuid = deltas[0];

        //if (IS_KEY_CONTAINED(connectedNetworkControllers, controllerGuid))
        //{
        //    EInputButton inputButton = (EInputButton)int.Parse(deltas[1]);
        //    EControllerID controllerID;
        //    connectedNetworkControllers.TryGetValue(controllerGuid, out controllerID);
        //    if ((IS_NOT_NULL(controllerID))
        //        && (IS_NOT_NULL(inputButton)))
        //    {
        //        InvokeButtonReleased(controllerID, inputButton);
        //    }
        //}
    }

    private EControllerID GetNextFreeNetworkControllerID()
    {
        EControllerID controllerID = EControllerID.NONE;
        foreach (EControllerID controllerIDitr in NETWORK_CONTROLLERS)
        {
            if (connectedNetworkControllers.ContainsKey(controllerIDitr) == false)
            {
                controllerID = controllerIDitr;
                break;
            }
        }
        return controllerID;
    }
}
