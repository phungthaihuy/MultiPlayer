using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGOArray;
    private void Start()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChanged += Instance_OnSelectedCounterChanged;
        }
        else
        {
            Player.OnAnyPlayerSpawn += Player_OnAnyPlayerSpawn;
        }
    }

    private void Player_OnAnyPlayerSpawn(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChanged -= Instance_OnSelectedCounterChanged;
            Player.LocalInstance.OnSelectedCounterChanged += Instance_OnSelectedCounterChanged;
        }
    }

    private void Instance_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.baseCounter == baseCounter)
        {
            Show();
        }
        else if (e.baseCounter != baseCounter)
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject visualGO in visualGOArray)
        {
            visualGO.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject visualGO in visualGOArray)
        {
            visualGO.SetActive(false);
        }
    }
}
