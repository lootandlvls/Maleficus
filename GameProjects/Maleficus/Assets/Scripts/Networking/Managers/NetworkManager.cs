using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using MongoDB.Bson;
using static Maleficus.Consts;
using static Maleficus.Utils;
using static UserManager;

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
    protected string currentDebugText="";

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

    protected override void Update()
    {
        base.Update();
        DebugManager.Instance.Log(63, currentDebugText);
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
    
    private IEnumerator ConnectToServerCoroutine()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable || MotherOfManagers.Instance.ServerIP == PLAY_OFFLINE_IP)
        {
            currentDebugText = "Couldn't connect to the internet.";
            PlayingOffline = true;
            yield return new WaitForSeconds(NETWORK_CONNECT_FREQUENCY);
            UpdateReceivedMessage(ENetworkMessageType.OFFLINE);
            yield break;
        }
        else
        {
            currentDebugText =  "Connected to the internet.";
        }
        currentDebugText = "Trying to ping Server...";
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
            currentDebugText = "Pinged Server successfully!...";
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
        currentDebugText = "Trying to connect to the Server...";
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
                        currentDebugText = "";

                        yield return new WaitForSeconds(NETWORK_UPDATE_FREQUENCY);

                        UpdateReceivedMessage(ENetworkMessageType.CONNECTED);
                        Net_Connected c = new Net_Connected();
                        isConnected = true;
                        CheckForSavedLoginData();
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
                break;
            case ENetMessageType.ON_UPDATE_ACCOUNT:
                Debug.Log("On Update Account");
                OnUpdateAccount((Net_OnUpdateAccount)netMessage);
                break;
            case ENetMessageType.ON_LOGIN_REQUEST:
                Debug.Log("Login");
                OnLoginRequest((Net_OnLoginRequest)netMessage);
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

    private void OnRequestGameInfo(Net_OnRequestGameInfo orgi)
    {
        Debug.Log("&/(&/(&(/&/(&( M Player ID : " + orgi.ownPlayerId);
        ownClientID = GetClienIDFrom(orgi.ownPlayerId);
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

        SendServer(ca);
    }
    
    public void SendUpdateAccount(bool update_random, string user_name="", string old_password="", string password="", string email="")
    {
        Net_UpdateAccount ua = new Net_UpdateAccount();
        ua.update_random = update_random;
        ua.token = token;
        ua.user_name = user_name;
        ua.old_password = old_password;
        ua.password = password;
        ua.email = email;

        SendServer(ua);
    }
    
    public void SendLoginRequest(bool is_automatic_login, string user_name_or_email, string password)
    {
        if (!is_automatic_login)
        {
            if (!IsPassword(password))
            {
                // invalid password
                currentDebugText = "Password is invalid";
                LoginContext.Instance.EnableInputs();
                return;
            }
        }

        Net_LoginRequest lr = new Net_LoginRequest();
        if (IsEmail(user_name_or_email))
        {
            lr.email = user_name_or_email;
        }
        else
        {
            lr.user_name = user_name_or_email;
        }

        lr.password = password;

        currentDebugText = "Logging in...";
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
    
    private void UpdateFollow(Net_UpdateFollow fu)
    {
        if (FriendsContext.Instance != null)
        {
            FriendsContext.Instance.UpdateFollow(fu.Follow);
        }
    }
    
    protected virtual void CheckForSavedLoginData()
    {
        if(user != null)
        {
            if (user.user_name != "" && user.password != "")
            {
                SendLoginRequest(true, user.user_name, user.password);
            }
        }
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

    #region AccountListeners
    
    private void OnCreateAccount(Net_OnCreateAccount oca)
    {
        Debug.Log("createdaccount");
        if (oca.success == 1)
        {
            UserManager.CreateLocalUserAccount(true);
            UserManager.UpdateSavedAccountData(oca.user_name, oca.password, oca.email, 255, -1, 255, -1, 255, 
                oca.account_created, default(BsonDateTime));
            AutoAccountContext.Instance.EnableInputs();
            AutoAccountContext.Instance.user_name_input_field.text = UserManager.user.user_name;
            AutoAccountContext.Instance.password_input_field.text = oca.plain_password;
            token = oca.token;
        }
    }
    
    private void OnUpdateAccount(Net_OnUpdateAccount oca)
    {
        if (oca.success == 2)
        {
            currentDebugText = "Current password was entered wrong";
            AutoAccountContext.Instance.EnableInputs();
            return;
        }
        if (oca.success == 3)
        {
            currentDebugText = "The new password is not a valid";
            AutoAccountContext.Instance.EnableInputs();
            return;
        }
        if (oca.success == 4)
        {
            currentDebugText = "The entered user name is not valid";
            AutoAccountContext.Instance.EnableInputs();
            return;
        }
        if (oca.success == 5)
        {
            currentDebugText = "The entered email address is not valid";
            AutoAccountContext.Instance.EnableInputs();
            return;
        }
        Debug.Log("Updating Account was successfull");
        UserManager.UpdateSavedAccountData(oca.user_name, oca.password, oca.email);
        AutoAccountContext.Instance.EnableInputs();
        UpdateReceivedMessage(ENetworkMessageType.REGISTERED);
    }
    
    private void OnLoginRequest(Net_OnLoginRequest olr)
    {
        if(olr.success == 3)
        {
            currentDebugText = "Wrong password-.-";
            LoginContext.Instance.EnableInputs();
            return;
        }
        if (olr.success == 2)
        {
            // unsuccessfull login
            currentDebugText = "Wrong user_name or email";
            LoginContext.Instance.EnableInputs();
            return;
        }
        else
        {
            // successfull login
            UserManager.UpdateSavedAccountData(olr.user_name, olr.password, olr.email, olr.status, 
                olr.coins, olr.level, olr.xp, olr.spent_spell_points, olr.account_created, olr.last_login);
            token = olr.Token;
            Debug.Log("token: " + token);
            LoginContext.Instance.EnableInputs();

            // change to next state
            UpdateReceivedMessage(ENetworkMessageType.LOGGED_IN);
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
    
    #endregion

    #region ManagerEventListeners
    
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
                //if(user == null)
                //{
                    SendCreateAccount(true);
                //}
                /*else
                {
                    Debug.Log("An Account already exists on your device.");
                    //TODO change to new buttons: login, create new account by name
                }*/
                break;
        }
    }
    
    #endregion

    #endregion
}
