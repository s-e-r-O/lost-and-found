using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text finders;
    [SerializeField] private TMP_Text items;
    private NetworkManagerLostFound room;
    private NetworkManagerLostFound Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLostFound;
        }
    }

    public void SetWinner(string winner)
    {
        finders.gameObject.SetActive(winner == "FINDER");
        items.gameObject.SetActive(winner == "ITEM");
    }

    public void Back()
    {
        Room.StopHost();
    }
}
