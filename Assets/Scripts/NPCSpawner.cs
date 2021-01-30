using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour
{
    [SerializeField]
    GameObject npcPrefab;
    [SerializeField]
    int npcTotal;

    public override void OnStartServer()
    {
        base.OnStartServer();
        for(int i = 0; i < npcTotal; i++)
        {

        }
            NetworkServer.SpawnObjects();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
