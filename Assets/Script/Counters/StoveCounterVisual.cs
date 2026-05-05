using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private GameObject stoveGameObject;
    [SerializeField] private GameObject particelsGameObject;
    [SerializeField] private StoveCounter stoveCounter;

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool ShowVisual = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried || e.state == StoveCounter.State.Burned;
        stoveGameObject.SetActive(ShowVisual);
        particelsGameObject.SetActive(ShowVisual);
    }
}
