using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour
{
    private void Start()
    {
        KitchenGameManager.Instance.OnMultiplayerGamePaused += KichenGameManager_OnMultiplayerGamePaused;
        KitchenGameManager.Instance.OnMultiplayerGameUnPaused += KitchenGameManager_OnMultiplayerGameUnPaused;

        Hide();
    }

    private void KitchenGameManager_OnMultiplayerGameUnPaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void KichenGameManager_OnMultiplayerGamePaused(object sender, System.EventArgs e)
    {
        ulong localClientID = NetworkManager.Singleton.LocalClientId;

        if (KitchenGameManager.Instance.IsThisPlayerPause(localClientID) == true)
        {
            Hide();
            return;
        }
        Show();
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
