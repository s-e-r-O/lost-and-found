using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerGame : NetworkManager
{

    public GameObject finder;
    public GameObject car;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(numPlayers == 0 ? finder : car);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}
