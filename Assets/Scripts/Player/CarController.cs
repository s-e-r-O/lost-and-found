﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : NetworkBehaviour
{

    [SerializeField]
    private float acceleration = 1f;
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
        if (isLocalPlayer && CheckIfFallen())
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
        if (isLocalPlayer)
        {
            float h = -Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            //foreach(var wheel in wheels)
            //{
            //    wheel.rotation = Quaternion.FromToRotation(transform.forward, Quaternion.AngleAxis(-h * 20f, Vector3.up) * transform.forward);
            //}
            //AudioManager.Instance.ModifyPitch("Engine", v / 5f);
            Vector3 speed = transform.forward * (v * acceleration);
            rb.AddForce(speed);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

            float direction = Vector3.Dot(transform.forward, rb.velocity);

            transform.Rotate(transform.up, ((direction >= 0f? -1 : 1) * h * steering * rb.velocity.magnitude / maxSpeed));
        }
    }
}
