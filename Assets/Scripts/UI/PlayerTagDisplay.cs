using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTagDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private Color noTypeColor;
    [SerializeField] private Color finderColor;
    [SerializeField] private Color itemColor;
    [SerializeField] private GameObject finderImg;
    [SerializeField] private GameObject itemImg;
    [SerializeField] private GameObject spinnerImg;
    [SerializeField] private GameObject hostImg;
    [SerializeField] private GameObject outline;

    private Image playerTag;

    private void Awake()
    {
        playerTag = GetComponent<Image>();
    }

    public void SetValues(NetworkRoomPlayerLostFound player)
    {
        playerName.text = player.DisplayName;
        hostImg.SetActive(player.IsLeader);
        outline.SetActive(player.isLocalPlayer);

        spinnerImg.SetActive(player.PlayerType != "FINDER" && player.PlayerType != "ITEM");
        finderImg.SetActive(player.PlayerType == "FINDER");
        itemImg.SetActive(player.PlayerType == "ITEM");

        switch(player.PlayerType)
        {
            case "FINDER":
                playerTag.color = finderColor;
                break;
            case "ITEM":
                playerTag.color = itemColor;
                break;
            default:
                playerTag.color = noTypeColor;
                break;
        }
        
    }
}
