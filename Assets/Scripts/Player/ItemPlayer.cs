using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlayer : NetworkBehaviour
{
    [ServerCallback]
    public void Collect()
    {
        Debug.Log("Someone collected", gameObject);
        var identity = GetComponent<NetworkIdentity>();
        Collected();
    }

    [ClientRpc]
    public void Collected()
    {
        gameObject.transform.position = Vector3.zero;
    }
}
