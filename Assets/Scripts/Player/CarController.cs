using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : NetworkBehaviour
{

    [SerializeField]
    private NetworkGamePlayerLostFound player = null;

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
    private Transform[] wheels;

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
        if (hasAuthority && CheckIfFallen())
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
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    void FixedUpdate()
    {
        if (hasAuthority)
        {
            float h = -Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            //foreach(var wheel in wheels)
            //{
            //    wheel.rotation = Quaternion.FromToRotation(transform.forward, Quaternion.AngleAxis(-h * 20f, Vector3.up) * transform.forward);
            //}
            //AudioManager.Instance.ModifyPitch("Engine", v / 5f);
            float multipler = player.PlayerType == "FINDER" ? finderSpeedMultiplier : 1f;
            Vector3 speed = transform.forward * (v * (v > 0f ? acceleration : reverseAcceleration));
            rb.AddForce(speed);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed * multipler);

            float direction = Vector3.Dot(transform.forward, rb.velocity);

            transform.Rotate(transform.up, ((direction >= 0f ? -1 : 1) * h * steering * rb.velocity.magnitude / (maxSpeed * multipler)));
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
}
