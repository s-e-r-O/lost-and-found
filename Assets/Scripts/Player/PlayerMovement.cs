using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    float rotationSpeed =3f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float walkDampingBasic = 0.4f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float walkDampingWhenStopping = 0.8f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float walkDampingWhenTurning = 0.5f;
    
    public Animator anim;

    private Rigidbody rb;
    private Vector2 movementInput;
    private Camera mainCamera;

    // Gizmos values
    private Vector3 gizmos_targetPosition;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        Walk();
        SetAnimationWalkingValues();
    }

    private void Walk()
    {
        if (isLocalPlayer)
        {
            OnMove();
            float xvelocity = rb.velocity.x;
            float zvelocity = rb.velocity.z;

            xvelocity += movementInput.x;
            zvelocity += movementInput.y;

            float dampingX = GetDamping(movementInput.x, xvelocity);
            float dampingZ = GetDamping(movementInput.y, zvelocity);

            xvelocity *= Mathf.Pow(1f - dampingX, Time.fixedDeltaTime * 10f);
            zvelocity *= Mathf.Pow(1f - dampingZ, Time.fixedDeltaTime * 10f);

            rb.velocity = new Vector3(xvelocity, rb.velocity.y, zvelocity);

            if (new Vector2(xvelocity, zvelocity).magnitude > 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(xvelocity, 0f, zvelocity)), Time.fixedDeltaTime * rotationSpeed);
            }

            gizmos_targetPosition = transform.position + new Vector3(xvelocity, transform.position.y, zvelocity);
        }
    }


    public void OnMove()
    {
        var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementInput = input.ApplyCameraPerspective(mainCamera);
    }

    private float GetDamping(float inputValue, float currentVelocity)
    {
        if (Mathf.Abs(inputValue) < 0.1f)
        {
            return walkDampingWhenStopping;
        }
        else if (Mathf.Sign(currentVelocity) != Mathf.Sign(inputValue))
        {
            return walkDampingWhenTurning;
        }
        return walkDampingBasic;
    }
    void SetAnimationWalkingValues()
    {
        var speed = new Vector2(rb.velocity.x, rb.velocity.z);
        anim.SetFloat("WalkSpeed", speed.magnitude);
        //if (speed.magnitude >= 0.1f)
        //{
        //    var values = speed.normalized.RotatePerspective(transform);
        //    anim.SetFloat("XSpeed", values.x);
        //    anim.SetFloat("YSpeed", values.y);
        //}
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, gizmos_targetPosition);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        Gizmos.DrawLine(transform.position, transform.position + transform.right);

    }
}