using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ManagerInitializer : MonoBehaviour
{
    [SerializeField] private NetworkManagerLostFound managerPrefab;
    [SerializeField] private GameObject mainMenu;
    [HideInInspector]
    public NetworkManagerLostFound Manager;

    public UnityEvent onConnected = new UnityEvent();
    public UnityEvent onDisconnected = new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.singleton == null)
        {
            Manager = Instantiate(managerPrefab);   
        } else
        {
            Manager = NetworkManager.singleton as NetworkManagerLostFound;
            if (Manager.isNetworkActive)
            {
                mainMenu.SetActive(false);
            }
        }

        NetworkManagerLostFound.OnClientConnected += onConnected.Invoke;
        NetworkManagerLostFound.OnClientDisconnected += onDisconnected.Invoke;
    }
}
