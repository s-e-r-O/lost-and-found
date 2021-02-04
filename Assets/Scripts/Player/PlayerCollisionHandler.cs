using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private NetworkGamePlayerLostFound player;
    [SerializeField] private AudioSource hit;
    [SerializeField] private AudioSource bush;
    public LayerMask bushLayer;
    private void OnCollisionEnter(Collision collision)
    { 
        PlayerCollisionHandler other = collision.gameObject.GetComponent<PlayerCollisionHandler>();
        if (other == null) 
        {
            //AudioManager.Instance.Play("Hit");
            hit.Play();
            return; 
        }
        player.OnPlayerCollision(other.player);
        AudioManager.Instance.Play("Caught");
    }

    private void OnTriggerEnter(Collider collision)
    {
        PlayerCollisionHandler other = collision.gameObject.GetComponent<PlayerCollisionHandler>();

        if (bushLayer == (bushLayer | (1 << collision.gameObject.layer))) {
            bush.Play();   
        }
            //AudioManager.Instance.Play("Bush");
    }
}
