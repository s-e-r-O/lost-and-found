using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes care of checking if the wheel is on rideable ground
/// </summary>
public class CarWheelGroundChecker
{
    public bool OnGround { get; private set; } = false;

    private readonly Transform _transform;
    private readonly float _wheelRadius;
    private readonly float _collisionRadius;
    private readonly LayerMask _rideableMask;

    public CarWheelGroundChecker(Transform transform, float wheelRadius, float collisionRadius, LayerMask rideableMask)
    {
        _transform = transform;
        _wheelRadius = wheelRadius;
        _collisionRadius = collisionRadius;
        _rideableMask = rideableMask;
    }

    public void Update()
    {
        CheckOnGround();
    }

    private void CheckOnGround()
    {
        var wheelBorder = _transform.position - _transform.up * _wheelRadius;
        OnGround = Physics.CheckSphere(wheelBorder, _collisionRadius, _rideableMask);
    }
}
