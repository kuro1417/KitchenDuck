using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button MainMenuButtton;
    [SerializeField] private Button ResumeButtton;
    [SerializeField] private Button OptionButton;

    private void Awake()
    {
        MainMenuButtton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScenes);
        });

        ResumeButtton.onClick.AddListener(() => { 
            KitchenGameManager.Instance.TogglePauseGame();
        });

        OptionButton.onClick.AddListener(() => {
            Hide();
            OptionUI.Instance.Show(Show);
        });
    }
    private void Start()
    {
        KitchenGameManager.Instance.OnLocalGamePause += KitchenGameManager_OnLocalGamePause;
        KitchenGameManager.Instance.OnLocalGameUnPause += KitchenGameManager_OnLocalGameUnPause;

        Hide();
    }

    private void KitchenGameManager_OnLocalGameUnPause(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void KitchenGameManager_OnLocalGamePause(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);

        ResumeButtton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
