using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NetworkManagerLostFound : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private string menuScene = string.Empty;
    [SerializeField] private string levelScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLostFound roomPlayerPrefab;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLostFound[] gamePlayerPrefabs;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<NetworkRoomPlayerLostFound> RoomPlayers { get; } = new List<NetworkRoomPlayerLostFound>();
    public List<NetworkGamePlayerLostFound> GamePlayers { get; } = new List<NetworkGamePlayerLostFound>();

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
            bool isLeader = RoomPlayers.Count == 0;
            NetworkRoomPlayerLostFound roomPlayerInstance = Instantiate(roomPlayerPrefab);
            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLostFound>();
            RoomPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach(var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }
    public bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }
        int finders = 0;
        int items = 0;
        foreach (var player in RoomPlayers)
        {
            if (player.PlayerType == "FINDER") { finders++; }
            if (player.PlayerType == "ITEM") { items++; }
        }
        return finders > 0 && items > 0 && (finders + items) == numPlayers;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            if (!IsReadyToStart()) { return; }
            ServerChangeScene(levelScene);
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {

        //From menu to game
        if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith(levelScene))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                Debug.Log(GetStartPosition());
                int index = Random.Range(0, gamePlayerPrefabs.Length);
                var gamePlayerInstance = Instantiate(gamePlayerPrefabs[index]);
                gamePlayerInstance.SetPlayerValues(RoomPlayers[i].DisplayName, RoomPlayers[i].PlayerType);
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject, true);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith(levelScene))
        {
            for (int i = GamePlayers.Count - 1; i >= 0; i--)
            {
                //GamePlayers[i].RpcInitializeCamera();
                GamePlayers[i].transform.position = GetStartPosition().position;
                //var conn = RoomPlayers[i].connectionToClient;
                //Debug.Log(GetStartPosition());
                //var gamePlayerInstance = Instantiate(gamePlayerPrefab);
                //gamePlayerInstance.SetPlayerValues(RoomPlayers[i].DisplayName, RoomPlayers[i].PlayerType);
                ////NetworkServer.Destroy(conn.identity.gameObject);
                //Debug.Log($"Generating for {conn.connectionId}...", gamePlayerInstance.gameObject);
                //NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject, true);
                //Debug.Log($"Generated for {conn.connectionId}",gamePlayerInstance.gameObject);
            }
        }
        base.OnServerSceneChanged(sceneName);
    }
}
