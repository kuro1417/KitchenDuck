using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";
    [SerializeField] private StoveCounter stoveCounter;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter.OnProgessChanged += StoveCounter_OnProgessChanged;

        animator.SetBool(IS_FLASHING, false);
    }

    private void StoveCounter_OnProgessChanged(object sender, IHasProgess.OnProgessChangedEventArgs e)
    {
        float burnProgessAmount = .5f;
        bool show = stoveCounter.IsFried() && e.progressNormalized >= burnProgessAmount;
        
        animator.SetBool(IS_FLASHING, show);
    }
}
