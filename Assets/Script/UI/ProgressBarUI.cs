using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image barImage;

    private IHasProgess hasProgress;
    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgess>();
        if(hasProgress == null)
        {
            Debug.Log("Game Object" + hasProgressGameObject + "does not have a component that implements IHasProgess!");
        }
        hasProgress.OnProgessChanged += HasProgress_OnProgessChanged;

        barImage.fillAmount= 0f;
        Hide();
    }

    private void HasProgress_OnProgessChanged(object sender, IHasProgess.OnProgessChangedEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;

        if(e.progressNormalized == 0f || e.progressNormalized == 1f)
        {
            Hide();
        }
        else
        {
            Show();
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
