using Cinemachine;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGamePlayerLostFound : NetworkBehaviour
{
    [SerializeField] Outline outline;

    [SyncVar]
    private string displayName = "";

    [SyncVar]
    public string PlayerType;

    [SyncVar]
    private bool shouldDetectCollissions = false;


    [SyncVar]
    public bool IsCaught = false;


    //public bool InGame()
    //{
    //    return PlayerType == "FINDER" || (PlayerType == "ITEM" && !IsCaught);
    //}

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
            playerCamera = Instantiate(playerCameraPrefab);
            playerCamera.Follow = transform;
            DontDestroyOnLoad(playerCamera.gameObject);
        }
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetPlayerValues(string name, string type)
    {
        displayName = name;
        PlayerType = type;
    }

    [Server]
    public void StartDetectingCollissions()
    {
        shouldDetectCollissions = true;
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
                    other.RpcPlayerCaught();
                }
            }
        }
    }

    //[Server]
    //private void PlayerCaught()
    //{
    //    IsCaught = true;
    //    // Test some things
    //    RpcPlayerCaught();
    //}

    [ClientRpc]
    private void RpcPlayerCaught()
    {
        gameObject.SetActive(false);
        IsCaught = true;
        Room.CheckGameState();
        if (hasAuthority)
        {
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
    public void TargetSetUpGraphics()
    {
        if (hasAuthority)
        {
            Debug.Log("Set Up Graphics");
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

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
