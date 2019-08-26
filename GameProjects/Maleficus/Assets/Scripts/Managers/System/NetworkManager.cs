using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : AbstractSingletonManager<NetworkManager>
{
    public bool HasAuthority { get { return ownClientID == EClientID.SERVER; } }
    public EClientID OwnClientID { get { return ownClientID; } }
    public List<Net_SpellInput> CastedSpells { get { return castedSpells; } }
    public Account Self;                                                                    // TODO [Leon]: public members on top + first letter uppercase
    public List<AbstractNetMessage> AllReceivedMsgs = new List<AbstractNetMessage>();


    //TODO [Leon]: move consts to maleficus types
    private const int MAX_USER = 100;
    private const int PORT = 26002;
    private const int WEB_PORT = 26004;
    private const int BYTE_SIZE = 1024;
    // 127.0.0.1 or localhost for connecting to yourself
    //ubuntu_server_ip
    //private const string SERVER_IP = "52.91.55.121";
    private const string SERVER_IP = "127.0.0.1";



    private byte reliableChannel;
    private int connectionId;
    private int hostId;
    // for lookup of error codes unitydocs networking networkError
    private byte error;

    private string token;
    private bool isStarted;
    private int ownLobbyID;
    protected EClientID ownClientID;
    private List<Net_SpellInput> castedSpells;


    #region Monobehaviour

    protected override void Awake()
    {
        base.Awake();

    }

    protected void Start()
    {
        EventManager.Instance.APP_AppStateUpdated.AddListener(On_APP_AppStateUpdated);
        //EventManager.Instance.INPUT_ButtonReleased.AddListener      (On_Input_ButtonReleased);
        //EventManager.Instance.INPUT_JoystickMoved.AddListener(On_INPUT_JoystickMoved);
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
        SendServer(netMessage);
    }

    protected void UpdateReceivedMessage(ENetworkMessage receivedMessage)
    {
        EventManager.Instance.Invoke_NETWORK_ReceivedMessageUpdated(receivedMessage);
    }

    public virtual void Init()
    {
        if (!isStarted)
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

        byte[] recBuffer = new byte[BYTE_SIZE];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);
        switch (type)
        {
            //case NetworkEventType.Nothing:
            //    UpdateState(ENetworkMessage.NONE);
            //    break;

            case NetworkEventType.ConnectEvent:
                Debug.Log("Connected to server");
                UpdateReceivedMessage(ENetworkMessage.CONNECTED);
                Net_Connected c = new Net_Connected();
                AllReceivedMsgs.Add(c);
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnected from server");
                UpdateReceivedMessage(ENetworkMessage.DISCONNECTED);
                Net_Disonnected di = new Net_Disonnected();
                AllReceivedMsgs.Add(di);
                break;

            case NetworkEventType.DataEvent:
                UpdateReceivedMessage(ENetworkMessage.DATA);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                AbstractNetMessage msg = (AbstractNetMessage)formatter.Deserialize(ms);

                OnData(connectionId, channelId, recHostId, msg);
                break;

                //default:
                //case NetworkEventType.BroadcastEvent:
                //    UpdateState(ENetworkMessage.BROADCAST);
                //    Debug.Log("Unexpected network event type");
                //    break;
        }
    }

    #region OnData
    private void OnData(int cnnId, int channelId, int recHostId, AbstractNetMessage msg)
    {
        Debug.Log("receiverd a message of type " + msg.ID);

        switch (msg.ID)
        {
            //TODO [Leon]: Add spacing between switch cases
            case NetID.None:
                Debug.Log("Unexpected NetOP");
                break;
            case NetID.OnCreateAccount:
                Debug.Log("Account Created.");
                OnCreateAccount((Net_OnCreateAccount)msg);
                UpdateReceivedMessage(ENetworkMessage.REGISTERED);
                AllReceivedMsgs.Add((Net_OnCreateAccount)msg);
                break;
            case NetID.OnLoginRequest:
                Debug.Log("Login");
                OnLoginRequest((Net_OnLoginRequest)msg);
                UpdateReceivedMessage(ENetworkMessage.LOGGED_IN);
                AllReceivedMsgs.Add((Net_OnLoginRequest)msg);

                break;
            case NetID.OnAddFollow:
                Debug.Log("Add Friend");
                OnAddFollow((Net_OnAddFollow)msg);
                UpdateReceivedMessage(ENetworkMessage.DATA_ONADDFOLLOW);
                AllReceivedMsgs.Add((Net_OnAddFollow)msg);
                break;
            case NetID.OnRequestFollow:
                Debug.Log("Get Some Friends");
                OnRequestFollow((Net_OnRequestFollow)msg);
                UpdateReceivedMessage(ENetworkMessage.DATA_ONREQUESTFOLLOW);
                AllReceivedMsgs.Add((Net_OnRequestFollow)msg);
                break;
            //Todo [Leon]: change to Onupdatefollow
            case NetID.UpdateFollow:
                Debug.Log("Update Friends");
                UpdateFollow((Net_UpdateFollow)msg);
                UpdateReceivedMessage(ENetworkMessage.DATA_ONUPDATEFOLLOW);
                AllReceivedMsgs.Add((Net_UpdateFollow)msg);
                break;
            case NetID.OnInitLobby:
                Debug.Log("Lobby initialized");
                UpdateReceivedMessage(ENetworkMessage.DATA_ONINITLOBBY);
                AllReceivedMsgs.Add((Net_OnInitLobby)msg);
                Net_OnInitLobby oil = (Net_OnInitLobby)msg;
                ownLobbyID = oil.lobbyID;
                break;

            case NetID.OnRequestGameInfo:
                Debug.Log("Game info received");
                UpdateReceivedMessage(ENetworkMessage.DATA_ONREQUESTGAMEINFO);
                AllReceivedMsgs.Add((Net_OnRequestGameInfo)msg);
                OnRequestGameInfo((Net_OnRequestGameInfo)msg);
                break;


            // Input
            case NetID.MovementInput:
                Debug.Log("Game input received");

                Net_JoystickInput movementMessage = (Net_JoystickInput)msg;
                JoystickMovedEventHandle movementEventHandle = new JoystickMovedEventHandle(movementMessage.JoystickType, movementMessage.Joystick_X, movementMessage.Joystick_Y, movementMessage.PlayerID);
                EventManager.Instance.INPUT_JoystickMoved.Invoke(movementEventHandle);

                AllReceivedMsgs.Add((Net_OnRequestGameInfo)msg);
                break;

            case NetID.SpellInput:
                Debug.Log("Received Spell Input from another Player");

                Net_SpellInput spellMessage = (Net_SpellInput)msg;
                if (spellMessage.IsPressed == true)
                {
                    ButtonPressedEventHandle spellEventHandle = new ButtonPressedEventHandle(spellMessage.PlayerID, spellMessage.InputButton);
                    EventManager.Instance.INPUT_ButtonPressed.Invoke(spellEventHandle);
                }
                else // released
                {
                    ButtonReleasedEventHandle spellEventHandle = new ButtonReleasedEventHandle(spellMessage.PlayerID, spellMessage.InputButton);
                    EventManager.Instance.INPUT_ButtonReleased.Invoke(spellEventHandle);
                }


                AllReceivedMsgs.Add((Net_SpellInput)msg);
                break;


        }
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
        FriendsContext.Instance.UpdateFollow(fu.Follow);
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
        EventManager.Instance.NETWORK_ReceivedGameSessionInfo.Invoke(new BasicEventHandle<List<EPlayerID>, EPlayerID>
            (connectedPlayers, playerID));
    }
    #endregion

    #region Send
    public void SendServer(AbstractNetMessage msg)
    {
        // this is where we hold our data
        byte[] buffer = new byte[BYTE_SIZE];

        // this is where we put our data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);
    }

    #region Account related
    public void SendCreateAccount(string username, string password, string email)
    {

        // invalid username
        if (!Utility.IsUsername(username))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Username is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        // invalid email
        if (!Utility.IsEmail(email))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Email is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        // invalid password
        if (!Utility.IsPassword(password))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Password is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        Net_CreateAccount ca = new Net_CreateAccount();
        ca.Username = username;
        ca.Password = Utility.Sha256FromString(password);
        ca.Email = email;

        LoginContext.Instance.ChangeAuthenticationMessage("Sending request...");
        SendServer(ca);
    }

    public void SendLoginRequest(string usernameOrEmail, string password)
    {
        // todo: username and token working and messages should work
        // invalid email or username
        if (!Utility.IsUsernameAndDiscriminator(usernameOrEmail) && !Utility.IsEmail(usernameOrEmail))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Email or Username#Discriminator is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        // invalid password
        if (!Utility.IsPassword(password))
        {
            LoginContext.Instance.ChangeAuthenticationMessage("Password is invalid");
            LoginContext.Instance.EnableInputs();
            return;
        }

        Net_LoginRequest lr = new Net_LoginRequest();

        lr.UsernameOrEmail = usernameOrEmail;
        lr.Password = Utility.Sha256FromString(password);

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

    #region Input related
    //public void SendSpellInput(EInputButton eInputButton, EPlayerID playerID)
    //{
    //    Net_SpellInputPressed si = new Net_SpellInputPressed();

    //    si.Token = token;
    //    switch (eInputButton)
    //    {
    //        case EInputButton.CAST_SPELL_1:
    //            si.spellId = ESpellID.SPELL_1;
    //            break;
    //        case EInputButton.CAST_SPELL_2:
    //            si.spellId = ESpellID.SPELL_2;
    //            break;
    //        case EInputButton.CAST_SPELL_3:
    //            si.spellId = ESpellID.SPELL_3;
    //            break;
    //        default:
    //            si.spellId = ESpellID.NONE;
    //            break;
    //    }
    //    si.ePlayerID = playerID;

    //    SendServer(si);
    //}

    //public void SendMovementInput(EInputAxis eInputAxis, float axisvalue)
    //{
    //    Net_MovementInput mi = new Net_MovementInput();

    //    mi.Token = token;
    //    mi.axis = eInputAxis;
    //    mi.axisValue = axisvalue;

    //    SendServer(mi);
    //}
    #endregion

    #endregion

    #region Listeners
    //public void On_Input_ButtonReleased(ButtonReleasedEventHandle eventHandle)
    //{
    //    EInputButton inputButton = eventHandle.InputButton;
    //    EPlayerID playerID = eventHandle.PlayerID;

    //    if (AppStateManager.Instance.CurrentScene == EScene.GAME)
    //    {
    //        if(inputButton == EInputButton.CAST_SPELL_1 || inputButton == EInputButton.CAST_SPELL_2 || inputButton == EInputButton.CAST_SPELL_3)
    //        {
    //            SendSpellInput(inputButton, playerID);
    //        }
    //    }
    //}
    //public void On_INPUT_JoystickMoved(EInputAxis eInputAxis, float axisvalue, EPlayerID ePlayerID)
    //{
    //    if(ePlayerID == EPlayerID.PLAYER_1)
    //    {
    //        SendMovementInput(eInputAxis, axisvalue);
    //    }
    //}
    private void On_APP_AppStateUpdated(StateUpdatedEventHandle<EAppState> eventHandle)
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
