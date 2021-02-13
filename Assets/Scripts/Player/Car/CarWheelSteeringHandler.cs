using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes care of rotating the wheel depending on steering value
/// </summary>
public class CarWheelSteeringHandler
{
    private float _steeringRawValue = 0f;
    private float _steeringValue = 0f;
    private bool _isDrifting = false;
    private bool _shouldRotate = false;
    private Transform _transform = null;
    private CarWheelFX _fx = null;
    private CarWheelGroundChecker _groundChecker = null;
    public CarWheelSteeringHandler(Transform transform, bool shouldRotate, CarWheelFX fx, CarWheelGroundChecker groundChecker)
    {
        _transform = transform;
        _fx = fx;
        _shouldRotate = shouldRotate;
        _groundChecker = groundChecker;
    }

    public void Update()
    {
        RotateWheel();
    }

    private void RotateWheel()
    {
        if (!_shouldRotate) { return; }
        _transform.localRotation = Quaternion.Slerp(_transform.localRotation, 
            Quaternion.Euler(new Vector3(0f, -_steeringRawValue * 45f, 0f)), 
            Time.deltaTime * 5f);
        //_transform.Rotate(_transform.up, _steeringRawValue * 30f);
    }

    /// <summary>
    /// Saves steering values for this wheel
    /// </summary>
    /// <param name="rawValue">The steering value without considering car speed</param>
    /// <param name="processedValue">The actual steering value (Considering car speed)</param>
    /// <param name="isDrifting">Whether the car is drifting or not</param>
    public void SetSteeringValues(float rawValue, float processedValue, bool isDrifting)
    {
        _steeringRawValue = rawValue;
        _steeringValue = processedValue;
        _isDrifting = isDrifting;
        if (_groundChecker.OnGround && _isDrifting)
        {
            _fx.ShowDriftFX(_steeringValue);
        } 
        else
        {
            _fx.HideDriftFX();
        }
    }
}
