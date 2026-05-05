using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyUI : MonoBehaviour
{
    [SerializeField] private Button ReadyButton;

    private void Awake()
    {
        ReadyButton.onClick.AddListener(() =>
        {
            CharacterSelecteReady.Instance.setPlayerReady();
        });
    }
}
