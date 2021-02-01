using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayMenu : MonoBehaviour
{
    public UnityEvent onConnected = new UnityEvent();
    public UnityEvent onDisconnected = new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        NetworkManagerLostFound.OnClientConnected += onConnected.Invoke;
        NetworkManagerLostFound.OnClientDisconnected += onDisconnected.Invoke;
    }
}
