using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomPlayerLostFound : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private Button startGameButton;
    [SerializeField] private VerticalLayoutGroup playerList;

    [SerializeField] private PlayerTagDisplay playerDisplayPrefab;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "";

    [SyncVar(hook = nameof(HandlePlayerTypeChanged))]
    public string PlayerType = "";

    private bool isLeader;
    public bool IsLeader { set { isLeader = value; startGameButton.gameObject.SetActive(isLeader); } }

    private NetworkManagerLostFound room;
    private NetworkManagerLostFound Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLostFound;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);
        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);
        UpdateDisplay();
    }

    public override void OnStopClient()
    {
            Room.RoomPlayers.Remove(this);
            UpdateDisplay();
        //if (isClientOnly)
        //{
        //}
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandlePlayerTypeChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }
        foreach (Transform child in playerList.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            PlayerTagDisplay playerTag = Instantiate(playerDisplayPrefab, playerList.transform);
            playerTag.SetValues(Room.RoomPlayers[i].DisplayName, Room.RoomPlayers[i].PlayerType);
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }
        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdSetType(string type)
    {
        PlayerType = type;
        Room.NotifyPlayersOfReadyState();
    }
    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }
        Room.StartGame();
    }
}
