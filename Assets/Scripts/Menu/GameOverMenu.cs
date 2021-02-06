using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text finders;
    [SerializeField] private TMP_Text items;
    [SerializeField] private GameObject waitHost;
    [SerializeField] private GameObject playAgainButton;
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

    public void SetWinner(string winner, bool isLeader)
    {
        finders.gameObject.SetActive(winner == "FINDER");
        items.gameObject.SetActive(winner == "ITEM");
        anim.ResetTrigger("GameOver");
        anim.SetTrigger("GameOver");
        playAgainButton.SetActive(isLeader);
        waitHost.SetActive(!isLeader);
    }

    public void Back()
    {
        Room.StopHost();
        //StartCoroutine(BackRoutine());
    }

    public void PlayAgain()
    {
        Room.PlayAgain();
        //StartCoroutine(BackRoutine());
    }

    IEnumerator BackRoutine()
    {
        SceneTransition.Instance.Close();
        yield return new WaitForSeconds(1f);

        Room.StopHost();
        yield return new WaitForSeconds(1f);

        SceneTransition.Instance.Open();
    }
}
