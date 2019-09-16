using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : AbstractSingletonManager<NetworkManager>
{
    public bool HasAuthority                        { get { return ownClientID == EClientID.SERVER; } }
    public EClientID OwnClientID                    { get { return ownClientID; } }
    public EPlayerID OwnPlayerID                    { get { return MaleficusUtilities.GetPlayerIDFrom(OwnClientID); } }
    public Account Self;                                                                    // TODO [Leon]: public members on top + first letter uppercase
    public List<AbstractNetMessage> AllReceivedMsgs = new List<AbstractNetMessage>();

    private byte reliableChannel;
    private int connectionId;
    private int hostId;
    // for lookup of error codes unitydocs networking networkError
    private byte error;

    private string token;
    private bool isStarted;
    private int ownLobbyID;
    protected EClientID ownClientID;


    #region Monobehaviour

    protected override void Awake()
    {
        base.Awake();

    }

    protected virtual void Start()
    {
        EventManager.Instance.APP_AppStateUpdated.AddListener(On_APP_AppStateUpdated);
    }

    public override void OnSceneStartReinitialize()
    {
        Init();
    }

    protected void Update()
    {
        UpdateMessagePump();
    }
    #endregion

    public virtual void BroadcastNetMessage(AbstractNetMessage netMessage)
    {
        // Prevent that other clients who recieve an event Triggered by a different client, to broadcast the same event again
        if (netMessage.SenderID != OwnClientID)
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
        if (!isStarted)
        {
            NetworkTransport.Init();

            ConnectionConfig cc = new ConnectionConfig();
            reliableChannel = cc.AddChannel(QosType.Unreliable);

            HostTopology topo = new HostTopology(cc, MaleficusConsts.CLIENT_MAX_USER);

            // Client only code
            hostId = NetworkTransport.AddHost(topo, 0);

#if UNITY_WEBGL && !UNITY_EDITOR
        // Web Client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, WEB_PORT, 0, out error);
        Debug.Log("Connecting from Web");
#else
            // Standalone Client
            Debug.Log(MotherOfManagers.Instance.ServerIP);
            connectionId = NetworkTransport.Connect(hostId, MotherOfManagers.Instance.ServerIP, MaleficusConsts.PORT, 0, out error);
            Debug.Log("Connecting from Standalone");
#endif
            Debug.Log(string.Format("Attempting to connect on {0}...", MotherOfManagers.Instance.ServerIP));
            isStarted = true;
            AllReceivedMsgs = new List<AbstractNetMessage>();
        }
    }

    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    public virtual void UpdateMessagePump()
    {
        if (!isStarted)
        {
            return;
        }

        int recHostId;      // is this from web? standalone?
        int connectionId;   // which user is sending me this?
        int channelId;      // which lane is he sending that message from

        byte[] recBuffer = new byte[MaleficusConsts.BYTE_SIZE];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, MaleficusConsts.BYTE_SIZE, out dataSize, out error);
        switch (type)
        {
            case NetworkEventType.ConnectEvent:
                Debug.Log("Connected to server");
                UpdateReceivedMessage(ENetworkMessageType.CONNECTED);
                Net_Connected c = new Net_Connected();
                AllReceivedMsgs.Add(c);
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnected from server");
                UpdateReceivedMessage(ENetworkMessageType.DISCONNECTED);
                Net_Disonnected di = new Net_Disonnected();
                AllReceivedMsgs.Add(di);
                break;

            case NetworkEventType.DataEvent:
                UpdateReceivedMessage(ENetworkMessageType.DATA);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                AbstractNetMessage msg = (AbstractNetMessage)formatter.Deserialize(ms);

                OnData(connectionId, channelId, recHostId, msg);
                break;
        }
    }

    #region OnData
    private void OnData(int cnnId, int channelId, int recHostId, AbstractNetMessage netMessage)
    {
        Debug.Log("receiverd a message of type " + netMessage.ID);

        switch (netMessage.ID)
        {
            /** Connection and Social Logic **/
            case ENetMessageID.NONE:
                Debug.Log("Unexpected NetOP");
                break;

            case ENetMessageID.ON_CREATE_ACCOUNT:
                Debug.Log("Account Created.");
                OnCreateAccount((Net_OnCreateAccount)netMessage);
                UpdateReceivedMessage(ENetworkMessageType.REGISTERED);
                break;

            case ENetMessageID.ON_LOGIN_REQUEST:
                Debug.Log("Login");
                OnLoginRequest((Net_OnLoginRequest)netMessage);
                UpdateReceivedMessage(ENetworkMessageType.LOGGED_IN);
                break;

            case ENetMessageID.ON_ADD_FOLLOW:
                Debug.Log("Add Friend");
                OnAddFollow((Net_OnAddFollow)netMessage);
                UpdateReceivedMessage(ENetworkMessageType.DATA_ONADDFOLLOW);
                break;

            case ENetMessageID.ON_REQUEST_FOLLOW:
                Debug.Log("Get Some Friends");
                OnRequestFollow((Net_OnRequestFollow)netMessage);
                UpdateReceivedMessage(ENetworkMessageType.DATA_ONREQUESTFOLLOW);
                break;
            //Todo [Leon]: change to Onupdatefollow

            case ENetMessageID.UPDATE_FOLLOW:
                Debug.Log("Update Friends");
                UpdateFollow((Net_UpdateFollow)netMessage);
                UpdateReceivedMessage(ENetworkMessageType.DATA_ONUPDATEFOLLOW);
                break;

            case ENetMessageID.ON_INITI_LOBBY:
                Debug.Log("Lobby initialized");
                UpdateReceivedMessage(ENetworkMessageType.DATA_ONINITLOBBY);
                Net_OnInitLobby oil = (Net_OnInitLobby)netMessage;
                ownLobbyID = oil.lobbyID;
                break;

            case ENetMessageID.ON_REQUEST_GAME_SESSION_INFO:
                Debug.Log("Game session info received");
                UpdateReceivedMessage(ENetworkMessageType.DATA_ONREQUESTGAMEINFO);
                OnRequestGameInfo((Net_OnRequestGameInfo)netMessage);
                break;


            /** Game Logic **/
            case ENetMessageID.GAME_STARTED:
                Debug.Log("Game Started");
                NetEvent_GameStarted gameStartedMessage = (NetEvent_GameStarted)netMessage;
                EventManager.Instance.NETWORK_GameStarted.Invoke(gameStartedMessage, EEventInvocationType.LOCAL_ONLY);
                break;

            case ENetMessageID.GAME_OVER:
                Debug.Log("Game Over");
                NetEvent_GameOver gameOver = (NetEvent_GameOver)netMessage;
                EventManager.Instance.GAME_GameOver.Invoke(gameOver, EEventInvocationType.LOCAL_ONLY);
                break;

            case ENetMessageID.AR_STAGE_PLACED:
                NetEvent_ARStagePlaced arStageMessage = (NetEvent_ARStagePlaced)netMessage;
                EventManager.Instance.AR_ARStagePlaced.Invoke(arStageMessage, EEventInvocationType.LOCAL_ONLY);
                break;

            case ENetMessageID.GAME_STATE_REPLICATION:
                Debug.Log("Game state replicated");
                NetEvent_GameStateReplicate gameStateReplicateMessage = (NetEvent_GameStateReplicate)netMessage;
                EventManager.Instance.NETWORK_GameStateReplicate.Invoke(gameStateReplicateMessage, EEventInvocationType.LOCAL_ONLY);
                break;

            // Input
            case ENetMessageID.JOYSTICK_MOVED:
                Debug.Log("Game input received");
                NetEvent_JoystickMoved joystickMoved = (NetEvent_JoystickMoved)netMessage;
                EventManager.Instance.INPUT_JoystickMoved.Invoke(joystickMoved, EEventInvocationType.LOCAL_ONLY);
                break;

            case ENetMessageID.BUTTON_PRESSED:
                Debug.Log("Received Spell Input pressed from another Player");
                NetEvent_ButtonPressed buttonPressed = (NetEvent_ButtonPressed)netMessage;
                EventManager.Instance.INPUT_ButtonPressed.Invoke(buttonPressed, EEventInvocationType.LOCAL_ONLY);
                break;

            case ENetMessageID.BUTTON_RELEASEED:
                Debug.Log("Received Spell Input released from another Player");
                NetEvent_ButtonReleased buttonReleased = (NetEvent_ButtonReleased)netMessage;
                EventManager.Instance.INPUT_ButtonReleased.Invoke(buttonReleased, EEventInvocationType.LOCAL_ONLY);
                break;
  
        }

        // Add message to the dispatcher
        AllReceivedMsgs.Add(netMessage);

    }

    private void OnCreateAccount(Net_OnCreateAccount oca)
    {
        LoginContext.Instance.EnableInputs();
        LoginContext.Instance.ChangeAuthenticationMessage(oca.Information);

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
            Self = new Account();
            Self.ActiveConnection = olr.ConnectionId;
            Self.Username = olr.Username;
            Self.Discriminator = olr.Discriminator;

            token = olr.Token;
            Debug.Log("token: " + token);
            LoginContext.Instance.EnableInputs();

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
        ownClientID = MaleficusUtilities.IntToClientID(orgi.ownPlayerId);
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

        EPlayerID playerID = MaleficusUtilities.GetPlayerIDFrom(ownClientID);
        EventManager.Instance.NETWORK_ReceivedGameSessionInfo.Invoke(new Event_AbstractHandle<List<EPlayerID>, EPlayerID>
            (connectedPlayers, playerID));
    }
    #endregion

    #region Send
    public void SendServer(AbstractNetMessage msg)
    {
        // this is where we hold our data
        byte[] buffer = new byte[MaleficusConsts.BYTE_SIZE];

        // this is where we put our data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, MaleficusConsts.BYTE_SIZE, out error);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        SendServer(new Net_Disonnected());
    }
    #endregion

    #region Account related
    public void SendCreateAccount(string username, string password, string email)
    {

        // invalid username
        if (!MaleficusUtilities.IsUsername(username))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Username is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        // invalid email
        if (!MaleficusUtilities.IsEmail(email))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Email is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        // invalid password
        if (!MaleficusUtilities.IsPassword(password))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Password is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        Net_CreateAccount ca = new Net_CreateAccount();
        ca.Username = username;
        ca.Password = MaleficusUtilities.Sha256FromString(password);
        ca.Email = email;

        LoginContext.Instance.ChangeAuthenticationMessage("Sending request...");
        SendServer(ca);
    }

    public void SendLoginRequest(string usernameOrEmail, string password)
    {
        // todo: username and token working and messages should work
        // invalid email or username
        if (!MaleficusUtilities.IsUsernameAndDiscriminator(usernameOrEmail) && !MaleficusUtilities.IsEmail(usernameOrEmail))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Email or Username#Discriminator is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        // invalid password
        if (!MaleficusUtilities.IsPassword(password))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Password is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        Net_LoginRequest lr = new Net_LoginRequest();

        lr.UsernameOrEmail = usernameOrEmail;
        lr.Password = MaleficusUtilities.Sha256FromString(password);

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

    #endregion
}
