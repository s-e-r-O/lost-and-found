using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private NetworkManagerLostFound room;
    private NetworkManagerLostFound Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLostFound;
        }
    }

    private void Start()
    {

        AudioManager.Instance.ChangeBackgroundMusic("Menu");
    }
    public void HostGame()
    {
        Room.StartHost();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
