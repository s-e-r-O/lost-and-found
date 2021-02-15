using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles car movement
/// </summary>
[RequireComponent(typeof(CarController), typeof(Rigidbody))]
public class CarAccelerationBehavior : MonoBehaviour
{
    [Serializable]
    private struct Acceleration
    {
        public float Forward;
        public float Reverse;
    }

    private Rigidbody _rb = null;
    private CarController _controller = null;
    
    [SerializeField] private Acceleration _acceleration = new Acceleration() { Forward = 10f, Reverse = 8f};
    [SerializeField] public float MaxSpeed { get; private set; } = 100f;

    private float _currentAcceleration = 0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _controller = GetComponent<CarController>();
    }

    private void FixedUpdate()
    {
        Accelerate(_currentAcceleration);
    }

    public void Accelerate(float acceleration)
    {
        if (_controller.CarGroundChecker.OnGround)
        {
            var isForward = acceleration > 0f;
            Vector3 processedAcceleration = _rb.transform.forward * (acceleration * (isForward ? _acceleration.Forward : _acceleration.Reverse));
            _rb.AddForce(processedAcceleration);
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, MaxSpeed);
        }
    }

    [UsedImplicitly]
    public void OnAccelerate(InputValue value)
    {
        _currentAcceleration = value.Get<float>();
    }
}
