using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows different car wheel effects (e.g. Trail when drifting)
/// </summary>
public class CarWheelFX
{
    private readonly TrailRenderer _tr = null;
    private readonly float _driftThreshold = 0f;

    public CarWheelFX(TrailRenderer tr, float driftThreshold)
    {
        _tr = tr;
        _driftThreshold = driftThreshold;
        if (_tr == null)
        {
            Debug.LogWarning("No trail renderer found for wheel effects");
        }
    }

    public void ShowDriftFX(float steeringValue)
    {
        if (_tr == null) { return; }
        var wasEmitting = _tr.emitting;
            _tr.emitting = Mathf.Abs(steeringValue) > _driftThreshold;
        if (wasEmitting != _tr.emitting)
        {
            if (!wasEmitting)
            {
                Debug.Log("Started emitting");
                // Started emitting
            }
            else 
            {
                Debug.Log("Stopped emitting");
                // Stopped emitting
            }
        }
    }
    public void HideDriftFX()
    {
        if (_tr == null) { return; }
        Debug.Log("Stopped emitting");
        _tr.emitting = false;
    }
}
