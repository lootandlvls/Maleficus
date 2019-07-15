using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    private const int MAX_USER = 10000;
    private const int PORT = 26002;
    private const int WEB_PORT = 26004;
    private const int INSTANCE_MANAGER_PORT = 9999;
    private const int BYTE_SIZE = 1024;
    
    // Networktransport for clients
    private byte reliableChannel;
    private int hostId;
    private int instanceManagerHostId;
    private int webHostId;

    // Networktransport for Instance Manager
    private int InstanceManagerConnectionId;
    private const string INSTANCE_MANAGER_SERVER_IP = "54.167.218.82";
    //private const string INSTANCE_MANAGER_SERVER_IP = "127.0.0.1";

    //private byte reliableChannel_InstanceManager;
    //private int hostId_InstanceManager;

    private bool isStarted;
    private byte error;

    private Mongo db;

    #region Monobehaviour
    private void Start()
    {
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
        db = new Mongo();
        db.Init();

        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);


        // Server only code

        hostId = NetworkTransport.AddHost(topo, PORT, null);
        Debug.Log("added host with Port 26002");
        webHostId = NetworkTransport.AddWebsocketHost(topo, WEB_PORT, null);
        Debug.Log("added host with Port 26004");
        InstanceManagerConnectionId = NetworkTransport.Connect(hostId, INSTANCE_MANAGER_SERVER_IP, INSTANCE_MANAGER_PORT, 0, out error);
        Debug.Log("connected to Instance Manager Port 9999");

        Debug.Log(string.Format("Opening connection on port {0} and webport {1}", PORT, WEB_PORT));
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
                if (recHostId != 2)
                {
                    Debug.Log(string.Format("User {0} has connected through host {1}!", connectionId, recHostId));
                }
                else
                {
                    Debug.Log(string.Format("The Instance Manager has connected through host {0}!", recHostId));
                }
                break;
            case NetworkEventType.DisconnectEvent:
                DisconnectEvent(recHostId, connectionId);
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
            case NetOP.CreateAccount:
                CreateAccount(cnnId, channelId, recHostId, (Net_CreateAccount)msg);
                break;
            case NetOP.LoginRequest:
                LoginRequest(cnnId, channelId, recHostId, (Net_LoginRequest)msg);
                break;
            case NetOP.AddFollow:
                AddFollow(cnnId, channelId, recHostId, (Net_AddFollow)msg);
                break;
            case NetOP.RemoveFollow:
                RemoveFollow(cnnId, channelId, recHostId, (Net_RemoveFollow)msg);
                break;
            case NetOP.RequestFollow:
                RequestFollow(cnnId, channelId, recHostId, (Net_RequestFollow)msg);
                break;
            case NetOP.InitLobby:
                InitLobby(cnnId, channelId, recHostId, (Net_InitLobby)msg);
                break;
        }
    }

    private void DisconnectEvent(int recHostId, int connectionId)
    {
        Debug.Log(string.Format("User {0} has disconnected!", connectionId));

        // get a reference to the connected account
        Model_Account account = db.FindAccountByConnectionId(connectionId);

        // if user is logged in
        if (account == null)
        {
            return;
        }
        
        db.UpdateAccountAfterDisconnection(account.Email);

        // prepare and send our update message
        Net_UpdateFollow fu = new Net_UpdateFollow();
        //Todo strip this down to only nessesary info
        Model_Account updateAccount = db.FindAccountByEmail(account.Email);
        fu.Follow = updateAccount.GetAccount();

        foreach(var f in db.FindAllFollowBy(account.Email))
        {
            if(f.ActiveConnection != 0)
            {
                SendClient(recHostId, f.ActiveConnection, fu);
            }
        }      
    }

    #region Account
    private void CreateAccount(int cnnId, int channelId, int recHostId, Net_CreateAccount ca)
    {
        Net_OnCreateAccount oca = new Net_OnCreateAccount();

        if(db.InsertAccount(ca.Username, ca.Password, ca.Email))
        {
            oca.Success = 1;
            oca.Information = "Account was created";
        }
        else
        {
            oca.Success = 0;
            oca.Information = "Account was not created";
        }

        SendClient(recHostId, cnnId, oca);
    }
    private void LoginRequest(int cnnId, int channelId, int recHostId, Net_LoginRequest lr)
    {
        string randomToken = Utility.GenerateRandom(128);
        Model_Account account = db.LoginAccount(lr.UsernameOrEmail, lr.Password, cnnId, randomToken);
        Net_OnLoginRequest olr = new Net_OnLoginRequest();
        if (account != null)
        {
            olr.Success = 1;
            olr.Information = "Logged in as " + account.Username;
            olr.Username = account.Username;
            olr.Discriminator = account.Discriminator;
            olr.Token = randomToken;
            olr.ConnectionId = cnnId;

            // prepare and send our update message
            Net_UpdateFollow fu = new Net_UpdateFollow();
            fu.Follow = account.GetAccount();

            foreach (var f in db.FindAllFollowBy(account.Email))
            {
                if (f.ActiveConnection != 0)
                {
                    //Todo rename cnnId to connectionId in whole solution
                    SendClient(recHostId, f.ActiveConnection, fu);
                }
            }
        }
        else
        {
            olr.Success = 0;
        }

        SendClient(recHostId, cnnId, olr);
    }
    private void AddFollow(int cnnId, int channelId, int recHostId, Net_AddFollow msg)
    {
        Net_OnAddFollow oaf = new Net_OnAddFollow();

        if(db.InsertFollow(msg.Token, msg.UsernameDiscriminatorOrEmail))
        {
            oaf.Success = 1;
            if(Utility.IsEmail(msg.UsernameDiscriminatorOrEmail))
            {
                // this is email
                oaf.Follow = db.FindAccountByEmail(msg.UsernameDiscriminatorOrEmail).GetAccount();
            }
            else
            {
                // this is username
                string[] data = msg.UsernameDiscriminatorOrEmail.Split('#');
                if(data[1] == null)
                {
                    return;
                }

                oaf.Follow = db.FindAccountByUsernameAndDiscriminator(data[0], data[1]).GetAccount();
            }
        }

        SendClient(recHostId, cnnId, oaf);
    }
    private void RemoveFollow(int cnnId, int channelId, int recHostId, Net_RemoveFollow msg)
    {
        db.RemoveFollow(msg.Token, msg.UsernameDiscriminator);
    }
    private void RequestFollow(int cnnId, int channelId, int recHostId, Net_RequestFollow msg)
    {
        Net_OnRequestFollow orf = new Net_OnRequestFollow();

        orf.Follows = db.FindAllFollowFrom(msg.Token);

        SendClient(recHostId, cnnId, orf);
    }
    #endregion

    #region Lobby
    private void InitLobby(int cnnId, int channelId, int recHostId, Net_InitLobby il)
    {
        Net_OnInitLobby oil = new Net_OnInitLobby();
        if (db.InitLobby(il.Token))
        {
            oil.Success = 1;
            oil.Information = "Lobby has been initialized";

            // send msg to InstanceManager to run new Instance
            SendClient(2, cnnId, il);
        }
        else
        {
            oil.Success = 0;
            oil.Information = "Lobby couldn't be initialized";
        }

        SendClient(recHostId, cnnId, oil);
    }
    #endregion

    #endregion

    #region Send
    public void SendClient(int recHost, int cnnId, NetMsg msg)
    {
        // this is where we hold our data
        byte[] buffer = new byte[BYTE_SIZE];

        // this is where we put our data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        if (recHost == 0)
        {
            NetworkTransport.Send(hostId, cnnId, reliableChannel, buffer, BYTE_SIZE, out error);
        }
        else if(recHost == 1)
        {
            NetworkTransport.Send(webHostId, cnnId, reliableChannel, buffer, BYTE_SIZE, out error);
        }
        else
        {
            NetworkTransport.Send(instanceManagerHostId, InstanceManagerConnectionId, reliableChannel, buffer, BYTE_SIZE, out error);
            Debug.Log("sent to im");
        }
    }
    #endregion
}
