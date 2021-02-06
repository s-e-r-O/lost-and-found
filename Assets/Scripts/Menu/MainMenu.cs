using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private bool useSteam = false;
    [SerializeField] private GameObject landingPage = null;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

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

        if (!useSteam) { return; }
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

    }
    public void HostGame()
    {
        if (useSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, Room.maxConnections);
            return;
        }

        Room.StartHost();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            landingPage.SetActive(true);
            return;
        }
        Room.StartHost();
        Debug.Log(SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress", SteamUser.GetSteamID().ToString());
    }
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log(callback.m_steamIDFriend);
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        Debug.Log(SteamFriends.GetPersonaName());
        PlayerPrefs.SetString("PlayerName", SteamFriends.GetPersonaName()); 
        if (NetworkServer.active)
        {
            return;
        }
        string hostAddress =
        SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");

        Room.networkAddress = hostAddress;

        Room.StartClient();

        landingPage.SetActive(false);
        gameObject.SetActive(false);

    }
}
