using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Services.Lobbies.Models;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button MainMenuButton;
    [SerializeField] private Button ReadyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI LobbyCodeText;

    private void Awake()
    {
        MainMenuButton.onClick.AddListener(() =>
        {
            if (KitchenGameLobby.Instance.IsLobbyHost())
            {
                KitchenGameLobby.Instance.DeleteLobby();
            }
            else
            {
                KitchenGameLobby.Instance.LeaveLobby();
            }

            
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScenes);
        });
        ReadyButton.onClick.AddListener(() =>
        {
            CharacterSelecteReady.Instance.setPlayerReady();
        });
    }

    private void Start()
    {
        Lobby lobby = KitchenGameLobby.Instance.GetLobby();

        lobbyNameText.text = "Lobby Name: "+ lobby.Name;
        LobbyCodeText.text = "Code: " + lobby.LobbyCode;
    }
}