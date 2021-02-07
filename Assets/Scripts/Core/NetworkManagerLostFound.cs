using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NetworkManagerLostFound : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene][SerializeField] private string menuScene = string.Empty;
    [Scene][SerializeField] private string levelScene = string.Empty;
    [SerializeField] private float sceneTransitionDelay = 2f;


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


    private bool isChangingScene = false;

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
        if (networkSceneName != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Debug.Log(conn.connectionId);
        if (networkSceneName == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;
            NetworkRoomPlayerLostFound roomPlayerInstance = Instantiate(roomPlayerPrefab);
            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            NotifyPlayersOfReadyState();
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
        gameSeconds = 0;
        startCounting = false;
        foreach (var player in GamePlayers)
        {
            player.TargetClean();
        }
        GamePlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.LobbyState = IsReadyToStart();
        }
    }
    public LobbyState IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return LobbyState.NOT_ENOUGH_PLAYERS; }
        int finders = 0;
        int items = 0;
        foreach (var player in RoomPlayers)
        {
            if (player.PlayerType == "FINDER") { finders++; }
            if (player.PlayerType == "ITEM") { items++; }
        }
        if ((finders + items) < numPlayers) { return LobbyState.PLAYERS_STILL_CHOOSING; }
        if (finders == 0 || items == 0) { return LobbyState.EMPTY_TEAM; }
        return LobbyState.READY;
    }

    public void StartGame()
    {
        if (networkSceneName == menuScene)
        {
            if (IsReadyToStart() != LobbyState.READY) { return; }
            ServerChangeScene(levelScene);
        }
    }

    public void LeaveGame(bool isServer)
    {
        if (isServer)
        {
            StopHost();
        }
        else
        {
            StopClient();
        }
    }

    public void EndGame()
    {

    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
    }

    private IEnumerator Delay(Action action, float delay = 1f)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        StartCoroutine(Delay(() => { SceneTransition.Instance.Open();
            isChangingScene = false;
        }, sceneTransitionDelay / 2f));
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (isChangingScene) { return; }
        float delay = 0f;
        //From menu to game
        if (networkSceneName == menuScene && newSceneName.StartsWith(levelScene))
        {
            delay = sceneTransitionDelay / 2f;
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {

                var conn = RoomPlayers[i].connectionToClient;
                RoomPlayers[i].TargetCloseTransition();
                RoomPlayers[i].TargetHideUI();
            }
        } 
        else
        {
            delay = sceneTransitionDelay / 2f;
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                RoomPlayers[i].TargetCloseTransition();
            }
            StartCoroutine(Delay(() => { 
                for(int i = GamePlayers.Count - 1; i >= 0; i--)
                {
                    NetworkServer.Destroy(GamePlayers[i].gameObject);
                }                
                base.ServerChangeScene(newSceneName);
                isChangingScene = true;
            }, delay));
                
            return;
        }
        StartCoroutine(Delay(() => { base.ServerChangeScene(newSceneName);
            isChangingScene = true;
        }, delay));
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith(levelScene))
        {
            gameSeconds = gameDurationSeconds;
            int itemsC = RoomPlayers.Where(g => g.PlayerType == "ITEM").Count();
            List<Vector3> alreadyPosition = new List<Vector3>();
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var position = GetStartPosition().position;
                while (alreadyPosition.Contains(position))
                {
                    position = GetStartPosition().position;
                }
                alreadyPosition.Add(position);
                int index = Random.Range(0, gamePlayerPrefabs.Length);
                var gamePlayerInstance = Instantiate(gamePlayerPrefabs[index], position, Quaternion.identity);
                NetworkServer.Spawn(gamePlayerInstance.gameObject, conn);
                gamePlayerInstance.SetPlayerValues(RoomPlayers[i].DisplayName, RoomPlayers[i].PlayerType, RoomPlayers[i].IsLeader);
                gamePlayerInstance.GameSeconds = gameSeconds;
                gamePlayerInstance.ItemCounter = itemsC;
                gamePlayerInstance.StartDetectingCollissions();
            }
            startCounting = true;
            StartCoroutine("Counter");
        } 
        else
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                RoomPlayers[i].TargetShowUI();
            }
        }
        base.OnServerSceneChanged(sceneName);
        isChangingScene = false;
    }


    public void CheckGameState()
    {
        int itemsCaught = 0;
        int itemsTotal = 0;
        foreach (var player in GamePlayers)
        {
            if (player.PlayerType == "ITEM")
            {
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
            player.StopDetectingCollissions();
            player.TargetGameOver(winner);

        }
    }

    private IEnumerator Counter()
    {
        while (gameSeconds > 0 && startCounting)
        {
            yield return new WaitForSeconds(1f);
            gameSeconds--;
            foreach (var player in GamePlayers)
            {
                player.GameSeconds = gameSeconds;
            }
        }
        if (gameSeconds <= 0)
        {
            CheckGameState();
        }
    }



    public void PlayAgain()
    {
        if (networkSceneName == levelScene)
        {
            ServerChangeScene(menuScene);
        }
    }
}
