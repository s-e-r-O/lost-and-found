using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class JoinMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLostFound networkManager;

    [Header("UI")]
    [SerializeField] private TMP_InputField ipAddressInputField;
    [SerializeField] private Button joinButton;

    private const string PlayerPrefsIPKey = "HostIP";

    public UnityEvent onConnected = new UnityEvent();
    public UnityEvent onDisconnected = new UnityEvent();
    void Start()
    {
        SetUpInputField();
        NetworkManagerLostFound.OnClientConnected += HandleClientConnected;
        NetworkManagerLostFound.OnClientDisconnected += HandleClientDisconnected;
    }

    private void SetUpInputField()
    {
        //if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        var ip = PlayerPrefs.GetString(PlayerPrefsIPKey, "localhost");

        ipAddressInputField.text = ip;

    }
    private void OnEnable()
    {
        //NetworkManagerLostFound.OnClientConnected += HandleClientConnected;
        //NetworkManagerLostFound.OnClientDisconnected += HandleClientDisconnected;
    }
    private void OnDisable()
    {
        //NetworkManagerLostFound.OnClientConnected -= HandleClientConnected;
        //NetworkManagerLostFound.OnClientDisconnected -= HandleClientDisconnected;
    }
    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;
        networkManager.networkAddress = ipAddress;
        PlayerPrefs.SetString(PlayerPrefsIPKey, ipAddress);
        networkManager.StartClient();
        joinButton.interactable = false;
    }
    private void HandleClientConnected()
    {
        joinButton.interactable = true;
        onConnected.Invoke();

    }
    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
        onDisconnected.Invoke();
    }
}
