using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCameraInit : NetworkBehaviour
{
    public CinemachineVirtualCamera playerCameraPrefab;
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        if (isLocalPlayer)
        {
            var playerCamera = Instantiate(playerCameraPrefab);
            playerCamera.Follow = transform;
        }
    }
}
