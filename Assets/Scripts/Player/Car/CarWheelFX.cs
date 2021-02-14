using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows different car wheel effects (e.g. Trail when drifting)
/// </summary>
public class CarWheelFX
{
    private readonly TrailRenderer _tr = null;
    private readonly ParticleSystem _ps = null;
    private readonly float _driftThreshold = 0f;

    public CarWheelFX(TrailRenderer tr, ParticleSystem ps, float driftThreshold)
    {
        _tr = tr;
        _ps = ps;
        _driftThreshold = driftThreshold;
        if (_tr == null)
        {
            Debug.LogWarning("No trail renderer found for wheel effects");
        }
        if (_ps == null)
        {
            Debug.LogWarning("No particle system found for wheel effects");
        }
    }

    public void ShowDriftFX(float steeringValue)
    {
        if (_tr == null || _ps == null) { return; }
        var wasEmitting = _tr.emitting;
        _tr.emitting = Mathf.Abs(steeringValue) > _driftThreshold;
        Debug.Log(wasEmitting + " " + _tr.emitting);
        if (wasEmitting != _tr.emitting)
        {
            if (!wasEmitting)
            {
                Debug.Log("Started emitting");
                _ps.Play();
                // Started emitting
            }
            else 
            {
                Debug.Log("Stopped emitting");
                _ps.Stop();
                // Stopped emitting
            }
        }
    }
    public void HideDriftFX()
    {
        if (_tr == null || _ps == null) { return; }
        _tr.emitting = false;
        _ps.Stop();
    }
}
