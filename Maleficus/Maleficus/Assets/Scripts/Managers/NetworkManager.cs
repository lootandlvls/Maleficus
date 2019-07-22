using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : AbstractSingletonManager<NetworkManager>
{

    // public static NetworkManager Instance { private set; get; }                                                  // TODO: Removed this member as it hides the one in parent class. remove the comments if this makes sense or revert
    //TODO move consts to maleficus types
    private const int MAX_USER = 100;
    private const int PORT = 26002;
    private const int WEB_PORT = 26004;
    private const int BYTE_SIZE = 1024;
    // 127.0.0.1 or localhost for connecting to yourself
    //ubuntu_server_ip
    private const string SERVER_IP = "52.91.55.121";
    //private const string SERVER_IP = "127.0.0.1";



    private byte reliableChannel;
    private int connectionId;
    private int hostId;
    // for lookup of error codes unitydocs networking networkError
    private byte error;

    public Account self;
    private string token;
    private bool isStarted;
    public List<NetMsg> allReceivedMsgs;

    #region Monobehaviour

    protected override void Awake()
    {
        base.Awake();
    }

    protected void Start()
    { 
        EventManager.Instance.INPUT_ButtonReleased += On_Input_ButtonReleased;
        EventManager.Instance.INPUT_JoystickMoved += On_INPUT_JoystickMoved;
    }

    public override void Initialize()
    {
        Init();
    }

    protected  void Update()
    {
        UpdateMessagePump();
    }
    #endregion

    protected void UpdateReceivedMessage(ENetworkMessage receivedMessage)
    {
        EventManager.Instance.Invoke_NETWORK_ReceivedMessageUpdated(receivedMessage);
    }

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
        allReceivedMsgs = new List<NetMsg>();
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
            //case NetworkEventType.Nothing:
            //    UpdateState(ENetworkMessage.NONE);
            //    break;

            case NetworkEventType.ConnectEvent:
                Debug.Log("Connected to server");
                UpdateReceivedMessage(ENetworkMessage.CONNECTED);
                Net_Connected c = new Net_Connected();
                allReceivedMsgs.Add(c);
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnected from server");
                UpdateReceivedMessage(ENetworkMessage.DISCONNECTED);
                Net_Disonnected di = new Net_Disonnected();
                allReceivedMsgs.Add(di);
                break;

            case NetworkEventType.DataEvent:
                UpdateReceivedMessage(ENetworkMessage.DATA);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms);

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
    private void OnData(int cnnId, int channelId, int recHostId, NetMsg msg)
    {
        Debug.Log("receiverd a message of type " + msg.OP);

        switch (msg.OP)
        {
            case NetOP.None:
                Debug.Log("Unexpected NetOP");
                break;
            case NetOP.OnCreateAccount:
                Debug.Log("Account Created.");
                OnCreateAccount((Net_OnCreateAccount)msg);
                UpdateReceivedMessage(ENetworkMessage.REGISTERED);
                allReceivedMsgs.Add((Net_OnCreateAccount)msg);
                break;
            case NetOP.OnLoginRequest:
                Debug.Log("Login");
                OnLoginRequest((Net_OnLoginRequest)msg);
                UpdateReceivedMessage(ENetworkMessage.LOGGED_IN);
                allReceivedMsgs.Add((Net_OnLoginRequest)msg);

                break;
            case NetOP.OnAddFollow:
                Debug.Log("Add Friend");
                OnAddFollow((Net_OnAddFollow)msg);
                UpdateReceivedMessage(ENetworkMessage.DATA_ONADDFOLLOW);
                allReceivedMsgs.Add((Net_OnAddFollow)msg);
                break;
            case NetOP.OnRequestFollow:
                Debug.Log("Get Friends");
                OnRequestFollow((Net_OnRequestFollow)msg);
                UpdateReceivedMessage(ENetworkMessage.DATA_ONREQUESTFOLLOW);
                allReceivedMsgs.Add((Net_OnRequestFollow)msg);
                break;
                //Todo change to Onupdatefollow
            case NetOP.UpdateFollow:
                Debug.Log("Update Friends");
                UpdateFollow((Net_UpdateFollow)msg);
                UpdateReceivedMessage(ENetworkMessage.DATA_ONUPDATEFOLLOW);
                allReceivedMsgs.Add((Net_UpdateFollow)msg);
                break;
            case NetOP.OnInitLobby:
                Debug.Log("Lobby initialized");
                break;
            case NetOP.SpellInput:
                Debug.Log("Received Spell Input from another Player");
                UpdateReceivedMessage(ENetworkMessage.DATA_SPELLINPUT);
                allReceivedMsgs.Add((Net_SpellInput)msg);
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
            self = new Account();
            self.ActiveConnection = olr.ConnectionId;
            self.Username = olr.Username;
            self.Discriminator = olr.Discriminator;

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
        // todo username and token working and messages should work
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

    #endregion

    #region Input related
    public void SendSpellInput(EInputButton eInputButton)
    {
        Net_SpellInput si = new Net_SpellInput();

        si.Token = token;
        si.spellId = eInputButton;

        SendServer(si);
    }

    public void SendMovementInput(EInputAxis eInputAxis, float axisvalue)
    {
        Net_MovementInput mi = new Net_MovementInput();

        mi.Token = token;
        mi.axis = eInputAxis;
        mi.axisValue = axisvalue;

        SendServer(mi);
    }
    #endregion

    #endregion

    #region Listeners
    public void On_Input_ButtonReleased(EInputButton eInputButton, EPlayerID ePlayerID)
    {
        if(AppStateManager.Instance.CurrentScene == EScene.GAME)
        {
            if(ePlayerID == EPlayerID.PLAYER_1)
            {
                switch (eInputButton)
                {
                    case EInputButton.CAST_SPELL_1:
                        SendSpellInput(eInputButton);
                        break;
                }
            }
        }
    }

    public void On_INPUT_JoystickMoved(EInputAxis eInputAxis, float axisvalue, EPlayerID ePlayerID)
    {
        if(ePlayerID == EPlayerID.PLAYER_1)
        {
            SendMovementInput(eInputAxis, axisvalue);
        }
    }
    #endregion
}
