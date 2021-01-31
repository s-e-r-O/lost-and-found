using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] TMP_InputField playerInputField;
    [SerializeField] Button[] disabledButtons;

    public static string DisplayName { get; private set; }
    private const string PlayerPrefsNameKey = "PlayerName";
    // Start is called before the first frame update
    void Start()
    {
        SetUpInputField();
    }

    private void SetUpInputField()
    {
        //if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        var displayName = PlayerPrefs.GetString(PlayerPrefsNameKey, "");

        playerInputField.text = displayName;
        SetPlayerName(displayName);

    }

    public void SetPlayerName(string name)
    {
        foreach(var button in disabledButtons)
        {
            button.interactable = !string.IsNullOrEmpty(name);
        }
    }
    public void SavePlayerName()
    {
        DisplayName = playerInputField.text;
        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
