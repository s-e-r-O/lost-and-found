using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowArrow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed=100f;

    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (target != null)
        {
            var direction = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);
        }
    }

    public void Show(Transform target)
    {
        this.target = target;
        anim.ResetTrigger("Hide");
        anim.ResetTrigger("Show");
        anim.SetTrigger("Show");
    }

    public void Hide()
    {
        //target = null;
        anim.ResetTrigger("Show");
        anim.ResetTrigger("Hide");
        anim.SetTrigger("Hide");

    }
}
