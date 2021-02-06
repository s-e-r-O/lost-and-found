using Cinemachine;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGamePlayerLostFound : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject typeFinderIcon;
    [SerializeField] GameObject typeItemIcon;
    [SerializeField] TMP_Text clockTime;
    [SerializeField] TMP_Text itemCounter;
    [SerializeField] GameOverMenu gameOverUI;
    [SerializeField] TMP_Text foundMessage;
    [SerializeField] PlayerTagUI tagPrefab;

    [SyncVar(hook = nameof(HandleGameSecondsChanged))]
    public int GameSeconds;
    [SyncVar(hook = nameof(HandleItemsCounterChanged))]
    public int ItemCounter;

    [Header("Others")]
    [SerializeField] GameObject graphics;
    [SerializeField] Outline outline;

    [SyncVar]
    private string displayName = "";

    [SyncVar]
    private bool isLeader = false;

    [SyncVar(hook = nameof(HandlePlayerTypeChanged))]
    public string PlayerType;

    [SyncVar]
    private bool shouldDetectCollissions = false;


    [SyncVar]
    public bool IsCaught = false;

    [SerializeField] private CinemachineVirtualCamera playerCameraPrefab;
    private CinemachineVirtualCamera playerCamera;

    private NetworkManagerLostFound room;
    private NetworkManagerLostFound Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLostFound;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        Room.GamePlayers.Add(this);

        if (hasAuthority)
        {
            outline.OutlineWidth = 0;
        }
        else
        {
            GameObject tagsParent = GameObject.Find("Tags");
            var playerTag = Instantiate(tagPrefab, tagsParent.transform);
            var currentType = Room.RoomPlayers.First(r => r.hasAuthority).PlayerType;
            if (PlayerType == currentType)
            {
                outline.OutlineColor = Color.green;
                playerTag.Init(transform, displayName, Color.green);
                SetLayerRecursively(gameObject, 10);
            }
            else
            {
                outline.OutlineColor = Color.red;
                playerTag.Init(transform, displayName, Color.red);
                SetLayerRecursively(gameObject, 9);
            }
        }


        playerCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (playerCamera == null)
        {
            playerCamera = Instantiate(playerCameraPrefab);
        }
        if (hasAuthority)
        {
            playerCamera.Follow = transform;
        }
        DontDestroyOnLoad(playerCamera.gameObject);
    }

    public override void OnStartAuthority()
    {
        gameUI.SetActive(true);
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetPlayerValues(string name, string type, bool isLeader)
    {
        displayName = name;
        this.isLeader = isLeader;
        PlayerType = type;
    }

    [Server]
    public void StartDetectingCollissions()
    {
        shouldDetectCollissions = true;
    }

    [Server]
    public void StopDetectingCollissions()
    {
        shouldDetectCollissions = false;
    }

    [TargetRpc]
    public void TargetHasBeenFoundByRadar(NetworkGamePlayerLostFound finder)
    {
        if (PlayerType != "ITEM") { return; }

        StartCoroutine(ShowFoundMessage(finder.displayName));
    }

    IEnumerator ShowFoundMessage(string name)
    {
        foundMessage.gameObject.SetActive(true);
        foundMessage.text = $"{name}'s radar has found you!";
        yield return new WaitForSeconds(3f);

        foundMessage.gameObject.SetActive(false);
    }

    public void OnPlayerCollision(NetworkGamePlayerLostFound other)
    {
        if (shouldDetectCollissions)
        {
            Debug.Log($"This object {name} has collided with this {other.name}");
            if (isServer)
            {
                if (PlayerType == "FINDER" && other.PlayerType == "ITEM")
                {
                    Debug.Log($"Bye bye {other.name}");
                    other.IsCaught = true;

                    other.RpcPlayerCaught();
                    Room.CheckGameState();
                }
            }
        }
    }

    [ClientRpc]
    private void RpcPlayerCaught()
    {
        graphics.SetActive(false);
        if (hasAuthority)
        {
            IsCaught = true;
            playerCamera.Follow = null;
            foreach (var player in Room.GamePlayers)
            {
                if (player.PlayerType == "ITEM" && !player.IsCaught)
                {
                    playerCamera.Follow = player.transform;
                }
            }
        }
    }

    [TargetRpc]
    public void TargetClean()
    {
        Destroy(playerCamera);
    }

    [TargetRpc]
    public void TargetStartTransition()
    {
        SceneTransition.Instance.Close();
    }


    [TargetRpc]
    public void TargetEndTransition()
    {
        SceneTransition.Instance.Open();
    }

    [ClientCallback]
    public void SetUpGraphics()
    {
        if (hasAuthority)
        {
            foreach (var player in Room.GamePlayers)
            {
                if (player == this)
                {
                    outline.OutlineWidth = 0;
                }
                else if (player.PlayerType == PlayerType)
                {
                    player.outline.OutlineColor = Color.green;
                    SetLayerRecursively(player.gameObject, 10);
                }
                else
                {
                    player.outline.OutlineColor = Color.red;
                    SetLayerRecursively(player.gameObject, 9);
                }
            }
        }

    }

    [TargetRpc]
    public void TargetGameOver(string winner)
    {
        gameOverUI.gameObject.SetActive(true);
        gameOverUI.SetWinner(winner, isLeader);
    }


    private void HandlePlayerTypeChanged(string oldValue, string newValue) => UpdateUI();
    private void HandleGameSecondsChanged(int oldValue, int newValue) => UpdateUI();
    private void HandleItemsCounterChanged(int oldValue, int newValue) => UpdateUI();

    private void UpdateUI()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.GamePlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateUI();
                    break;
                }
            }
            return;
        }

        typeFinderIcon.SetActive(PlayerType == "FINDER");
        typeItemIcon.SetActive(PlayerType == "ITEM");
        itemCounter.text = ItemCounter.ToString();
        string h = (GameSeconds / 60).ToString("00");
        string m = (GameSeconds % 60).ToString("00");
        clockTime.text = h + ":" + m;

    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child || child.gameObject.layer == 5)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
