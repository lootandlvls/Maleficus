using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorTutorial : NetworkBehaviour
{

    // got active on server
    public override void OnStartServer()
    {
        base.OnStartServer();


    }

    // Owning client is spawned
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();


    }

    // On every client
    public override void OnStartClient()
    {
        base.OnStartClient();


    }


    // Better use OnStartAuthority
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

    }

    
    public override void OnStopAuthority()
    {
        base.OnStopAuthority();

    }

    [Client] // only runs on the client
    public void Client_UpdateInput()
    {
        if (isLocalPlayer) // only counts for PlayerPrefab. Don't use!
        {

        }

        if (base.hasAuthority) // Correct is locally controlled
        {

        }
    }

    [Command] // Send to server
    public void Cmd_UpdateInput()
    {

        //NetworkIdentity networkIdentity;
        //networkIdentity.connectionToClient;
        //networkIdentity.connectionToServer;
    }

    [ClientRpc] // Send to all clients
    public void Rpc_UpdateInput()
    {

    }

    [TargetRpc] // From Server to specific client
    public void Target_ChangeColor(NetworkConnection networkConnection)
    {

    }
}
