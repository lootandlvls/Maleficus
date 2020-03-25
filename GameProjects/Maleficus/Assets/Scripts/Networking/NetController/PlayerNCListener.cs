using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NetController;
using System;

public class PlayerNCListener : NetworkBehaviour
{
    public event Action<EControllerID, EInputButton> ButtonPressed;
    public event Action<EControllerID, EInputButton> ButtonReleased;
    public event Action<EControllerID, EJoystickType, float, float> JoystickMoved;

    public string IpAdress { get { return myNetworkIdentity.connectionToClient.address; } }

    private NetworkIdentity myNetworkIdentity;
    private EControllerID controllerID = EControllerID.NONE;

    private void Awake()
    {
        myNetworkIdentity = GetComponent<NetworkIdentity>();
    }

    // got active on server
    public override void OnStartServer()
    {
        base.OnStartServer();


    }

    // Owning client is spawned
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        // Bind events of buttons and joysticks
        NCTouchJoystickAdapter touchJoystickAdapter = FindObjectOfType<NCTouchJoystickAdapter>();
        if (touchJoystickAdapter)
        {
            Debug.Log("Binding events from NCTouchJoystickAdapter");

            //touchJoystickAdapter.ButtonPressed += On_TouchJoystickAdapter_ButtonPressed;
            //touchJoystickAdapter.ButtonReleased += On_TouchJoystickAdapter_ButtonReleased;
            //touchJoystickAdapter.JoystickMoved += On_TouchJoystickAdapter_JoystickMoved;
        }

        RequestControllerID();

        // TODO: Find a better way to check if is server
        if (InputManager.IsInstanceSet == false)
        {
            RequestControllerID();
        }
    }



    #region Requesting Controller ID 
    [Client]
    private void RequestControllerID()
    {
        Debug.Log("RequestControllerID");

        Cmd_OnRequestControllerID(myNetworkIdentity);
    }

    [Command]
    private void Cmd_OnRequestControllerID(NetworkIdentity networkIdentity)
    {
        Debug.Log("Cmd_OnRequestControllerID : " + networkIdentity.connectionToClient.address);

        NetControllerInputSource netControllerInputSource = InputManager.Instance.GetInputSource<NetControllerInputSource>();
        if (netControllerInputSource)
        {
            EControllerID newControllerID = netControllerInputSource.OnNewNCJoined(this);
            Debug.Log("Assigned controller ID : " + newControllerID);

            if (controllerID != EControllerID.NONE)
            {
                Target_OnAssignedControllerID(networkIdentity, newControllerID);
            }
        }
    }

    [TargetRpc]
    private void Target_OnAssignedControllerID(NetworkIdentity networkIdentity, EControllerID assignedControllerID)
    {
        controllerID = assignedControllerID;
        Debug.Log("Assigned ControllerID : " + controllerID);
    }
    #endregion

    //#region Button Pressed 
    //[Client]
    //private void On_TouchJoystickAdapter_ButtonPressed(EInputButton inputButton)
    //{
    //    Cmd_OnButtonPressed(controllerID, inputButton);
    //}

    //[Command]
    //private void Cmd_OnButtonPressed(EControllerID controllerID, EInputButton inputButton)
    //{
    //    if (NetControllerInputSource.IsInstanceSet)
    //    {
    //        NetControllerInputSource.Instance.butto
    //    }
    //}
    //#endregion


    //[Client]
    //private void On_TouchJoystickAdapter_ButtonReleased(EInputButton inputButton)
    //{
    //    Cmd_OnButtonReleased(controllerID, inputButton);
    //}

    //[Client]
    //private void On_TouchJoystickAdapter_JoystickMoved(EJoystickType joystickType, float x, float y)
    //{
    //    Cmd_OnJoystickMoved(controllerID, joystickType, x, y);
    //}

    ///* To NC Server */



    //[Command]
    //private void Cmd_OnButtonReleased(EControllerID controllerID, EInputButton inputButton)
    //{

    //}

    //[Command]
    //private void Cmd_OnJoystickMoved(EControllerID controllerID, EJoystickType joystickType, float x, float y)
    //{

    //}














    // On every client
    public override void OnStartClient()
    {
        base.OnStartClient();


    }


    // Better use OnStartAuthority
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

    }


    public override void OnStopAuthority()
    {
        base.OnStopAuthority();

    }

    [Client] // only runs on the client
    public void Client_UpdateInput()
    {
        if (isLocalPlayer) // only counts for PlayerPrefab. Don't use!
        {

        }

        if (base.hasAuthority) // Correct is locally controlled
        {

        }
    }

    [Command] // run on server from client
    public void Cmd_UpdateInput()
    {

    }





    public void SendJoystickMoved(EJoystickType joystickType, float horizontalDelta, float verticalDelta)
    {
        //debugX = horizontalDelta;
        //debugY = verticalDelta;

        //if (client.isConnected)   
        //{
        //    StringMessage message = new StringMessage();
        //    message.value = controllerGuid + "|" + (int)joystickType + "|" + horizontalDelta + "|" + verticalDelta;
        //    client.Send(NET_CONTROLLER_MESSAGE_JOYSTICK_MOVED, message);
        //}
    }

    public void SendButtonPressed(EInputButton inputButton)
    {
        //if (client.isConnected)
        //{
        //    StringMessage message = new StringMessage();
        //    message.value = controllerGuid + "|" + ((int)inputButton).ToString();
        //    client.Send(NET_CONTROLLER_MESSAGE_BUTTON_PRESSED, message);
        //}
    }

    public void SendButtonReleased(EInputButton inputButton)
    {
        //if (client.isConnected)
        //{
        //    StringMessage message = new StringMessage();
        //    message.value = controllerGuid + "|" + ((int)inputButton).ToString();
        //    client.Send(NET_CONTROLLER_MESSAGE_BUTTON_RELEASED, message);
        //}
    }

    private void On_ConfirmUIAction_ActionButtonPressed()
    {
        //if (client.isConnected)
        //{
        //    StringMessage message = new StringMessage();
        //    message.value = controllerGuid + "|" + ((int)EInputButton.CONFIRM).ToString();
        //    client.Send(NET_CONTROLLER_MESSAGE_BUTTON_PRESSED, message);
        //}
    }

    private void On_CancelUIAction_ActionButtonPressed()
    {
        //if (client.isConnected)
        //{
        //    StringMessage message = new StringMessage();
        //    message.value = controllerGuid + "|" + ((int)EInputButton.CANCEL).ToString();
        //    client.Send(NET_CONTROLLER_MESSAGE_BUTTON_PRESSED, message);
        //}
    }
}
