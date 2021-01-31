using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLostFound networkManager;

    private void Start()
    {
        AudioManager.Instance.ChangeBackgroundMusic("Menu");
    }
    public void HostGame()
    {
        networkManager.StartHost();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
