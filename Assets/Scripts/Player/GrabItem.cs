using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItem : NetworkBehaviour
{
    [SerializeField]
    LayerMask itemMask;
    [SerializeField]
    Transform grabTransform;
    [SerializeField]
    float grabRadius;
    private void Update()
    {
        if (isLocalPlayer)
        {
            CheckNearItems();
        }
    }
    
    [ClientCallback]
    void CheckNearItems()
    {
        var cols = Physics.OverlapSphere(grabTransform.position, grabRadius, itemMask);
        if (cols.Length > 0)
        {
            Debug.Log($"I got some ({cols.Length})");
            if (Input.GetKeyDown(KeyCode.X))
            {
                foreach (var col in cols)
                {
                    CollectItem(col.GetComponentInParent<ItemPlayer>().gameObject);
                }
            }
        }
    }
    [Command]
    void CollectItem(GameObject item)
    {
        item.GetComponent<ItemPlayer>().Collect();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(grabTransform.position, grabRadius);
    }
    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponentInParent<ItemPlayer>();
        if (item != null)
        {
            Debug.Log($"Got 1 item!! {item.gameObject.name}");
        }
    }
}
