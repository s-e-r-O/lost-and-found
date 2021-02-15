using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CarController))]
public class CarJumpBehaviour: MonoBehaviour
{
    [SerializeField] private float jumpForce = 7f;
    private Rigidbody _rb = null;
    private CarController _controller = null;

    public static event Action OnJumped;

    private void Awake()
    {
        _controller = GetComponent<CarController>();
        _rb = GetComponent<Rigidbody>();
    }

    [UsedImplicitly]
    public void OnJump()
    {
        if (_controller.CarGroundChecker.OnGround)
        {
            _rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            OnJumped?.Invoke();
        }
    }
}
