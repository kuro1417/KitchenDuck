using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingPlayerReadyUI : MonoBehaviour
{
    private void Start()
    {
        KitchenGameManager.Instance.OnLocalReadyPlayerChanged += KitchenGameManager_OnLocalReadyPlayerChanged;
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsGameCountDownToStartActive() || KitchenGameManager.Instance.IsGamePlaying())
        {
            Hide();
        }
    }

    private void KitchenGameManager_OnLocalReadyPlayerChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsLocalPlayerReady())
        {
            Show();
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

   private void Show()
    {
        gameObject.SetActive(true); 
    }
}
