using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI messageText;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailToJoinGame += KitchenGameMultiplayer_OnFailToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KichenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KichenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KichenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnJoinFailed += KichenGameLobby_OnJoinFailed;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KichenGameLobby_OnQuickJoinFailed;

        Hide();
    }

    private void KichenGameLobby_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("failed to quick join lobby");
    }

    private void KichenGameLobby_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Could not find a lobby");
    }

    private void KichenGameLobby_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining........");
    }

    private void KichenGameLobby_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("failed to create Lobby");
    }

    private void KichenGameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating........");
    }

    private void ShowMessage(string message)
    {
        Show();

        messageText.text = message;
    }

    private void KitchenGameMultiplayer_OnFailToJoinGame(object sender, System.EventArgs e)
    {
        if(NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }

        Show();

        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if(messageText.text == "")
        {
            messageText.text = "Fail to connect";
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailToJoinGame -= KitchenGameMultiplayer_OnFailToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KichenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KichenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= KichenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnJoinFailed -= KichenGameLobby_OnJoinFailed;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KichenGameLobby_OnQuickJoinFailed;
    }
}