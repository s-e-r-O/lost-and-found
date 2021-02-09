using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarWheel : MonoBehaviour
{
    [SerializeField] float offset;
    [SerializeField] float radius;
    [SerializeField] LayerMask rideable;
    public bool isOnGround()
    {
        return Physics.CheckSphere(transform.position + transform.right * offset, radius, rideable);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.right * offset, radius);
    }
}