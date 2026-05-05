using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeDeliveryText;
    [SerializeField] private Button RestartButton;
    [SerializeField] private Button MainMenuButton;
    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Hide();
        RestartButton.Select();
    }

    private void Awake()
    {
        RestartButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.GameScenes);
        });

        MainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScenes);
        });
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        int Score = DeliveryManager.Instance.GetSuccessfulRecipesAmount();
        if (KitchenGameManager.Instance.IsGameOver())
        {
            Show();
            recipeDeliveryText.text = Score.ToString();
        }
        else
        {
            Hide();
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
}
