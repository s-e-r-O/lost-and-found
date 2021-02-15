using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CarFX : MonoBehaviour
{
    private ParticleSystem _ps;
    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        CarJumpBehaviour.OnJumped += JumpFX;
    }
    private void OnDisable()
    {
        CarJumpBehaviour.OnJumped -= JumpFX;
    }

    private void JumpFX()
    {
        _ps.Play();
    }
}
