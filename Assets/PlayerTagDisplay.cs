using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerTagDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private GameObject finderImg;
    [SerializeField] private GameObject itemImg;

    public void SetValues(string name, string playerType)
    {
        playerName.text = name;
        finderImg.SetActive(playerType == "FINDER");
        itemImg.SetActive(playerType == "ITEM");
    }
}
