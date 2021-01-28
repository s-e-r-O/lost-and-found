using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAgent : NetworkBehaviour
{
    NavMeshAgent agent;
    List<Transform> targets;
    Transform currentTarget;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targets = new List<Transform>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(GetClosestTarget());
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
        targets.Add(target);
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
}
