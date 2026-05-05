using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button multiPlayerButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button settingButton;

    private void Awake()
    {
        multiPlayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.playMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScences);         
        });
        
        singlePlayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.playMultiplayer = false;
            Loader.Load(Loader.Scene.LobbyScences);
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        settingButton.onClick.AddListener(() =>
        {
            SettingUI.Instance.Show();
        });

        Time.timeScale= 1.0f;
    }

 
}
