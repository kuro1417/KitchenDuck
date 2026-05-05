using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private void Start()
    {
        if(Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCouterChanged += Player_OnSelectedCouterChanged;
        }
        else
        {
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }
       
    }

    private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCouterChanged -= Player_OnSelectedCouterChanged;
            Player.LocalInstance.OnSelectedCouterChanged += Player_OnSelectedCouterChanged;
        }
    }

    private void Player_OnSelectedCouterChanged(object sender, Player.OnSelectedCounterChangedArgs e)
    {
        if(e.selectedCounter == baseCounter)
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
        foreach(GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }     
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        { 
            visualGameObject.SetActive(false);
        }
    }
}