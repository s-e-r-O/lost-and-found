using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGamePlayerLostFound : NetworkBehaviour
{
    [SyncVar]
    private string displayName = "";

    [SyncVar]
    private string playerType;

    public CinemachineVirtualCamera playerCameraPrefab;

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
            var playerCamera = Instantiate(playerCameraPrefab);
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
        playerType = type;
    }

    //public void InitializeCamera()
    //{
    //    RpcInitializeCamera();
    //    if (hasAuthority)
    //    {
    //        var playerCamera = Instantiate(playerCameraPrefab);
    //        playerCamera.Follow = transform;
    //    }
    //}

    //[ClientRpc]
    //public void RpcInitializeCamera()
    //{
    //    Debug.Log("Initializing");
    //    if (hasAuthority)
    //    {
    //        var playerCamera = Instantiate(playerCameraPrefab);
    //        playerCamera.Follow = transform;
    //    }
    //}
}
