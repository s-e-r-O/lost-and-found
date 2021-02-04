using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerInitializer : MonoBehaviour
{
    [SerializeField] private NetworkManagerLostFound managerPrefab;
    [HideInInspector]
    public NetworkManagerLostFound Manager;
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.singleton == null)
        {
            Manager = Instantiate(managerPrefab);   
        } else
        {
            Manager = NetworkManager.singleton as NetworkManagerLostFound;
        }
    }
}
