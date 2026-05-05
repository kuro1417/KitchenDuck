using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveWarnBurningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private void Start()
    {
        stoveCounter.OnProgessChanged += StoveCounter_OnProgessChanged;
        Hide();
    }

    private void StoveCounter_OnProgessChanged(object sender, IHasProgess.OnProgessChangedEventArgs e)
    {
        float burnProgessAmount = .5f;
        bool show = stoveCounter.IsFried() && e.progressNormalized >= burnProgessAmount;

        if (show)
        {
            Show();
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
