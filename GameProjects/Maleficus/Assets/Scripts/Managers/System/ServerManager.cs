using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Collections;

public class ServerManager : NetworkManager
{
    // Networktransport for clients
    private byte server_reliableChannel;
    private int server_hostId;
    private int server_instanceManagerHostId;
    private int server_webHostId;

    // Networktransport for Instance Manager
    private int InstanceManagerConnectionId;

    //private byte reliableChannel_InstanceManager;
    //private int hostId_InstanceManager; 

    public readonly bool isPlayer = false;

    private byte error;
    private bool connected = false;
    private Mongo dataBank;
    private Dictionary<EClientID, int> connectedPlayers = new Dictionary<EClientID, int>();
    private IEnumerator UpdateGameStateEnumerator;
    private bool isLobbyInitialized = false;

    #region Monobehaviour
    public override void OnSceneStartReinitialize()
    {
        Init();
    }

    protected override void Awake()
    {
        base.Awake();

        ownClientID = EClientID.SERVER;
    }

    protected override void Start()
    {
        base.Start();

        EventManager.Instance.GAME_GameOver.AddListener(On_GameOver);
    }

    protected override void Update()
    {
        if (!connected)
        {
            StartCoroutine("SetUpConnections");
        }
        else
        {
            StartCoroutine("UpdateMessagePump");
        }
    }


    #endregion

    System.Collections.IEnumerator SetUpConnections()
    {
        yield return new WaitForSeconds(MaleficusConsts.NETWORK_CONNECT_FREQUENCY);
        dataBank = new Mongo();
        if (dataBank.Init())
        {
            NetworkTransport.Init();

            ConnectionConfig cc = new ConnectionConfig();

            server_reliableChannel = cc.AddChannel(QosType.Unreliable);


            HostTopology topo = new HostTopology(cc, MaleficusConsts.SERVER_MAX_USER);


            // Server only code

            server_hostId = NetworkTransport.AddHost(topo, MaleficusConsts.PORT, null);
            Debug.Log("added host with Port 26002");
            /*InstanceManagerConnectionId = NetworkTransport.Connect(server_hostId, MaleficusConsts.INSTANCE_MANAGER_SERVER_IP, MaleficusConsts.SERVER_INSTANCE_MANAGER_PORT, 0, out error);
            Debug.Log("connected to Instance Manager Port 9999");

            Debug.Log(string.Format("Opening connection on port {0} and webport {1}", MaleficusConsts.PORT, MaleficusConsts.WEB_PORT));*/
        }
        if(dataBank != null && (server_hostId != -1))
        {
            connected = true;
        }
    }


    System.Collections.IEnumerator UpdateMessagePump()
    {
        int recHostId;      // is this from web? standalone?
        int connectionId;   // which user is sending me this?
        int channelId;      // which lane is he sending that message from

        byte[] recBuffer = new byte[MaleficusConsts.BYTE_SIZE];
        int dataSize;

        bool isContinue = true;
        while (isContinue)
        {
            yield return new WaitForSeconds(MaleficusConsts.NETWORK_UPDATE_FREQUENCY);
            NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, MaleficusConsts.BYTE_SIZE, out dataSize, out error);
            switch (type)
            {
                case NetworkEventType.Nothing:
                    isContinue = false;
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
    }

    #region OnData

    private void OnData(int cnnId, int channelId, int recHostId, AbstractNetMessage netMessage)
    {
        Debug.Log("receiverd a message of type " + netMessage.ID);

        switch (netMessage.ID)
        {
            case ENetMessageID.NONE:
                Debug.Log("Unexpected ENetMessageID");
                break;

            case ENetMessageID.CREATE_ACCOUNT:
                CreateAccount(cnnId, channelId, recHostId, (Net_CreateAccount)netMessage);
                break;

            case ENetMessageID.LOGIN_REQUEST:
                LoginRequest(cnnId, channelId, recHostId, (Net_LoginRequest)netMessage);
                break;

            case ENetMessageID.ADD_FOLLOW:
                AddFollow(cnnId, channelId, recHostId, (Net_AddFollow)netMessage);
                break;

            case ENetMessageID.REMOVE_FOLLOW:
                RemoveFollow(cnnId, channelId, recHostId, (Net_RemoveFollow)netMessage);
                break;

            case ENetMessageID.REQUEST_FOLLOW:
                RequestFollow(cnnId, channelId, recHostId, (Net_RequestFollow)netMessage);
                break;

            case ENetMessageID.INIT_LOBBY:
                if (isLobbyInitialized == false)
                {
                    InitLobby(cnnId, channelId, recHostId, (Net_InitLobby)netMessage);
                    isLobbyInitialized = true;
                }
                break;

            case ENetMessageID.REQUEST_GAME_SESSION_INFO:
                Debug.Log("&/(&/(  recieve request game info");
                Net_OnRequestGameInfo(cnnId, channelId, recHostId, (Net_RequestGameInfo)netMessage);
                break;

                // Disconnect event
            case ENetMessageID.DISCONNECTED:
                DisconnectEvent(recHostId, cnnId);
                break;

            case ENetMessageID.GAME_STARTED:
                Debug.Log("Game starting received");

                NetEvent_GameStarted gameStartedMessage = (NetEvent_GameStarted)netMessage;
                EventManager.Instance.NETWORK_GameStarted.Invoke(gameStartedMessage, EEventInvocationType.LOCAL_ONLY);
                BroadcastMessageToAllClients(gameStartedMessage, false);
                break;

            // Input
            case ENetMessageID.JOYSTICK_MOVED:
                Debug.Log("Game input received");

                NetEvent_JoystickMoved movementMessage = (NetEvent_JoystickMoved)netMessage;
                EventManager.Instance.INPUT_JoystickMoved.Invoke(movementMessage, EEventInvocationType.LOCAL_ONLY);
                BroadcastMessageToAllClients(movementMessage, false);
                break;

            case ENetMessageID.BUTTON_PRESSED:
                Debug.Log("Received Spell Input from another Player");

                NetEvent_ButtonPressed buttonPressed = (NetEvent_ButtonPressed)netMessage;
                EventManager.Instance.INPUT_ButtonPressed.Invoke(buttonPressed, EEventInvocationType.LOCAL_ONLY);
                BroadcastMessageToAllClients(buttonPressed, false);
                break;


            case ENetMessageID.BUTTON_RELEASEED:
                Debug.Log("Received Spell Input from another Player");

                NetEvent_ButtonReleased buttonReleased = (NetEvent_ButtonReleased)netMessage;
                EventManager.Instance.INPUT_ButtonReleased.Invoke(buttonReleased, EEventInvocationType.LOCAL_ONLY);
                BroadcastMessageToAllClients(buttonReleased,  false);
                break;


        }
    }

    private void BroadcastMessageToAllClients(AbstractNetMessage netMessage, bool isExcludeSenderClient)
    {
        foreach (EClientID clientID in connectedPlayers.Keys)
        {
            if (((isExcludeSenderClient == true) && (clientID != netMessage.SenderID))
                || (isExcludeSenderClient == false))
                // Send message to original sender too ?
            {
                int connectionID = connectedPlayers[clientID];
                SendClient(0, connectionID, netMessage);
            }
        }
    }


    private void DisconnectEvent(int recHostId, int connectionId)
    {
        Debug.Log(string.Format("User {0} has disconnected!", connectionId));

        // get a reference to the connected account
        Model_Account account = dataBank.FindAccountByConnectionId(connectionId);

        // if user is logged in
        if (account == null)
        {
            return;
        }

        dataBank.UpdateAccountAfterDisconnection(account.Email);

        // prepare and send our update message
        Net_UpdateFollow fu = new Net_UpdateFollow();
        //Todo strip this down to only nessesary info
        Model_Account updateAccount = dataBank.FindAccountByEmail(account.Email);
        fu.Follow = updateAccount.GetAccount();

        foreach (var f in dataBank.FindAllFollowBy(account.Email))
        {
            if (f.ActiveConnection != 0)
            {
                SendClient(recHostId, f.ActiveConnection, fu);
            }
        }
    }


    #region Account
    private void CreateAccount(int cnnId, int channelId, int recHostId, Net_CreateAccount ca)
    {
        Net_OnCreateAccount oca = new Net_OnCreateAccount();

        if (dataBank.InsertAccount(ca.Username, ca.Password, ca.Email))
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
        string randomToken = MaleficusUtilities.GenerateRandom(128);
        Model_Account account = dataBank.LoginAccount(lr.UsernameOrEmail, lr.Password, cnnId, randomToken);
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

            foreach (var f in dataBank.FindAllFollowBy(account.Email))
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

        if (dataBank.InsertFollow(msg.Token, msg.UsernameDiscriminatorOrEmail))
        {
            oaf.Success = 1;
            if (MaleficusUtilities.IsEmail(msg.UsernameDiscriminatorOrEmail))
            {
                // this is email
                oaf.Follow = dataBank.FindAccountByEmail(msg.UsernameDiscriminatorOrEmail).GetAccount();
            }
            else
            {
                // this is username
                string[] data = msg.UsernameDiscriminatorOrEmail.Split('#');
                if (data[1] == null)
                {
                    return;
                }

                oaf.Follow = dataBank.FindAccountByUsernameAndDiscriminator(data[0], data[1]).GetAccount();
            }
        }

        SendClient(recHostId, cnnId, oaf);
    }
    private void RemoveFollow(int cnnId, int channelId, int recHostId, Net_RemoveFollow msg)
    {
        dataBank.RemoveFollow(msg.Token, msg.UsernameDiscriminator);
    }
    private void RequestFollow(int cnnId, int channelId, int recHostId, Net_RequestFollow msg)
    {
        Net_OnRequestFollow orf = new Net_OnRequestFollow();

        orf.Follows = dataBank.FindAllFollowFrom(msg.Token);

        SendClient(recHostId, cnnId, orf);
    }
    #endregion

    #region Lobby
    private void InitLobby(int cnnId, int channelId, int recHostId, Net_InitLobby il)
    {
        Net_OnInitLobby oil = new Net_OnInitLobby();
        Model_Account self = dataBank.FindAccountByToken(il.Token);
        int lobbyID = dataBank.InitLobby(self._id);
        if (lobbyID != -1)
        {
            oil.Success = 1;
            oil.Information = "Lobby has been initialized";

            // send msg to InstanceManager to run new Instance
            //SendClient(2, cnnId, il);
            EPlayerID playerID = EPlayerID.NONE;
            Model_Lobby thislobby = dataBank.FindLobbyByLobbyID(lobbyID);
            List<EPlayerID> players = new List<EPlayerID>();

            if (thislobby.Team1 != null)
            {
                players.Add(EPlayerID.PLAYER_1);
                Model_Account player1 = dataBank.FindAccountByObjectId(thislobby.Team1[0]);
                connectedPlayers.Add(EClientID.CLIENT_1, player1.ActiveConnection);
            }
            if (thislobby.Team2 != null)
            {
                players.Add(EPlayerID.PLAYER_2);
                Model_Account player2 = dataBank.FindAccountByObjectId(thislobby.Team2[0]);
                connectedPlayers.Add(EClientID.CLIENT_2, player2.ActiveConnection);         // INVERTED!!!
            }
            if (thislobby.Team3 != null)
            {
                Debug.Log("Adding fucking player 3");
                players.Add(EPlayerID.PLAYER_3);
                Model_Account player3 = dataBank.FindAccountByObjectId(thislobby.Team3[0]);
                connectedPlayers.Add(EClientID.CLIENT_3, player3.ActiveConnection);         // INVERTED!!!
            }
            if (thislobby.Team4 != null)
            {
                players.Add(EPlayerID.PLAYER_4);
                Model_Account player4 = dataBank.FindAccountByObjectId(thislobby.Team4[0]);
                connectedPlayers.Add(EClientID.CLIENT_4, player4.ActiveConnection);
            }
            foreach(EClientID clientID in connectedPlayers.Keys)
            {
                Debug.Log(clientID + " is connected : " + connectedPlayers[clientID]);
            }

            var eventHandle = new Event_AbstractHandle<List<EPlayerID>, EPlayerID>(players, playerID);
            EventManager.Instance.NETWORK_ReceivedGameSessionInfo.Invoke(eventHandle, EEventInvocationType.LOCAL_ONLY);
        }
        else
        {
            oil.Success = 0;
            oil.Information = "Lobby couldn't be initialized";
            SendClient(recHostId, cnnId, oil);
            return;
        }

        Model_Lobby lobby = dataBank.FindLobbyByLobbyID(lobbyID);

        oil.lobbyID = lobby.LobbyID;
        // add lobbyID to the player
        dataBank.UpdateAccountInLobby(self._id, lobby._id);

        // send all lobby members the message that the game can start now
        // Todo change so different game modes can be initialized

        //SendClient(recHostId, cnnId, oil);

        Debug.Log("trying to send second client");


        if ((lobby.Team1 != null) && (lobby.Team1.Count != 0))
        {
            Model_Account player1 = dataBank.FindAccountByObjectId(lobby.Team1[0]);
            if (player1 != null)
            {
                Debug.Log("trying to send second client " + player1.ActiveConnection);
                dataBank.UpdateAccountInLobby(player1._id, lobby._id);
                SendClient(recHostId, player1.ActiveConnection, oil);
            }
        }

        if ((lobby.Team2 != null) && (lobby.Team2.Count != 0))
        {
            Model_Account player2 = dataBank.FindAccountByObjectId(lobby.Team2[0]);
            if (player2 != null)
            {
                Debug.Log("trying to send second client " + player2.ActiveConnection);
                dataBank.UpdateAccountInLobby(player2._id, lobby._id);
                SendClient(recHostId, player2.ActiveConnection, oil);
            }
        }

        if ((lobby.Team3 != null) && (lobby.Team3.Count != 0))
        {
            Model_Account player3 = dataBank.FindAccountByObjectId(lobby.Team3[0]);
            if (player3 != null)
            {
                dataBank.UpdateAccountInLobby(player3._id, lobby._id);
                SendClient(recHostId, player3.ActiveConnection, oil);
            }
        }

        if ((lobby.Team4 != null) && (lobby.Team4.Count != 0))
        {
            Model_Account player4 = dataBank.FindAccountByObjectId(lobby.Team4[0]);
            if (player4 != null)
            {
                dataBank.UpdateAccountInLobby(player4._id, lobby._id);
                SendClient(recHostId, player4.ActiveConnection, oil);
            }
        }
    }
    #endregion

    #region Game
    private void Net_OnRequestGameInfo(int connectionID, int channelId, int recHostId, Net_RequestGameInfo rgi)
    {
        Net_OnRequestGameInfo onRequestGameSessionInfo = new Net_OnRequestGameInfo();
        Model_Account self = dataBank.FindAccountByToken(rgi.Token);
        Model_Lobby lobby = dataBank.FindLobbyByLobbyID(rgi.lobbyID);

        onRequestGameSessionInfo.Token = rgi.Token;

        if (dataBank.FindAccountByObjectId(lobby.Team1[0]).Token == rgi.Token)
        {
            onRequestGameSessionInfo.ownPlayerId = 1;
        }
        if ((lobby.Team2 != null) && (dataBank.FindAccountByObjectId(lobby.Team2[0]).Token == rgi.Token))
        {
            onRequestGameSessionInfo.ownPlayerId = 2;
        }
        if ((lobby.Team3 != null) && (dataBank.FindAccountByObjectId(lobby.Team3[0]).Token == rgi.Token))
        {
            onRequestGameSessionInfo.ownPlayerId = 3;
        }
        if ((lobby.Team4 != null) && (dataBank.FindAccountByObjectId(lobby.Team4[0]).Token == rgi.Token))
        {
            onRequestGameSessionInfo.ownPlayerId = 4;
        }

        onRequestGameSessionInfo.initialiser = dataBank.FindAccountByObjectId(lobby.Team1[0]).GetAccount();

        if (lobby.Team1 != null)
        {
            for (int i = 0; i < lobby.Team1.Count; ++i)
            {
                onRequestGameSessionInfo.Player1 = dataBank.FindAccountByObjectId(lobby.Team1[i]).GetAccount();
            }
        }

        if (lobby.Team2 != null)
        {
            for (int i = 0; i < lobby.Team2.Count; ++i)
            {
                onRequestGameSessionInfo.Player2 = dataBank.FindAccountByObjectId(lobby.Team2[i]).GetAccount();
            }
        }

        if (lobby.Team3 != null)
        {
            for (int i = 0; i < lobby.Team3.Count; ++i)
            {
                onRequestGameSessionInfo.Player3 = dataBank.FindAccountByObjectId(lobby.Team3[i]).GetAccount();
            }
        }

        if (lobby.Team4 != null)
        {
            for (int i = 0; i < lobby.Team4.Count; ++i)
            {
                onRequestGameSessionInfo.Player4 = dataBank.FindAccountByObjectId(lobby.Team4[i]).GetAccount();
            }
        }

        SendClient(recHostId, connectionID, onRequestGameSessionInfo);
    }
    #endregion



    #endregion

    #region Send
    public void SendClient(int recHost, int cnnId, AbstractNetMessage message)
    {
        // this is where we hold our data
        byte[] buffer = new byte[MaleficusConsts.BYTE_SIZE];

        // this is where we put our data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(buffer);
        formatter.Serialize(memoryStream, message);
        if (recHost == 0)
        {
            NetworkTransport.Send(server_hostId, cnnId, server_reliableChannel, buffer, MaleficusConsts.BYTE_SIZE, out error);
        }
        else if (recHost == 1)
        {
            NetworkTransport.Send(server_webHostId, cnnId, server_reliableChannel, buffer, MaleficusConsts.BYTE_SIZE, out error);
        }
        else
        {
            NetworkTransport.Send(server_instanceManagerHostId, InstanceManagerConnectionId, server_reliableChannel, buffer, MaleficusConsts.BYTE_SIZE, out error);
            Debug.Log("sent to im");
        }
    }
    #endregion

    private IEnumerator UpdateGameStateCoroutine()
    {
        float[] playerPosition = new float[3];

        while (AppStateManager.Instance.CurrentState == EAppState.IN_GAME_IN_RUNNING)
        {
            yield return new WaitForSeconds(MaleficusConsts.GAME_STATE_UPDATE_FREQUENCY);

            foreach (EPlayerID connectedPlayerIDj in PlayerManager.Instance.ConnectedPlayers.Keys)
            {
                foreach (EPlayerID activePlayerIDi in PlayerManager.Instance.ConnectedPlayers.Keys)
                {
                    if (PlayerManager.Instance.ActivePlayers.ContainsKey(activePlayerIDi))
                    {
                        Debug.Log("Updating player : " + activePlayerIDi + " for player : " + connectedPlayerIDj);
                        playerPosition[0] = PlayerManager.Instance.ActivePlayers[activePlayerIDi].transform.localPosition.x;
                        playerPosition[1] = PlayerManager.Instance.ActivePlayers[activePlayerIDi].transform.localPosition.y;
                        playerPosition[2] = PlayerManager.Instance.ActivePlayers[activePlayerIDi].transform.localPosition.z;
                        NetEvent_GameStateReplicate msg_gameState = new NetEvent_GameStateReplicate(EClientID.SERVER, activePlayerIDi, playerPosition);

                        EClientID updatedClientID = MaleficusUtilities.GetClientIDFrom(connectedPlayerIDj);
                        Debug.Log("Sending game replication to client : " + updatedClientID);
                        if (connectedPlayers.ContainsKey(updatedClientID))
                        {
                            SendClient(0, connectedPlayers[updatedClientID], msg_gameState);
                        }
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
        }
    }

    protected override void On_APP_AppStateUpdated(Event_StateUpdated<EAppState> eventHandle)
    {
        // Do not call base method. Need to be overrid.

        if (UpdateGameStateEnumerator != null)
        {
            StopCoroutine(UpdateGameStateEnumerator);
        }

        switch(eventHandle.NewState)
        {
            case EAppState.IN_GAME_IN_RUNNING:
                StartNewCoroutine(UpdateGameStateEnumerator, UpdateGameStateCoroutine());
                break;
        }
    }

    private void On_GameOver(NetEvent_GameOver gameOverEventHandle)
    {
        foreach (EClientID connectedClientID in connectedPlayers.Keys)
        {
            SendClient(0, connectedPlayers[connectedClientID], gameOverEventHandle);
        }
    }


    private void StartNewCoroutine(IEnumerator enumerator, IEnumerator coroutine)
    {
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
        }
        enumerator = coroutine;
        StartCoroutine(enumerator);
    }
}
