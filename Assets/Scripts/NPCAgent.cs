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
    List<Transform> targets;
    Transform currentTarget;
    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        targets = new List<Transform>();
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
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }
    }

    [ServerCallback]
    public void SetTarget(Transform target)
    {
        //targets.Add(target);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkRadius);
    }

    IEnumerator GetClosestTarget() { 
    while(true)
        {
            yield return new WaitForSeconds(1);
            
            if (targets.Count > 1)
            {
                var minTarget = targets[0];
                var min = Vector3.Distance(transform.position, targets[0].position);
                for(int i = 1; i < targets.Count; i++)
                {
                    var distance = Vector3.Distance(transform.position, targets[i].position);
                    if (distance < min)
                    {
                        min = distance;
                        minTarget = targets[i];
                    }
                }
                //var path = new NavMeshPath();
                //var minTarget = targets[0];

                //agent.SetDestination(targets[0].position);
                //NavMesh.CalculatePath(transform.position, targets[0].position, NavMesh.AllAreas, path);
                //var min = agent.remainingDistance;
                //for(int i = 1; i < targets.Count; i++)
                //{
                //    agent.SetDestination(targets[i].position);
                //    NavMesh.CalculatePath(transform.position, targets[i].position, NavMesh.AllAreas, path);
                //    if (min < agent.remainingDistance)
                //    {
                //        min = agent.remainingDistance;
                //        minTarget = targets[i];
                //    }

                //}
                currentTarget = minTarget;

            } 
            else if (targets.Count == 1)
            {
                currentTarget = targets[0];
            }
        }
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

    [ClientRpc]
    void SetDestination(Vector3 position)
    {
        agent.SetDestination(position);
    }
}
