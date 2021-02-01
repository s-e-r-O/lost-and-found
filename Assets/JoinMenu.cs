using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class JoinMenu : MonoBehaviour
{
    private NetworkManagerLostFound room;
    private NetworkManagerLostFound Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLostFound;
        }
    }
    [Header("UI")]
    [SerializeField] private TMP_InputField ipAddressInputField;
    [SerializeField] private List<Selectable> toDisableWhenJoining;
    [SerializeField] private GameObject spinner;

    private const string PlayerPrefsIPKey = "HostIP";

    public UnityEvent onConnected = new UnityEvent();
    public UnityEvent onDisconnected = new UnityEvent();
    void Start()
    {
        SetUpInputField();
    }

    private void SetUpInputField()
    {
        //if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        var ip = PlayerPrefs.GetString(PlayerPrefsIPKey, "localhost");

        ipAddressInputField.text = ip;

    }
    private void OnEnable()
    {
        NetworkManagerLostFound.OnClientConnected += HandleClientConnected;
        NetworkManagerLostFound.OnClientDisconnected += HandleClientDisconnected;
    }
    private void OnDisable()
    {
        NetworkManagerLostFound.OnClientConnected -= HandleClientConnected;
        NetworkManagerLostFound.OnClientDisconnected -= HandleClientDisconnected;
    }
    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;
        Room.networkAddress = ipAddress;
        PlayerPrefs.SetString(PlayerPrefsIPKey, ipAddress);
        Room.StartClient();
        toDisableWhenJoining.ForEach(selectable => selectable.interactable = false);
        spinner.SetActive(true);
    }
    private void HandleClientConnected()
    {
        toDisableWhenJoining.ForEach(selectable => selectable.interactable = true); 
        spinner.SetActive(false);
        onConnected.Invoke();

    }
    private void HandleClientDisconnected()
    {
        toDisableWhenJoining.ForEach(selectable => selectable.interactable = true); 
        spinner.SetActive(false);
        onDisconnected.Invoke();
    }
}
