using JetBrains.Annotations;
using UnityEngine;

public class CarWheel : MonoBehaviour
{
    [SerializeField] Transform pointOfContact = null;
    [SerializeField] float radius = 0f;
    [SerializeField] float steeringForceTrailTreshold = 0.5f;
    [SerializeField] LayerMask rideable = 0;
    [SerializeField] bool shouldRotate = false;

    public CarWheelGroundChecker WheelGroundChecker { get; private set; }
    public CarWheelSteeringHandler WheelSteeringHandler{ get; private set; }
    public CarWheelFX WheelEffects { get; private set; }
    

    [UsedImplicitly]
    private void Awake()
    {
        var tr = GetComponentInChildren<TrailRenderer>();
        var ps = GetComponentInChildren<ParticleSystem>();

        WheelGroundChecker = new CarWheelGroundChecker(transform, pointOfContact, radius, rideable);
        WheelEffects = new CarWheelFX(tr, ps, steeringForceTrailTreshold);
        WheelSteeringHandler = new CarWheelSteeringHandler(transform, shouldRotate, WheelEffects, WheelGroundChecker);
    }

    [UsedImplicitly]
    private void Update()
    {
        WheelGroundChecker.Update();
        WheelSteeringHandler.Update();
    }

    [UsedImplicitly]
    void OnDrawGizmos()
    {
        if (pointOfContact != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointOfContact.position, radius);
        }
    }
}