﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    public static Client Instance { private set; get; }

    private const int MAX_USER = 100;
    private const int PORT = 26002;
    private const int WEB_PORT = 26004;
    private const int BYTE_SIZE = 1024;
    // 127.0.0.1 or localhost for connecting to yourself
    //ubuntu_server_ip
    private const string SERVER_IP = "52.91.55.121";



    private byte reliableChannel;
    private int connectionId;
    private int hostId;
    // for lookup of error codes unitydocs networking networkError
    private byte error;

    public Account self;
    private string token;
    private bool isStarted;

    #region Monobehaviour
    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Update()
    {
        UpdateMessagePump();
    }
    #endregion

    public void Init()
    {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);

        // Client only code
        hostId = NetworkTransport.AddHost(topo, 0);

#if UNITY_WEBGL && !UNITY_EDITOR
        // Web Client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, WEB_PORT, 0, out error);
        Debug.Log("Connecting from Web");
#else
        // Standalone Client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, PORT, 0, out error);
        Debug.Log("Connecting from Standalone");
#endif
        Debug.Log(string.Format("Attempting to connect on {0}...", SERVER_IP));
        isStarted = true;
    }

    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    public void UpdateMessagePump()
    {
        if (!isStarted)
        {
            return;
        }

        int recHostId;      // is this from web? standalone?
        int connectionId;   // which user is sending me this?
        int channelId;      // which lane is he sending that message from

        byte[] recBuffer = new byte[BYTE_SIZE];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);
        switch (type)
        {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                Debug.Log("Connected to server");
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnected from server");
                break;

            case NetworkEventType.DataEvent:
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms);

                OnData(connectionId, channelId, recHostId, msg);
                break;

            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type");
                break;
        }
    }

    #region OnData
    private void OnData(int cnnId, int channelId, int recHostId, NetMsg msg)
    {
        Debug.Log("receiverd a message of type " + msg.OP);

        switch (msg.OP)
        {
            case NetOP.None:
                Debug.Log("Unexpected NetOP");
                break;
            case NetOP.OnCreateAccount:
                OnCreateAccount((Net_OnCreateAccount)msg);
                break;
            case NetOP.OnLoginRequest:
                OnLoginRequest((Net_OnLoginRequest)msg);
                break;
            case NetOP.OnAddFollow:
                OnAddFollow((Net_OnAddFollow)msg);
                break;
            case NetOP.OnRequestFollow:
                OnRequestFollow((Net_OnRequestFollow)msg);
                break;
            case NetOP.UpdateFollow:
                UpdateFollow((Net_UpdateFollow)msg);
                break;
        }
    }

    private void OnCreateAccount(Net_OnCreateAccount oca)
    {
        LobbyScene.Instance.EnableInputs();
        LobbyScene.Instance.ChangeAuthenticationMessage(oca.Information);
    }
    private void OnLoginRequest(Net_OnLoginRequest olr)
    {
        LobbyScene.Instance.ChangeAuthenticationMessage(olr.Information);
        if (olr.Success != 1)
        {
            // unsuccessfull login
            LobbyScene.Instance.EnableInputs();
        }
        else
        {
            // successfull login

            // this is where we save data about ourself
            self = new Account();
            self.ActiveConnection = olr.ConnectionId;
            self.Username = olr.Username;
            self.Discriminator = olr.Discriminator;

            token = olr.Token;

            UnityEngine.SceneManagement.SceneManager.LoadScene("Hub");
        }
    }
    private void OnAddFollow(Net_OnAddFollow oaf)
    {
        if(oaf.Success == 1)
        {
            HubScene.Instance.AddFollowToUi(oaf.Follow);
        }
    }
    private void OnRequestFollow(Net_OnRequestFollow orf)
    {
        foreach(var follow in orf.Follows)
        {
            HubScene.Instance.AddFollowToUi(follow);
        }
    }
    private void UpdateFollow(Net_UpdateFollow fu)
    {
        HubScene.Instance.UpdateFollow(fu.Follow);
    }
    #endregion

    #region Send
    public void SendServer(NetMsg msg)
    {
        // this is where we hold our data
        byte[] buffer = new byte[BYTE_SIZE];

        // this is where we put our data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);
    }
    public void SendCreateAccount(string username, string password, string email)
    {

        // invalid username
        if(!Utility.IsUsername(username))
        {
            LobbyScene.Instance.ChangeAuthenticationMessage("Username is invalid");
            LobbyScene.Instance.EnableInputs();
            return;
        }

        // invalid email
        if (!Utility.IsEmail(email))
        {
            LobbyScene.Instance.ChangeAuthenticationMessage("Email is invalid");
            LobbyScene.Instance.EnableInputs();
            return;
        }

        // invalid password
        if (!Utility.IsPassword(password))
        {
            LobbyScene.Instance.ChangeAuthenticationMessage("Password is invalid");
            LobbyScene.Instance.EnableInputs();
            return;
        }

        Net_CreateAccount ca = new Net_CreateAccount();
        ca.Username = username;
        ca.Password = Utility.Sha256FromString(password);
        ca.Email = email;

        LobbyScene.Instance.ChangeAuthenticationMessage("Sending request...");
        SendServer(ca);
    }
    public void SendLoginRequest(string usernameOrEmail, string password)
    {
        // todo username and token working and messages should work
        // invalid email or username
        if (!Utility.IsUsernameAndDiscriminator(usernameOrEmail) && !Utility.IsEmail(usernameOrEmail))
        {
            LobbyScene.Instance.ChangeAuthenticationMessage("Email or Username#Discriminator is invalid");
            LobbyScene.Instance.EnableInputs();
            return;
        }

        // invalid password
        if (!Utility.IsPassword(password))
        {
            LobbyScene.Instance.ChangeAuthenticationMessage("Password is invalid");
            LobbyScene.Instance.EnableInputs();
            return;
        }

        Net_LoginRequest lr = new Net_LoginRequest();

        lr.UsernameOrEmail = usernameOrEmail;
        lr.Password = Utility.Sha256FromString(password);

        LobbyScene.Instance.ChangeAuthenticationMessage("Sending Login request...");
        SendServer(lr);
    }

    public void SendAddFollow(string usernameOrEmail)
    {
        Net_AddFollow af = new Net_AddFollow();
        af.Token = token;
        af.UsernameDiscriminatorOrEmail = usernameOrEmail;

        SendServer(af);
    }
    public void SendRemoveFollow(string username)
    {
        Net_RemoveFollow rf = new Net_RemoveFollow();
        rf.Token = token;
        rf.UsernameDiscriminator = username;

        SendServer(rf);
    }
    public void SendRequestFollow()
    {
        Net_RequestFollow rf = new Net_RequestFollow();

        rf.Token = token;

        SendServer(rf);
    }
    #endregion
}
