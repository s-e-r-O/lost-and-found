using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Checks if the car has any wheel on the ground
/// </summary>
public class CarGroundChecker
{
    private readonly CarWheel[] _wheels = null;

    public CarGroundChecker(CarWheel[] wheels)
    {
        _wheels = wheels;
    }

    public bool OnGround { get
        {
            if (_wheels == null)
            {
                return false;
            }
            return _wheels.Any(wheel => wheel.WheelGroundChecker.OnGround);
        }
    }
}
