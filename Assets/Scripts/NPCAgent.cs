using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAgent : NetworkBehaviour
{
    [SerializeField]
    private float walkRadius = 10f;

    private NavMeshAgent agent;

    [SerializeField]
    private Animator anim;

    private float remainingDistance;

    [HideInInspector]
    [SyncVar(hook=nameof(OnRandomDirectionChange))]
    public Vector3 randomDirection;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void OnStartClient()
    {
        StartCoroutine(SetParentRoutine());
    }

    [ClientCallback]
    private IEnumerator SetParentRoutine()
    {
        // Adding a delay so NavAgent initializes properly
        yield return new WaitForSeconds(1f);
        NPCSpawner spawner;
        if ((spawner = FindObjectOfType<NPCSpawner>()) != null){
            transform.parent = spawner.transform;
        }
    }

    public override void OnStartServer()
    {
        StartCoroutine(RoamAround());
    }

    // Update is called once per frame
    void Update()
    {
        remainingDistance = agent.remainingDistance;
        anim.SetFloat("remainingDistance",remainingDistance);
    }
    
    [ServerCallback]
    IEnumerator RoamAround()
    {
        while(true)
        {
            randomDirection = Random.insideUnitSphere * walkRadius + transform.position;
            //RpcSetDestination(randomDirection);
            yield return new WaitForSeconds(Random.Range(3f, 10f));
        }
    }

    [ClientCallback]
    private void OnRandomDirectionChange(Vector3 oldValue, Vector3 newValue)
    {
        if(NavMesh.SamplePosition(newValue, out NavMeshHit hit, walkRadius, 1))
        {
            Vector3 finalPosition = hit.position;
            agent.SetDestination(finalPosition);
        }
    }
}
