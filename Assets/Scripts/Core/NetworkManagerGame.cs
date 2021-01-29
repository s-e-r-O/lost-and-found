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
        bool isFinder = numPlayers == 0;
        if (isFinder)
        {
            GameObject player = Instantiate(finder);
            NetworkServer.AddPlayerForConnection(conn, player);
        } else
        {
            var transformPosition = GetStartPosition();
            GameObject player = Instantiate(car, transformPosition.position, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}
