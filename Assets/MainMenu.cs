using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLostFound networkManager;

    public void HostGame()
    {
        networkManager.StartHost();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
