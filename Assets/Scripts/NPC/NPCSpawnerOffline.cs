using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpawnerOffline : MonoBehaviour
{
    [SerializeField]
    GameObject npcPrefab;
    [SerializeField]
    int npcTotal;
    [SerializeField]
    float radius;

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }


    public void Generate()
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
                npc.transform.parent = transform;
            }

        }

    }
}
