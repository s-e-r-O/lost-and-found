using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarWheel : MonoBehaviour
{
    [SerializeField] float offset = 0f;
    [SerializeField] float radius = 0f;
    [SerializeField] float steeringForceTrailTreshold = 0.5f;
    [SerializeField] LayerMask rideable = 0;

    [HideInInspector] public WheelCollider wheelCollider = null;

    private TrailRenderer tr = null;

    private bool isOnGround = false;
    private float steeringForce = 0f;

    private void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
        tr = GetComponentInChildren<TrailRenderer>();
    }

    private void Update()
    {
        isOnGround = Physics.CheckSphere(transform.position + transform.up * offset, radius, rideable);
        tr.emitting = steeringForce >= steeringForceTrailTreshold && isOnGround;
    }

    public bool IsOnGround()
    {
        return isOnGround;
    }

    public void SetSteeringForce(float steeringForce)
    {
        this.steeringForce = Mathf.Abs(steeringForce);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up * offset, radius);
    }
}