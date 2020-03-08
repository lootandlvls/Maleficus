using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using static Maleficus.Utils;
using static Maleficus.Consts;

namespace NetController
{
    public enum ENCClientState
    {
        NOT_CONNECTED,
        CONNECTED
    }

    public class NCClient : AbstractSingletonManager<NCClient>
    {
        public event Action<ENCClientState> StateUpdated;

        private NetworkClient client;
        private bool controllerIDRequestSent = false;
        private string controllerGuid;
        private ENCClientState clientState = ENCClientState.NOT_CONNECTED;

        private float debugX;
        private float debugY;

        protected override void InitializeObjecsInScene()
        {
            base.InitializeObjecsInScene();

            ConfirmUIAction confirmUIAction = FindObjectOfType<ConfirmUIAction>();
            if (IS_NOT_NULL(confirmUIAction))
            {
                confirmUIAction.ActionButtonPressed += On_ConfirmUIAction_ActionButtonPressed;
            }

            CancelUIAction cancelUIAction = FindObjectOfType<CancelUIAction>();
            if (IS_NOT_NULL(cancelUIAction))
            {
                cancelUIAction.ActionButtonPressed += On_CancelUIAction_ActionButtonPressed; ;
            }
        }

        protected override void Start()
        {
            base.Start();

            NetworkTransport.Init();
            controllerGuid = Guid.NewGuid().ToString();
            client = new NetworkClient();
        }

        protected override void Update()
        {
            base.Update();

            if (client != null)
            {
                // Send ID request and update state to connected
                if  ((client.isConnected)
                    && (controllerIDRequestSent == false))
                {
                    StringMessage message = new StringMessage();
                    message.value = controllerGuid;
                    client.Send(NET_CONTROLLER_MESSAGE_CONNECT, message);
                    UpdateState(ENCClientState.CONNECTED);
                    controllerIDRequestSent = true;
                }
                // Update state to not connected
                else if ((client.isConnected == false)
                    && (clientState == ENCClientState.CONNECTED))
                {
                    UpdateState(ENCClientState.NOT_CONNECTED);
                }
            }

            LogCanvas(82, "Net Ctrl  - X : " + debugX + " - Y : " + debugY);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            NetworkTransport.Shutdown();

        }

        protected override void OnGUI()
        {
            base.OnGUI();

            string ipAddress = GetLocalIPAddress();
            GUI.Box(new Rect(10, Screen.height - 50, 100, 50), ipAddress);
            if (client != null)
            {
                GUI.Label(new Rect(20, Screen.height - 30, 100, 20), "Status : " + client.isConnected);

                if (client.isConnected == false)
                {
                    if (GUI.Button(new Rect(10, 10, 60, 50), "Connect"))
                    {
                        client.Connect(NCUIContext.Instance.IpAddress, 25000);
                    }
                }
            }
        }

        public void SendJoystickMoved(EJoystickType joystickType, float horizontalDelta, float verticalDelta)
        {
            debugX = horizontalDelta;
            debugY = verticalDelta;

            if (client.isConnected)
            {
                StringMessage message = new StringMessage();
                message.value = controllerGuid + "|" + (int)joystickType + "|" + horizontalDelta + "|" + verticalDelta;
                client.Send(NET_CONTROLLER_MESSAGE_JOYSTICK_MOVED, message);
            }
        }

        public void SendButtonPressed(EInputButton inputButton)
        {
            if (client.isConnected)
            {
                StringMessage message = new StringMessage();
                message.value = controllerGuid + "|" + ((int)inputButton).ToString();
                client.Send(NET_CONTROLLER_MESSAGE_BUTTON_PRESSED, message);
            }
        }

        public void SendButtonReleased(EInputButton inputButton)
        {
            if (client.isConnected)
            {
                StringMessage message = new StringMessage();
                message.value = controllerGuid + "|" + ((int)inputButton).ToString();
                client.Send(NET_CONTROLLER_MESSAGE_BUTTON_RELEASED, message);
            }
        }

        private void On_ConfirmUIAction_ActionButtonPressed()
        {
            if (client.isConnected)
            {
                StringMessage message = new StringMessage();
                message.value = controllerGuid + "|" + ((int)EInputButton.CONFIRM).ToString();
                client.Send(NET_CONTROLLER_MESSAGE_BUTTON_PRESSED, message);
            }
        }

        private void On_CancelUIAction_ActionButtonPressed()
        {
            if (client.isConnected)
            {
                StringMessage message = new StringMessage();
                message.value = controllerGuid + "|" + ((int)EInputButton.CANCEL).ToString();
                client.Send(NET_CONTROLLER_MESSAGE_BUTTON_PRESSED, message);
            }
        }

        private void UpdateState(ENCClientState newState)
        {
            clientState = newState;
            InvokeEventIfBound(StateUpdated, clientState);
        }
    }
}