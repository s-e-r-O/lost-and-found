using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
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

    [SerializeField] private ParticleSystem dust;

    [Header("Audio")]
    [SerializeField] private float pitchScale = 5f;
    [SerializeField] private float pitchBase = 0.1f;
    [SerializeField] private float pitchTime = 0.5f;

    private Vector2 move = Vector2.zero;
    private float accelerate = 0f;
    private float rotate = 0f;
    private bool isDrifting = false;

    private Rigidbody rb;

    [SerializeField] private AudioSource engineSound = null;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //GameManager.Instance.gameOver.AddListener(() => isOver = true);
        //AudioManager.Instance.Play("Engine", true);
    }

    private void Update()
    {
        if (OnGround())
        {
            engineSound.pitch = Mathf.Lerp(engineSound.pitch, Mathf.Abs(accelerate) / pitchScale + pitchBase, pitchTime * Time.deltaTime);
        }
        else
        {
            engineSound.pitch = Mathf.Lerp(engineSound.pitch, pitchBase / 2f, 2f * pitchTime * Time.deltaTime);
        }

        //if (CheckIfFallen())
        //{
        //    StartCoroutine(Restaure());
        //} else
        //{
        //    StopCoroutine(Restaure());
        //}
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
        float vRotate = rotate;
        float v = accelerate;

        //foreach(var wheel in wheels)
        //{
        //    wheel.rotation = Quaternion.FromToRotation(transform.forward, Quaternion.AngleAxis(-h * 20f, Vector3.up) * transform.forward);
        //}
        //AudioManager.Instance.ModifyPitch("Engine", v / 5f);
        // float multipler = player.PlayerType == "FINDER" ? finderSpeedMultiplier : 1f;
        Vector3 speed = transform.forward * (v * (v > 0f ? acceleration : reverseAcceleration));
        var grounded = OnGround();
        rb.drag = grounded ? 1f : 0.3f;

        if (grounded)
        {
            rb.AddForce(speed);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

            float direction = Vector3.Dot(transform.forward, rb.velocity);

            float steeringForce = (direction >= 0f ? -1 : 1) * h * (isDrifting ? driftSteering : steering) * (rb.velocity.magnitude / (maxSpeed));
            rb.AddTorque(transform.up * steeringForce, ForceMode.VelocityChange);
                foreach(var wheel in rearWheels)
                {
                    wheel.SetSteeringForce(isDrifting? steeringForce : 0f);
                }
        }
        else
        {
            rb.AddTorque(transform.up * -h * steeringAir, ForceMode.VelocityChange);
            rb.AddTorque(transform.right * vRotate * steeringVerticalAir, ForceMode.VelocityChange);
        }
    }

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnAccelerate(InputValue value)
    {
        accelerate = value.Get<float>();
    }
    public void OnRotate(InputValue value)
    {
        rotate = value.Get<float>();
    }

    public void OnJump()
    {
        if (OnGround())
        {
            rb.AddForce(transform.up * jump, ForceMode.VelocityChange);
            dust.Play();
        }
    }

    private bool OnGround()
    {
        return rearWheels.Any(w => w.IsOnGround());
    }

    public void OnDrift(InputValue value)
    {
        isDrifting = value.Get<float>() != 0f;
    }
}
