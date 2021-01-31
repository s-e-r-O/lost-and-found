using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text finders;
    [SerializeField] private TMP_Text items;
    private Animator anim;
    private NetworkManagerLostFound room;
    private NetworkManagerLostFound Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLostFound;
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetWinner(string winner)
    {
        finders.gameObject.SetActive(winner == "FINDER");
        items.gameObject.SetActive(winner == "ITEM");
        anim.ResetTrigger("GameOver");
        anim.SetTrigger("GameOver");
    }

    public void Back()
    {
        Room.StopHost();
    }
}
