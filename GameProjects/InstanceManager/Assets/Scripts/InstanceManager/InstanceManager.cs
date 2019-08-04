using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class InstanceManager : MonoBehaviour
{
    private const int MAX_USER = 10000;
    private const int PORT = 9999;
    private const int CLIENT_PORT = 26002;
    private const int BYTE_SIZE = 1024;


    private byte reliableChannel;
    private int hostId;

    private bool isStarted;
    private byte error;

    private Mongo_InstanceManager db;

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
        db = new Mongo_InstanceManager();
        db.Init();

        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);


        // Server only code
        hostId = NetworkTransport.AddHost(topo, PORT, null);

        Debug.Log(string.Format("Opening connection on port {0} and clientport {1}", PORT, CLIENT_PORT));
        isStarted = true;
    }

    public void UpdateMessagePump()
    {
        if (!isStarted)
        {
            return;
        }

        int recHostId;      // is this from web? standalone? not really needed bc masterserver is the only one to connect with the Instance Manager
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
                Debug.Log(string.Format("Master Server {0} has connected through host 2!", connectionId));
                break;
            case NetworkEventType.DisconnectEvent:
                DisconnectEvent(connectionId);
                break;
            case NetworkEventType.DataEvent:
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms);

                OnData(connectionId, channelId, msg);
                break;
            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type");
                break;
        }
    }

    #region OnData
    private void OnData(int cnnId, int channelId, NetMsg msg)
    {
        Debug.Log("receiverd a message of type " + msg.OP);

        switch (msg.OP)
        {
            case NetOP.None:
                Debug.Log("Unexpected NetOP");
                break;
            case NetOP.InitLobby:
                InitLobby((Net_InitLobby) msg);
                break;
        }
    }
    private void DisconnectEvent(int connectionId)
    {
        Debug.Log(string.Format("Master Server has disconnected!", connectionId));

    }

    private void InitLobby(Net_InitLobby il)
    {
        Model_Lobby newLobby = new Model_Lobby();
        newLobby = db.FindLobbyByToken(il.Token);
        if(newLobby != null)
        {
            Process.Start(new ProcessStartInfo(
            "/home/ubuntu/docker/Instance/Maleficus.x86_64",
            "--no-first-run")
             { UseShellExecute = false });
        }
    }
    #endregion

    #region Send
    public void SendMasterServer(int cnnId, NetMsg msg)
    {
        // this is where we hold our data
        byte[] buffer = new byte[BYTE_SIZE];

        // this is where we put our data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostId, cnnId, reliableChannel, buffer, BYTE_SIZE, out error);
    }
    #endregion

    // for launching a programm in ubuntu
    public static void ExecuteCommand()
    {
        Process proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = "/docker/Instance/Instance";
        proc.StartInfo.Arguments = "";
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();

        /*while (!proc.StandardOutput.EndOfStream)
        {
            Console.WriteLine(proc.StandardOutput.ReadLine());
        }*/
    }

}
