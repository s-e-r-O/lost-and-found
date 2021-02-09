using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private float finderSpeedMultiplier = 1.1f;
    [SerializeField]
    private float acceleration = 1f;
    [SerializeField]
    private float reverseAcceleration = 1f;
    [SerializeField]
    private float maxSpeed = 1f;
    [SerializeField]
    private float steering = 1f;
    [SerializeField]
    private float steeringAir = 1f;
    [SerializeField]
    private float driftSteering = 1f;
    [SerializeField]
    private float steeringVerticalAir = 1f;
    [SerializeField]
    private float jump = 1f;

    [SerializeField]
    private CarWheel[] rearWheels;

    private Vector2 move = Vector2.zero;
    private float accelerate = 0f;
    private bool isDrifting = false;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //GameManager.Instance.gameOver.AddListener(() => isOver = true);
        //AudioManager.Instance.Play("Engine", true);
    }

    private void Update()
    {
        if (CheckIfFallen())
        {
            StartCoroutine(Restaure());
        }
    }

    private bool CheckIfFallen()
    {
        return transform.rotation.eulerAngles.x > 60 && transform.rotation.eulerAngles.x < 300
            || (transform.rotation.eulerAngles.z > 60 && transform.rotation.eulerAngles.z < 300);
    }

    private IEnumerator Restaure()
    {
        yield return new WaitForSeconds(3f);

        if (CheckIfFallen())
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z);
        }
    }

    void FixedUpdate()
    {
        // float h = -Input.GetAxis("Horizontal");
        // float v = Input.GetAxis("Vertical");
        float h = -move.x;
        float vRotate = -move.y;
        float v = accelerate;
        //foreach(var wheel in wheels)
        //{
        //    wheel.rotation = Quaternion.FromToRotation(transform.forward, Quaternion.AngleAxis(-h * 20f, Vector3.up) * transform.forward);
        //}
        //AudioManager.Instance.ModifyPitch("Engine", v / 5f);
        // float multipler = player.PlayerType == "FINDER" ? finderSpeedMultiplier : 1f;
        const float multiplier = 1f;
        Vector3 speed = transform.forward * (v * (v > 0f ? acceleration : reverseAcceleration));
        var grounded = onGround();
        rb.drag = grounded ? 1f : 0.3f;
        if (grounded)
        {
            rb.AddForce(speed);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed * multiplier);

            float direction = Vector3.Dot(transform.forward, rb.velocity);

            rb.AddTorque(transform.up * ((direction >= 0f ? -1 : 1) * h * (isDrifting ? driftSteering : steering) * (rb.velocity.magnitude / (maxSpeed * multiplier))), ForceMode.VelocityChange);
        }
        else
        {
            Debug.Log(transform.up);
            rb.AddTorque(transform.up * -h * steeringAir, ForceMode.VelocityChange);
            if (isDrifting)
            {
                rb.AddTorque(transform.right * -vRotate * steeringVerticalAir, ForceMode.VelocityChange);

            }
            // transform.Rotate(transform.up, -h * steeringAir, Space.World);
            // transform.Rotate(transform.right, -v * steeringAir, Space.World);
        }
    }

    private void OnBecameVisible()
    {
        Debug.Log("Visible");
    }
    private void OnBecameInvisible()
    {
        Debug.Log("Invisible");
    }

    void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    void OnAccelerate(InputValue value)
    {
        accelerate = value.Get<float>();
    }
    void OnJump()
    {
        if (onGround())
        {
            rb.AddForce(transform.up * jump, ForceMode.VelocityChange);

        }
    }

    private bool onGround()
    {
        return rearWheels.All(w => w.isOnGround());
    }

    void OnDrift(InputValue value)
    {
        isDrifting = value.Get<float>() != 0f;
    }
}
