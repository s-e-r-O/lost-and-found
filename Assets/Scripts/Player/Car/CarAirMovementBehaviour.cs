using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CarController), typeof(Rigidbody))]
public class CarAirMovementBehaviour : MonoBehaviour
{
    [SerializeField] private float _horizontalRotationSpeed = 1f;
    [SerializeField] private float _verticalRotationSpeed = 2f;

    private Rigidbody _rb = null;
    private CarController _controller = null;

    private Vector2 _currentAirRotation = Vector2.zero;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _controller = GetComponent<CarController>();
    }
    void FixedUpdate()
    {
        Move(_currentAirRotation);
    }

    public void Move(Vector2 airRotation)
    {
        _rb.drag = _controller.CarGroundChecker.OnGround ? 1f : 0.3f;

        if (!_controller.CarGroundChecker.OnGround)
        {
            _rb.AddTorque(transform.up * airRotation.x * _horizontalRotationSpeed, ForceMode.VelocityChange);
            _rb.AddTorque(transform.right * airRotation.y * _verticalRotationSpeed, ForceMode.VelocityChange);
        }
    }

    public void OnMove(InputValue value)
    {
        _currentAirRotation = new Vector2(value.Get<Vector2>().x, _currentAirRotation.y);
    }
    public void OnRotate(InputValue value)
    {
        _currentAirRotation = new Vector2(_currentAirRotation.x, value.Get<float>());
    }
}
