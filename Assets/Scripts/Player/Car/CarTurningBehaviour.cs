using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CarController), typeof(Rigidbody), typeof(CarAccelerationBehavior))]
public class CarTurningBehaviour : MonoBehaviour
{
    private Rigidbody _rb = null;
    private CarController _controller = null;
    private CarAccelerationBehavior _carAccelerationBehavior = null;

    private CarWheel[] _wheels = null;

    [SerializeField] private float _turningSpeed = 5f;
    [SerializeField] private float _driftingTurningSpeed = 10f;

    private bool _isDrifting = false;
    private float _steering = 0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _controller = GetComponent<CarController>();
        _wheels = GetComponentsInChildren<CarWheel>();
        _carAccelerationBehavior = GetComponent<CarAccelerationBehavior>();
    }

    private void FixedUpdate()
    {
        Turn(_steering, _isDrifting);
    }

    public void Turn(float steering, bool isDrifting)
    {
        if (_controller.CarGroundChecker.OnGround)
        {
            var isMovingForward = Vector3.Dot(_rb.transform.forward, _rb.velocity) >= 0f;
            
            float processedSteering = (isMovingForward ? 1 : -1) * steering * (isDrifting ? _driftingTurningSpeed : _turningSpeed) * (_rb.velocity.magnitude / (_carAccelerationBehavior.MaxSpeed));
            
            _rb.AddTorque(_rb.transform.up * processedSteering, ForceMode.VelocityChange);
            foreach (var wheel in _wheels)
            {
                wheel.WheelSteeringHandler.SetSteeringValues(steering, processedSteering, isDrifting);
            }
        } 
        else
        {
            foreach (var wheel in _wheels)
            {
                wheel.WheelSteeringHandler.SetSteeringValues(steering, steering, isDrifting);
            }
        }
    }

    public void OnMove(InputValue value)
    {
        _steering = value.Get<Vector2>().x;
    }

    public void OnDrift(InputValue value)
    {
        _isDrifting = value.Get<float>() != 0f;
    }
}
