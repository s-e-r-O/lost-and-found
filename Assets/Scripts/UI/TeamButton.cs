using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamButton : MonoBehaviour
{
    [SerializeField] NetworkRoomPlayerLostFound roomPlayer;
    [SerializeField] private string playerType;
    [SerializeField] private GameObject outline;

    public void SetValues(NetworkRoomPlayerLostFound roomPlayer)
    {
        outline.SetActive(roomPlayer.PlayerType == playerType);
    }
    public void OnClick()
    {
        roomPlayer.CmdSetType(playerType);
    }
}
