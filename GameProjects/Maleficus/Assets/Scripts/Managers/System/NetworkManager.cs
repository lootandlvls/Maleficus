using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using MongoDB.Bson;
using static Maleficus.MaleficusConsts;
using static Maleficus.MaleficusUtilities;

public class NetworkManager : AbstractSingletonManager<NetworkManager>
{
    public bool HasAuthority                        { get { return ownClientID == EClientID.SERVER; } }
    public EClientID OwnerClientID                    { get { return ownClientID; } }
    public Local_Account Self;
    public bool PlayingOffline = false;

    private byte reliableChannel;
    private int connectionId;
    private int hostId;
    // for lookup of error codes unitydocs networking networkError
    private byte error;

    private string token;
    protected bool pingedSuccessfully = false;
    protected bool isConnected = false;
    private byte tries = 0;
    private int ownLobbyID;
    protected EClientID ownClientID;


    protected int currentTimeStampID;

    /// <summary>
    /// Messages that will be sent on the next client to server update 
    /// </summary>
    private List<AbstractNetMessage> pendingMessages = new List<AbstractNetMessage>();
    /// <summary>
    /// Message already sent to the server but not acnkowledged back yet
    /// </summary>
    private List<AbstractNetMessage> nonAcknowledgedMessages = new List<AbstractNetMessage>();

    #region Monobehaviour
    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        EventManager.Instance.APP_AppStateUpdated.AddListener(On_APP_AppStateUpdated);
        EventManager.Instance.UI_MenuStateUpdated.AddListener(On_UI_MenuStateUpdated);
    }
    protected override void Start()
    {
        base.Start();

        pingedSuccessfully = false;
        StartCoroutine(ConnectToServerCoroutine());
    }
    public override void OnSceneStartReinitialize()
    {
        Init();
    }
    #endregion

    public virtual void BroadcastNetMessage(AbstractNetMessage netMessage)
    {
        // Prevent that other clients who recieve an event Triggered by a different client, to broadcast the same event again
        if (netMessage.SenderID != OwnerClientID)
        {
            return;
        }

        // Broadcast event if main sender
        SendServer(netMessage);
    }
    protected void UpdateReceivedMessage(ENetworkMessageType receivedMessage)
    {
        EventManager.Instance.Invoke_NETWORK_ReceivedMessageUpdated(receivedMessage);
    }
    public virtual void Init()
    {

    }
    private IEnumerator ConnectToServerCoroutine()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable || MotherOfManagers.Instance.ServerIP == PLAY_OFFLINE_IP)
        {
            Debug.Log("couldn't connect to the internet, or playing offline");
            PlayingOffline = true;
            yield return new WaitForSeconds(NETWORK_CONNECT_FREQUENCY);
            UpdateReceivedMessage(ENetworkMessageType.OFFLINE);
            yield break;
        }
        else
        {
            Debug.Log("Connected to the internet");
        }
        DebugLog("Trying to ping Server");

        NetworkTransport.Init();

        ConnectionConfig connectionConfig = new ConnectionConfig();
        reliableChannel = connectionConfig.AddChannel(QosType.Reliable);

        HostTopology hostTopology = new HostTopology(connectionConfig, CLIENT_MAX_USER);

        // Client only code
        hostId = NetworkTransport.AddHost(hostTopology, 0);

#if UNITY_WEBGL && !UNITY_EDITOR
// Web Client
connectionId = NetworkTransport.Connect(hostId, SERVER_IP, WEB_PORT, 0, out error);
Debug.Log("Connecting from Web");
#else
        // Standalone Client
        Debug.Log(MotherOfManagers.Instance.ServerIP);
        connectionId = NetworkTransport.Connect(hostId, MotherOfManagers.Instance.ServerIP, PORT, 0, out error);
        Debug.Log("Ping from Standalone");
#endif
        Debug.Log(string.Format("Attempting to ping {0}...", MotherOfManagers.Instance.ServerIP));
        if (connectionId != 0)
        {
            pingedSuccessfully = true;
            DebugLog("Pinged Server successfully!");
        }
        else
        {
            Debug.Log("Server seems to be not existing");
            PlayingOffline = true;
            yield return new WaitForSeconds(NETWORK_CONNECT_FREQUENCY);
            UpdateReceivedMessage(ENetworkMessageType.OFFLINE);
            yield break;
        }

        yield return new WaitForEndOfFrame();

        // Start fetching network messages
        StartCoroutine(UpdateMessagePumpCoroutine());
    }
    public void Shutdown()
    {
        NetworkTransport.Shutdown();
    }
    private IEnumerator UpdateMessagePumpCoroutine()
    {
        Debug.Log("Trying to connect to the Server...");
        int recHostId;      // is this from web? standalone?
        int connectionId;   // which user is sending me this?
        int channelId;      // which lane is he sending that message from

        byte[] recBuffer = new byte[BYTE_SIZE];
        int dataSize;

        // Start fetching messages routine
        while (pingedSuccessfully == true)
        {
            bool isFetchingCompleted = false;
            while (isFetchingCompleted == false)
            {
                NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);
                switch (type)
                {
                    case NetworkEventType.ConnectEvent:
                        Debug.Log("Connected to server");
                        yield return new WaitForSeconds(NETWORK_UPDATE_FREQUENCY);
                        UpdateReceivedMessage(ENetworkMessageType.CONNECTED);
                        Net_Connected c = new Net_Connected();
                        isConnected = true;
                        break;
                    case NetworkEventType.DisconnectEvent:
                        Debug.Log("Disconnected from server");
                        UpdateReceivedMessage(ENetworkMessageType.DISCONNECTED);
                        Net_Disonnected disconnected = new Net_Disonnected();
                        isConnected = false;
                        // check if internet down or server
                        if (Application.internetReachability == NetworkReachability.NotReachable)
                        {
                            Debug.Log("lost connection to the internet...");
                            pingedSuccessfully = false;
                            UpdateReceivedMessage(ENetworkMessageType.OFFLINE);
                            yield break;
                        }
                        else
                        {
                            Debug.Log("Server down...");
                            yield return new WaitForSeconds(NETWORK_CONNECT_FREQUENCY);
                            Shutdown();
                            StartCoroutine(ConnectToServerCoroutine());
                        }
                        break;

                    case NetworkEventType.DataEvent:
                        UpdateReceivedMessage(ENetworkMessageType.DATA);
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new BinaryFormatter();
                        MemoryStream memoryStream = new MemoryStream(recBuffer);
                        AbstractNetMessage message = (AbstractNetMessage)formatter.Deserialize(memoryStream);

                        OnData(connectionId, channelId, recHostId, message);
                        break;

                    case NetworkEventType.Nothing:
                        isFetchingCompleted = true;
                        break;
                }
                yield return new WaitForSeconds(NETWORK_UPDATE_FREQUENCY);
            }
            if (isConnected == false)
            {
                yield return new WaitForSeconds(NETWORK_CONNECT_FREQUENCY);
                break;
            }
        }

        // if disconnected start connection coroutine
        if (pingedSuccessfully == false)
        {
            yield return new WaitForSeconds(NETWORK_CONNECT_FREQUENCY);
            Shutdown();
            StartCoroutine(ConnectToServerCoroutine());
        }
        StartCoroutine(UpdateMessagePumpCoroutine());
    }

    #region OnData
    private void OnData(int cnnId, int channelId, int recHostId, AbstractNetMessage netMessage)
    {
        Debug.Log("receiverd a message of type " + netMessage.MessageType);

        switch (netMessage.MessageType)
        {
            /** Connection and Social Logic **/
            case ENetMessageType.NONE:
                Debug.Log("Unexpected NetOP");
                break;

            case ENetMessageType.ON_CREATE_ACCOUNT:
                Debug.Log("Account Created.");
                OnCreateAccount((Net_OnCreateAccount)netMessage);
                UpdateReceivedMessage(ENetworkMessageType.REGISTERED);
                break;

            case ENetMessageType.ON_LOGIN_REQUEST:
                Debug.Log("Login");
                OnLoginRequest((Net_OnLoginRequest)netMessage);
                UpdateReceivedMessage(ENetworkMessageType.LOGGED_IN);
                break;

            case ENetMessageType.ON_ADD_FOLLOW:
                Debug.Log("Add Friend");
                OnAddFollow((Net_OnAddFollow)netMessage);
                UpdateReceivedMessage(ENetworkMessageType.DATA_ONADDFOLLOW);
                break;

            case ENetMessageType.ON_REQUEST_FOLLOW:
                Debug.Log("Get Some Friends");
                OnRequestFollow((Net_OnRequestFollow)netMessage);
                UpdateReceivedMessage(ENetworkMessageType.DATA_ONREQUESTFOLLOW);
                break;
            //Todo [Leon]: change to Onupdatefollow

            case ENetMessageType.UPDATE_FOLLOW:
                Debug.Log("Update Friends");
                UpdateFollow((Net_UpdateFollow)netMessage);
                UpdateReceivedMessage(ENetworkMessageType.DATA_ONUPDATEFOLLOW);
                break;

            case ENetMessageType.ON_INITI_LOBBY:
                Debug.Log("Lobby initialized");
                UpdateReceivedMessage(ENetworkMessageType.DATA_ONINITLOBBY);
                Net_OnInitLobby oil = (Net_OnInitLobby)netMessage;
                ownLobbyID = oil.lobbyID;
                break;

            case ENetMessageType.ON_REQUEST_GAME_SESSION_INFO:
                Debug.Log("Game session info received");
                UpdateReceivedMessage(ENetworkMessageType.DATA_ONREQUESTGAMEINFO);
                OnRequestGameInfo((Net_OnRequestGameInfo)netMessage);
                break;


            /** Game Logic **/
            case ENetMessageType.GAME_STARTED:
                Debug.Log("Game Started");
                NetEvent_GameStarted gameStartedMessage = (NetEvent_GameStarted)netMessage;
                EventManager.Instance.NETWORK_GameStarted.Invoke(gameStartedMessage, EEventInvocationType.LOCAL_ONLY);
                break;

            case ENetMessageType.GAME_OVER:
                Debug.Log("Game Over");
                NetEvent_GameOver gameOver = (NetEvent_GameOver)netMessage;
                EventManager.Instance.GAME_GameOver.Invoke(gameOver, EEventInvocationType.LOCAL_ONLY);
                break;

            case ENetMessageType.GAME_STATE_REPLICATION:
                Debug.Log("Game state replicated");
                NetEvent_GameStateReplication gameStateReplicateMessage = (NetEvent_GameStateReplication)netMessage;
                EventManager.Instance.NETWORK_GameStateReplication.Invoke(gameStateReplicateMessage, EEventInvocationType.LOCAL_ONLY);
                break;

            // Input
            case ENetMessageType.JOYSTICK_MOVED:
                Debug.Log("Game input received");
                NetEvent_JoystickMoved joystickMoved = (NetEvent_JoystickMoved)netMessage;
                EventManager.Instance.INPUT_JoystickMoved.Invoke(joystickMoved, EEventInvocationType.LOCAL_ONLY);
                break;

            case ENetMessageType.BUTTON_PRESSED:
                Debug.Log("Received Spell Input pressed from another Player");
                NetEvent_ButtonPressed buttonPressed = (NetEvent_ButtonPressed)netMessage;
                EventManager.Instance.INPUT_ButtonPressed.Invoke(buttonPressed, EEventInvocationType.LOCAL_ONLY);
                break;

            case ENetMessageType.BUTTON_RELEASEED:
                Debug.Log("Received Spell Input released from another Player");
                NetEvent_ButtonReleased buttonReleased = (NetEvent_ButtonReleased)netMessage;
                EventManager.Instance.INPUT_ButtonReleased.Invoke(buttonReleased, EEventInvocationType.LOCAL_ONLY);
                break;
  
        }

    }

    private void OnCreateAccount(Net_OnCreateAccount oca)
    {
        UserManager.CreateLocalUserAccount(true);
        UserManager.UpdateSavedAccountData(oca.user_name, oca.password, oca.email, 255,-1,255,-1,255,oca.account_created,default(BsonDateTime));
        UserManager.LoadSavedAccount();
        if (!oca.random)
        {
            AutoAccountContext.Instance.EnableInputs();
            AutoAccountContext.Instance.user_name.text = UserManager.user.user_name;
            AutoAccountContext.Instance.password.text = UserManager.user.password;
        }
        else
        {
            RegisterContext.Instance.EnableInputs();
            RegisterContext.Instance.ChangeAuthenticationMessage("Registered!");
        }
    }
    private void OnLoginRequest(Net_OnLoginRequest olr)
    {
        LoginContext.Instance.ChangeAuthenticationMessage(olr.Information);
        if (olr.Success != 1)
        {
            // unsuccessfull login
            LoginContext.Instance.EnableInputs();
        }
        else
        {
            // successfull login

            // this is where we save data about ourself
            //Self = new Local_Account();
            //Self.ActiveConnection = olr.ConnectionId;
            //Self.Username = olr.Username;
            //Self.Discriminator = olr.Discriminator;

            //token = olr.Token;
            //Debug.Log("token: " + token);
            //LoginContext.Instance.EnableInputs();

            // change to next state

        }
    }
    private void OnAddFollow(Net_OnAddFollow oaf)
    {
        if (oaf.Success == 1)
        {
            FriendsContext.Instance.AddFollowToUi(oaf.Follow);
        }
    }
    private void OnRequestFollow(Net_OnRequestFollow orf)
    {
        foreach (var follow in orf.Follows)
        {
            FriendsContext.Instance.AddFollowToUi(follow);
        }
    }
    private void UpdateFollow(Net_UpdateFollow fu)
    {
        if (FriendsContext.Instance != null)
        {
            FriendsContext.Instance.UpdateFollow(fu.Follow);
        }
    }
    private void OnRequestGameInfo(Net_OnRequestGameInfo orgi)
    {
        Debug.Log("&/(&/(&(/&/(&( M Player ID : " + orgi.ownPlayerId);
        ownClientID = IntToClientID(orgi.ownPlayerId);
        if (ownClientID == EClientID.NONE)
        {
            Debug.LogError("Couldn't convert Client ID");
        }

        // TODO [Leon]: Change this after presentation
        List<EPlayerID> connectedPlayers = new List<EPlayerID>();
        if (orgi.Player1 != null)
        {
            connectedPlayers.Add(EPlayerID.PLAYER_1);
        }
        if (orgi.Player2 != null)
        {
            connectedPlayers.Add(EPlayerID.PLAYER_2);
        }
        if (orgi.Player3 != null)
        {
            connectedPlayers.Add(EPlayerID.PLAYER_3);
        }
        if (orgi.Player4 != null)
        {
            connectedPlayers.Add(EPlayerID.PLAYER_4);
        }

        EPlayerID playerID = GetPlayerIDFrom(ownClientID);
        EventManager.Instance.NETWORK_ReceivedGameSessionInfo.Invoke(new Event_GenericHandle<List<EPlayerID>, EPlayerID>
            (connectedPlayers, playerID), EEventInvocationType.LOCAL_ONLY);
    }
    #endregion

#region Send
    private void SendServer(AbstractNetMessage msg)
    {
        if(isConnected == false)
        {
            return;
        }
        // this is where we hold our data
        byte[] buffer = new byte[BYTE_SIZE];

        // this is where we put our data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(buffer);
        formatter.Serialize(memoryStream, msg);

        NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        SendServer(new Net_Disonnected());
    }

    #region Account related
    public void SendCreateAccount(bool random, string user_name="", string password="", string email="")
    {
        Net_CreateAccount ca = new Net_CreateAccount();
        ca.random = random;

        if (!random)
        {
            // if account values given
            if (!IsUsername(user_name))
            {
                RegisterContext.Instance.ChangeAuthenticationMessage("Username is invalid");
                RegisterContext.Instance.EnableInputs();
                return;
            }
            if (!IsEmail(email))
            {
                RegisterContext.Instance.ChangeAuthenticationMessage("Email is invalid");
                RegisterContext.Instance.EnableInputs();
                return;
            }
            if (!IsPassword(password))
            {
                RegisterContext.Instance.ChangeAuthenticationMessage("Password is invalid");
                RegisterContext.Instance.EnableInputs();
                return;
            }

            ca.user_name = user_name;
            ca.password = Sha256FromString(password);
            ca.email = email;
        }


        RegisterContext.Instance.ChangeAuthenticationMessage("Sending create account request...");
        SendServer(ca);
    }

    public void SendLoginRequest(string usernameOrEmail, string password)
    {
        // todo: username and token working and messages should work
        // invalid email or username
        if (!IsUsername(usernameOrEmail) && !IsEmail(usernameOrEmail))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Email or Username#Discriminator is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        // invalid password
        if (!IsPassword(password))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Password is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        Net_LoginRequest lr = new Net_LoginRequest();

        lr.UsernameOrEmail = usernameOrEmail;
        lr.Password = Sha256FromString(password);

        LoginContext.Instance.ChangeAuthenticationMessage("Sending Login request...");
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
        Debug.Log("trying to send requestollow");
    }

    protected virtual void CheckForSavedLoginData()
    {
        /*if(user.email != "" && user.password != "")
        {
            SendLoginRequest(user.email, user.password);
        }*/
    }

    #endregion

    #region Lobby related
    public void SendInitLobby()
    {
        Debug.Log("Sending innit lobby");
        Net_InitLobby il = new Net_InitLobby();

        il.Token = token;

        SendServer(il);
    }
    public void SendRequestGameInfo()
    {
        Debug.Log("Sending game info request");
        Net_RequestGameInfo rgi = new Net_RequestGameInfo();

        rgi.Token = token;
        rgi.lobbyID = ownLobbyID;

        SendServer(rgi);
    }

    #endregion
#endregion

#region Listeners
    protected virtual void On_APP_AppStateUpdated(Event_StateUpdated<EAppState> eventHandle)
    {
        switch (eventHandle.NewState)
        {
            case EAppState.IN_GAME_IN_NOT_STARTED:
                SendRequestGameInfo();
                break;
        }
    }

    protected virtual void On_UI_MenuStateUpdated(Event_StateUpdated<EMenuState> eventHandle)
    {
        switch (eventHandle.NewState)
        {
            case EMenuState.IN_ENTRY_IN_LOGIN_IN_AUTO_REGISTER:
                SendCreateAccount(true);
                break;
        }
    }

#endregion
}
