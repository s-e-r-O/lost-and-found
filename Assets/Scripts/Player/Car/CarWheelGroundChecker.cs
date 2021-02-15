using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if the wheel is touching rideable ground
/// </summary>
public class CarWheelGroundChecker
{
    public bool OnGround { get; private set; } = false;

    private readonly Transform _transform;
    private readonly Transform _pointOfContact;
    private readonly float _collisionRadius;
    private readonly LayerMask _rideableMask;

    public CarWheelGroundChecker(Transform transform, Transform pointOfContact, float collisionRadius, LayerMask rideableMask)
    {
        _transform = transform;
        _pointOfContact = pointOfContact;
        _collisionRadius = collisionRadius;
        _rideableMask = rideableMask;
    }

    public void Update()
    {
        CheckOnGround();
    }

    private void CheckOnGround()
    {
        OnGround = Physics.CheckSphere(_pointOfContact.position, _collisionRadius, _rideableMask);
    }
}
