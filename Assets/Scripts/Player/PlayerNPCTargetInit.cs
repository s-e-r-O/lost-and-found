using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNPCTargetInit : NetworkBehaviour
{

    public override void OnStartServer()
    {
        base.OnStartServer();
        var npcs = FindObjectsOfType<NPCAgent>();
        foreach(var npc in npcs)
        {
            //npc.SetTarget(transform);
        }
    }
}
