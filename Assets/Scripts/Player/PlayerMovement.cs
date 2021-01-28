using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    private float speed = 5f;
    void HandleMovement()
    {
        if (isLocalPlayer)
        {
            Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            transform.position = transform.position + new Vector3(dir.x, dir.y, 0) * speed * Time.deltaTime;
        }
    }

    void Update()
    {
        HandleMovement();
    }
}
