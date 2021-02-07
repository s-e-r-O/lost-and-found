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
    [SerializeField] private GameObject waitingPlayersUI;
    [SerializeField] private GameObject waitingHostUI;
    [SerializeField] private GameObject waitingTeamsUI;
    [SerializeField] private VerticalLayoutGroup playerList;
    [SerializeField] private TeamButton[] teamButtons;

    [SerializeField] private PlayerTagDisplay playerDisplayPrefab;


    private bool shouldUpdateUI = false;

    [HideInInspector]
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "";

    [HideInInspector]
    [SyncVar(hook = nameof(HandlePlayerTypeChanged))]
    public string PlayerType = "";

    [HideInInspector]
    [SyncVar(hook = nameof(HandleIsLeaderChanged))]
    public bool IsLeader = false;

    [HideInInspector]
    [SyncVar(hook = nameof(HandleLobbyStateChanged))]
    public LobbyState LobbyState = LobbyState.EMPTY;

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
        CmdSetDisplayName(PlayerPrefs.GetString("PlayerName", ""));
        SetUIActive(true);
    }

    [ClientCallback]
    public void SetUIActive(bool isActive)
    {
        lobbyUI.SetActive(isActive);
        shouldUpdateUI = isActive;
    }
    //public override void OnStartServer()
    //{
    //    Room.RoomPlayers.Add(this);
    //    DontDestroyOnLoad(gameObject);
    //}
    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);
        UpdateDisplay();
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);
        UpdateDisplay();
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandlePlayerTypeChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleIsLeaderChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleLobbyStateChanged(LobbyState oldValue, LobbyState newValue) => UpdateDisplay();

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
        if (shouldUpdateUI)
        {
            foreach (TeamButton teamButton in teamButtons)
            {
                teamButton.SetValues(this);
            }


            foreach (Transform child in playerList.transform)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < Room.RoomPlayers.Count; i++)
            {
                PlayerTagDisplay playerTag = Instantiate(playerDisplayPrefab, playerList.transform);
                playerTag.SetValues(Room.RoomPlayers[i]);
            }


            bool alreadyChoosed = PlayerType == "FINDER" || PlayerType == "ITEM";
            waitingPlayersUI.SetActive(alreadyChoosed && (LobbyState == LobbyState.NOT_ENOUGH_PLAYERS || LobbyState == LobbyState.PLAYERS_STILL_CHOOSING));
            waitingTeamsUI.SetActive(alreadyChoosed && LobbyState == LobbyState.EMPTY_TEAM);
            waitingHostUI.SetActive(!IsLeader && alreadyChoosed && LobbyState == LobbyState.READY);
            startGameButton.gameObject.SetActive(IsLeader && alreadyChoosed && LobbyState == LobbyState.READY);
            startGameButton.interactable = IsLeader && alreadyChoosed && LobbyState == LobbyState.READY;
        }
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
        if (!IsLeader) { return; }
        Room.StartGame();
    }

    public void LeaveGame()
    {
        Room.LeaveGame(isServer);
    }

    [TargetRpc]
    public void TargetCloseTransition()
    {
        SceneTransition.Instance.Close();
    }

    [TargetRpc]
    public void TargetHideUI()
    {
        SetUIActive(false);
    }
    [TargetRpc]
    public void TargetShowUI()
    {
        SetUIActive(true);
        UpdateDisplay();
    }
}
