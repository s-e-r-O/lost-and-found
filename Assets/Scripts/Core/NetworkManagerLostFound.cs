using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NetworkManagerLostFound : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private string menuScene = string.Empty;
    [SerializeField] private string levelScene = string.Empty;

    
    [SerializeField] private int gameDurationSeconds = 60;
    private int gameSeconds;
    private bool startCounting;

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
            //if (numPlayers < minPlayers) { StopServer(); }
            //int finders = 0;
            //int items = 0;
            //foreach (var gplayer in GamePlayers)
            //{
            //    if (gplayer.PlayerType == "FINDER") { finders++; }
            //    if (gplayer.PlayerType == "ITEM") { items++; }
            //}
            //return finders > 0 && items > 0 && (finders + items) == numPlayers;
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
        gameSeconds = 0;
        startCounting = false;
        foreach(var player in GamePlayers)
        {
            player.TargetClean();
        }
        GamePlayers.Clear();
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

    public void EndGame()
    {
        
        //if (SceneManager.GetActiveScene().name == menuScene)
        //{
        //    if (!IsReadyToStart()) { return; }
        //    ServerChangeScene(levelScene);
        //}
    }

    public override void ServerChangeScene(string newSceneName)
    {

        //From menu to game
        if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith(levelScene))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                //Debug.Log(GetStartPosition());
                List<Vector3> alreadyPosition = new List<Vector3>();
                var position = GetStartPosition().position;
                while (alreadyPosition.Contains(position))
                {
                    position = GetStartPosition().position;
                }
                alreadyPosition.Add(position);
                int index = Random.Range(0, gamePlayerPrefabs.Length);
                var gamePlayerInstance = Instantiate(gamePlayerPrefabs[index], position, Quaternion.identity);
                gamePlayerInstance.SetPlayerValues(RoomPlayers[i].DisplayName, RoomPlayers[i].PlayerType);
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject, true);
                gamePlayerInstance.TargetStartTransition();
            }
        }

        StartCoroutine(Delay(newSceneName));
    }

    private IEnumerator Delay(string newSceneName)
    {
        yield return new WaitForSeconds(1f);

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith(levelScene))
        {
            gameSeconds = gameDurationSeconds;
            int itemsC = GamePlayers.Where(g => g.PlayerType == "ITEM").Count();
            //List<Vector3> alreadyPosition = new List<Vector3>(); 
            for (int i = GamePlayers.Count - 1; i >= 0; i--)
            {
                ////GamePlayers[i].RpcInitializeCamera();
                //var position = GetStartPosition().position;
                //while (alreadyPosition.Contains(position))
                //{
                //    position = GetStartPosition().position;
                //}
                //alreadyPosition.Add(position);
                //GamePlayers[i].transform.position = position;
                GamePlayers[i].StartDetectingCollissions();
                GamePlayers[i].TargetSetUpGraphics();
                GamePlayers[i].GameSeconds = gameSeconds;
                GamePlayers[i].ItemCounter = itemsC;
                //var conn = RoomPlayers[i].connectionToClient;
                //Debug.Log(GetStartPosition());
                //var gamePlayerInstance = Instantiate(gamePlayerPrefab);
                //gamePlayerInstance.SetPlayerValues(RoomPlayers[i].DisplayName, RoomPlayers[i].PlayerType);
                ////NetworkServer.Destroy(conn.identity.gameObject);
                //Debug.Log($"Generating for {conn.connectionId}...", gamePlayerInstance.gameObject);
                //NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject, true);
                //Debug.Log($"Generated for {conn.connectionId}",gamePlayerInstance.gameObject);
            GamePlayers[i].TargetEndTransition();
            }
            startCounting = true;
            StartCoroutine("Counter");

        }
        base.OnServerSceneChanged(sceneName);
    }


    public void CheckGameState()
    {
        int itemsCaught = 0;
        int itemsTotal = 0;
        foreach(var player in GamePlayers)
        {
            if (player.PlayerType == "ITEM") {
                itemsTotal++;
                if (player.IsCaught)
                {
                    itemsCaught++;
                }
            }
        }
        foreach (var player in GamePlayers)
        {
            player.ItemCounter = itemsTotal - itemsCaught;
        }
        if (itemsCaught == itemsTotal)
        {
            NotifyGameOver("FINDER");
        }
        else if (gameSeconds <= 0)
        {
            NotifyGameOver("ITEM");
        }
    }

    public void NotifyGameOver(string winner)
    {
        startCounting = false;
        StopCoroutine("Counter");
        foreach (var player in GamePlayers)
        {
            Debug.Log("Notifying");
            player.TargetGameOver(winner);         

        }
    }

    private IEnumerator Counter()
    {
        while (gameSeconds > 0 && startCounting)
        {
            yield return new WaitForSeconds(1f);
            gameSeconds--;
            foreach(var player in GamePlayers)
            {
                player.GameSeconds = gameSeconds;
            }
        }
        if (gameSeconds <= 0)
        {
            CheckGameState();
        }
    }
}
