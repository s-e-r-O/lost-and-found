using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : NetworkBehaviour
{
    [SerializeField]
    GameObject npcPrefab;
    [SerializeField]
    int npcTotal;
    [SerializeField]
    float radius;


    public override void OnStartServer()
    {
        GenerateNPCs();
    }

    [ServerCallback]
    public void GenerateNPCs()
    {
        for (int i = 0; i < npcTotal; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            Vector3 finalPosition;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                finalPosition = hit.position;
                var npc = Instantiate(npcPrefab, finalPosition, Quaternion.identity);
                NetworkServer.Spawn(npc);
            }

        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
