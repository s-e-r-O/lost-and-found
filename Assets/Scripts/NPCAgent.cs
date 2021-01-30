using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAgent : NetworkBehaviour
{
    [SerializeField]
    float walkRadius = 10f;

    NavMeshAgent agent;

    [SerializeField]
    Animator anim;

    [SyncVar]
    float remainingDistance;
    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(RoamAround());
        //StartCoroutine(GetClosestTarget());
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            remainingDistance = agent.remainingDistance;
        }
        anim.SetFloat("remainingDistance",remainingDistance);
        //if (currentTarget != null)
        //{
        //    agent.SetDestination(currentTarget.position);
        //}
    }
    
    [ServerCallback]
    IEnumerator RoamAround()
    {
        while(true)
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
