using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private NetworkGamePlayerLostFound player;
    private void OnCollisionEnter(Collision collision)
    {
        PlayerCollisionHandler other = collision.gameObject.GetComponent<PlayerCollisionHandler>();
        if (other == null) { return; }
        player.OnPlayerCollision(other.player);
    }
}
