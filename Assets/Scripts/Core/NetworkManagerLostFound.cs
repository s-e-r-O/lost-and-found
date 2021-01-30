using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NetworkManagerLostFound : NetworkManager
{
    [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLostFound roomPlayerPrefab;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
        if (SceneManager.GetActiveScene().name != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            NetworkRoomPlayerLostFound roomPlayerInstance = Instantiate(roomPlayerPrefab);
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }
}
