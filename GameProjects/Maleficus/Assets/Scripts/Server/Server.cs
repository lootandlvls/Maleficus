using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using MongoDB.Bson;
using System.Collections.Generic;

public class Server : NetworkManager
{
    private const int SERVER_MAX_USER = 10000;
    private const int SERVER_PORT = 26002;
    private const int SERVER_WEB_PORT = 26004;
    private const int SERVER_INSTANCE_MANAGER_PORT = 9999;
    private const int SERVER_BYTE_SIZE = 1024;
    
    // Networktransport for clients
    private byte server_reliableChannel;
    private int server_hostId;
    private int server_instanceManagerHostId;
    private int server_webHostId;

    // Networktransport for Instance Manager
    private int InstanceManagerConnectionId;
    private const string INSTANCE_MANAGER_SERVER_IP = "54.167.218.82";
    //private const string INSTANCE_MANAGER_SERVER_IP = "127.0.0.1";

    //private byte reliableChannel_InstanceManager;
    //private int hostId_InstanceManager;

    public readonly bool isPlayer = false;
    private bool server_isStarted;
    private byte error;
    private List<Net_SpellInput> castedSpells;

    private Mongo db;

    #region Monobehaviour

    protected override void Awake()
    {
        // Do not override Instance
    }



    // TODO [Leon]: As you are inheriting from NetworkManager, you can use these same methods from parent class. Remove comments if you agree

    //public override void Initialize()
    //{
    //    Init();
    //}

    //protected void Update()
    //{
    //    UpdateMessagePump();
    //}


    #endregion


    public override bool HasAuthority()
    {
        return true;
    }

    public override void Init()
    {
        db = new Mongo();
        db.Init();

        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();

        server_reliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, SERVER_MAX_USER);


        // Server only code

        server_hostId = NetworkTransport.AddHost(topo, SERVER_PORT, null);
        Debug.Log("added host with Port 26002");
        server_webHostId = NetworkTransport.AddWebsocketHost(topo, SERVER_WEB_PORT, null);
        Debug.Log("added host with Port 26004");
        InstanceManagerConnectionId = NetworkTransport.Connect(server_hostId, INSTANCE_MANAGER_SERVER_IP, SERVER_INSTANCE_MANAGER_PORT, 0, out error);
        Debug.Log("connected to Instance Manager Port 9999");

        Debug.Log(string.Format("Opening connection on port {0} and webport {1}", SERVER_PORT, SERVER_WEB_PORT));
        server_isStarted = true;
    }

    //public void Shutdown()
    //{
    //    server_isStarted = false;
    //    NetworkTransport.Shutdown();
    //}

    //protected void UpdateReceivedMessage(ENetworkMessage receivedMessage)
    //{
    //    EventManager.Instance.Invoke_NETWORK_ReceivedMessageUpdated(receivedMessage);
    //}

    public override void UpdateMessagePump()
    {
        if (!server_isStarted)
        {
            return;
        }

        int recHostId;      // is this from web? standalone?
        int connectionId;   // which user is sending me this?
        int channelId;      // which lane is he sending that message from

        byte[] recBuffer = new byte[SERVER_BYTE_SIZE];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, SERVER_BYTE_SIZE, out dataSize, out error);
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
                AbstractNetMessage msg = (AbstractNetMessage)formatter.Deserialize(ms);
                OnData(connectionId, channelId, recHostId, msg);
                break;
            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type");
                break;
        }
    }

    #region OnData
    protected override void OnData(int cnnId, int channelId, int recHostId, AbstractNetMessage msg)
    {
        Debug.Log("receiverd a message of type " + msg.ID);

        switch (msg.ID)
        {
            case NetID.None:
                Debug.Log("Unexpected NetID");
                break;
            case NetID.CreateAccount:
                CreateAccount(cnnId, channelId, recHostId, (Net_CreateAccount)msg);
                break;
            case NetID.LoginRequest:
                LoginRequest(cnnId, channelId, recHostId, (Net_LoginRequest)msg);
                break;
            case NetID.AddFollow:
                AddFollow(cnnId, channelId, recHostId, (Net_AddFollow)msg);
                break;
            case NetID.RemoveFollow:
                RemoveFollow(cnnId, channelId, recHostId, (Net_RemoveFollow)msg);
                break;
            case NetID.RequestFollow:
                RequestFollow(cnnId, channelId, recHostId, (Net_RequestFollow)msg);
                break;
            case NetID.InitLobby:
                InitLobby(cnnId, channelId, recHostId, (Net_InitLobby)msg);
                break;
            case NetID.RequestGameInfo:
                Net_OnRequestGameInfo(cnnId, channelId, recHostId, (Net_RequestGameInfo)msg);
                break;

                // Input
            case NetID.MovementInput:
                Debug.Log("Game input received");
                Net_MovementInput movementInput = (Net_MovementInput)msg;
                MovementInputEventHandle eventHandle = new MovementInputEventHandle(movementInput.AxisType, movementInput.AxisValue, movementInput.PlayerID);
                EventManager.Instance.INPUT_Movement.Invoke(eventHandle, false);
                AllReceivedMsgs.Add((Net_MovementInput)msg);
                break;

            case NetID.SpellInput:
                Debug.Log("Game input received");
                Net_SpellInput spellInput = (Net_SpellInput)msg;
                SpellInputEventHandle spellEventHandle = new SpellInputEventHandle(spellInput.InputButton, spellInput.PlayerID, spellInput.IsPressed);
                if (spellInput.IsPressed == true)
                {
                    EventManager.Instance.INPUT_ButtonPressed.Invoke(spellEventHandle, false);
                }
                else
                {
                    EventManager.Instance.INPUT_ButtonReleased.Invoke(spellEventHandle, false);
                }
                AllReceivedMsgs.Add((Net_SpellInput)msg);
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
        Model_Account self = db.FindAccountByToken(il.Token);
        if (db.InitLobby(self._id))
        {
            oil.Success = 1;
            oil.Information = "Lobby has been initialized";

            // send msg to InstanceManager to run new Instance
            //SendClient(2, cnnId, il);
            EPlayerID playerID = EPlayerID.NONE;
            List<EPlayerID> connectedPlayers = new List<EPlayerID>();

            Model_Lobby thislobby = db.FindLobbyByInitializerId(self._id);
            
            if(thislobby.Team1 != null)
            {
                connectedPlayers.Add(EPlayerID.PLAYER_1);
            }
            if(thislobby.Team2 != null)
            {
                connectedPlayers.Add(EPlayerID.PLAYER_2);
            }
            if (thislobby.Team3 != null)
            {
                connectedPlayers.Add(EPlayerID.PLAYER_3);
            }
            if (thislobby.Team4 != null)
            {
                connectedPlayers.Add(EPlayerID.PLAYER_4);
            }
            EventManager.Instance.NETWORK_ReceivedGameSessionInfo.Invoke(new BasicEventHandle<List<EPlayerID>, EPlayerID>
            (connectedPlayers, playerID));
        }
        else
        {
            oil.Success = 0;
            oil.Information = "Lobby couldn't be initialized";
            SendClient(recHostId, cnnId, oil);
            return;
        }

        Model_Lobby lobby = db.FindLobbyByInitializerId(self._id);
        oil.lobbyID = lobby.LobbyID;
        // add lobbyID to the player
        db.UpdateAccountInLobby(self._id, lobby._id);

        // send all lobby members the message that the game can start now
        // Todo change so different game modes can be initialized
        SendClient(recHostId, cnnId, oil);

        if ((lobby.Team2 != null) && (lobby.Team2.Count != 0))
        {
            Model_Account player2 = db.FindAccountByObjectId(lobby.Team2[0]);
            if (player2 != null)
            {
                db.UpdateAccountInLobby(player2._id, lobby._id);
                SendClient(recHostId, player2.ActiveConnection, oil);
            }
        }

        if ((lobby.Team3 != null) && (lobby.Team3.Count != 0))
        {
            Model_Account player3 = db.FindAccountByObjectId(lobby.Team3[0]);
            if(player3 != null)
            {
                db.UpdateAccountInLobby(player3._id, lobby._id);
                SendClient(recHostId, player3.ActiveConnection, oil);
            }
        }

        if ((lobby.Team4 != null) && (lobby.Team4.Count != 0))
        {
            Model_Account player4 = db.FindAccountByObjectId(lobby.Team4[0]);
            if (player4 != null)
            {
                db.UpdateAccountInLobby(player4._id, lobby._id);
                SendClient(recHostId, player4.ActiveConnection, oil);
            }
        }
    }
    #endregion

    #region Game
    private void Net_OnRequestGameInfo(int cnnId, int channelId, int recHostId, Net_RequestGameInfo rgi)
    {
        Net_OnRequestGameInfo org = new Net_OnRequestGameInfo();
        Model_Account self = db.FindAccountByToken(rgi.Token);
        Model_Lobby lobby = db.FindLobbyByLobbyID(rgi.lobbyID);

        org.Token = rgi.Token;

        if (db.FindAccountByObjectId(lobby.Team1[0]).Token == rgi.Token)
        {
            org.ownPlayerId = 1;
        }
        if ((lobby.Team2 != null) && (db.FindAccountByObjectId(lobby.Team2[0]).Token == rgi.Token))
        {
            org.ownPlayerId = 2;
        }
        if ((lobby.Team3 != null) && (db.FindAccountByObjectId(lobby.Team3[0]).Token == rgi.Token))
        {
            org.ownPlayerId = 3;
        }
        if ((lobby.Team4 != null) && (db.FindAccountByObjectId(lobby.Team4[0]).Token == rgi.Token))
        {
            org.ownPlayerId = 4;
        }

        org.initialiser = db.FindAccountByObjectId(lobby.Team1[0]).GetAccount();

        if (lobby.Team1 != null)
        {
            for (int i = 0; i < lobby.Team1.Count; ++i)
            {
                org.Player1 = db.FindAccountByObjectId(lobby.Team1[i]).GetAccount();
            }
        }

        if (lobby.Team2 != null)
        {
            for (int i = 0; i < lobby.Team2.Count; ++i)
            {
                org.Player2 = db.FindAccountByObjectId(lobby.Team2[i]).GetAccount();
            }
        }

        if (lobby.Team3 != null)
        {
            for (int i = 0; i < lobby.Team3.Count; ++i)
            {
                org.Player3 = db.FindAccountByObjectId(lobby.Team3[i]).GetAccount();
            }
        }

        if (lobby.Team4 != null)
        {
            for (int i = 0; i < lobby.Team4.Count; ++i)
            {
                org.Player4 = db.FindAccountByObjectId(lobby.Team4[i]).GetAccount();
            }
        }
        SendClient(recHostId, cnnId, org);
    }
    private void ForwardSpellInput(int cnnId, int channelId, int recHostId, Net_SpellInput si)
    {
        Model_Account self = db.FindAccountByToken(si.Token);
        if(self != null)
        {
            Model_Lobby lobby = db.FindLobbyByObjectId(self.inLobby);
            if(lobby != null)
            {
                // send all other players the movement message
                if (lobby.Team1 != null && lobby.Team1.Count != 0 && lobby.Team1[0] != self._id)
                {
                    Model_Account friend1 = db.FindAccountByObjectId(lobby.Team1[0]);
                    if (friend1 != null)
                    {
                        SendClient(recHostId, friend1.ActiveConnection, si);
                    }
                }

                if (lobby.Team2 != null && lobby.Team2.Count != 0 && lobby.Team2[0] != self._id)
                {
                    Model_Account friend2 = db.FindAccountByObjectId(lobby.Team2[0]);
                    if(friend2 != null)
                    {
                        SendClient(recHostId, friend2.ActiveConnection, si);
                    }
                }

                if (lobby.Team3 != null && lobby.Team3.Count != 0 && lobby.Team3[0] != self._id)
                {
                    Model_Account friend3 = db.FindAccountByObjectId(lobby.Team3[0]);
                    if (friend3 != null)
                    {
                        SendClient(recHostId, friend3.ActiveConnection, si);
                    }
                }

                if (lobby.Team4 != null && lobby.Team4.Count != 0 && lobby.Team4[0] != self._id)
                {
                    Model_Account friend4 = db.FindAccountByObjectId(lobby.Team4[0]);
                    if (friend4 != null)
                    {
                        SendClient(recHostId, friend4.ActiveConnection, si);
                    }
                }
            }
        }

    }
    #endregion



    #endregion

    #region Send
    public void SendClient(int recHost, int cnnId, AbstractNetMessage msg)
    {
        // this is where we hold our data
        byte[] buffer = new byte[SERVER_BYTE_SIZE];

        // this is where we put our data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        if (recHost == 0)
        {
            NetworkTransport.Send(server_hostId, cnnId, server_reliableChannel, buffer, SERVER_BYTE_SIZE, out error);
        }
        else if(recHost == 1)
        {
            NetworkTransport.Send(server_webHostId, cnnId, server_reliableChannel, buffer, SERVER_BYTE_SIZE, out error);
        }
        else
        {
            NetworkTransport.Send(server_instanceManagerHostId, InstanceManagerConnectionId, server_reliableChannel, buffer, SERVER_BYTE_SIZE, out error);
            Debug.Log("sent to im");
        }
    }
    #endregion
}
