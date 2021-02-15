using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private float pitchScale = 5f;
    [SerializeField] private float pitchBase = 0.1f;
    [SerializeField] private float pitchTime = 0.5f;

    public CarGroundChecker CarGroundChecker = null;

    [SerializeField] private AudioSource engineSound = null;
    // Start is called before the first frame update
    void Awake()
    {
        var wheels = GetComponentsInChildren<CarWheel>();
        CarGroundChecker = new CarGroundChecker(wheels);
    }

    private void Update()
    {
        if (CarGroundChecker.OnGround)
        {
            //engineSound.pitch = Mathf.Lerp(engineSound.pitch, Mathf.Abs(accelerate) / pitchScale + pitchBase, pitchTime * Time.deltaTime);
        }
        else
        {
            //engineSound.pitch = Mathf.Lerp(engineSound.pitch, pitchBase / 2f, 2f * pitchTime * Time.deltaTime);
        }
    }
}
