using JetBrains.Annotations;
using UnityEngine;

public class CarWheel : MonoBehaviour
{
    [SerializeField] float offset = 0f;
    [SerializeField] float radius = 0f;
    [SerializeField] float steeringForceTrailTreshold = 0.5f;
    [SerializeField] LayerMask rideable = 0;
    [SerializeField] bool shouldRotate = false;

    private TrailRenderer tr = null;

    public CarWheelGroundChecker WheelGroundChecker { get; private set; }
    public CarWheelSteeringHandler WheelSteeringHandler{ get; private set; }
    public CarWheelFX WheelEffects { get; private set; }
    

    [UsedImplicitly]
    private void Awake()
    {
        tr = GetComponentInChildren<TrailRenderer>();

        WheelGroundChecker = new CarWheelGroundChecker(transform, offset, radius, rideable);
        WheelEffects = new CarWheelFX(tr, steeringForceTrailTreshold);
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - transform.up * offset, radius);
    }
}