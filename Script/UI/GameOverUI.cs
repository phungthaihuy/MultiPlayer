using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeDeliverNumber;

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += Instance_OnStateChanged;

        Hide();
    }

    private void Instance_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsGameOver())
        {
            Show();
        }
        else
        {
            Hide();
        }

    }

    private void Update()
    {
        recipeDeliverNumber.text = Mathf.Ceil(DeliveryManager.Instance.GetSuccessRecipeAmount()).ToString();
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
