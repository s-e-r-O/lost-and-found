using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAgentOffline : MonoBehaviour
{

    [SerializeField]
    float walkRadius = 10f;

    NavMeshAgent agent;

    [SerializeField]
    Animator anim;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        StartCoroutine(RoamAround());
    }

    private void Update()
    {
        anim.SetFloat("remainingDistance", agent.remainingDistance);
    }

    IEnumerator RoamAround()
    {
        while (true)
        {
            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection += transform.position;
            NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, walkRadius, 1);
            Vector3 finalPosition = hit.position;
            agent.SetDestination(finalPosition);
            yield return new WaitForSeconds(Random.Range(3f, 10f));
        }
    }
}
