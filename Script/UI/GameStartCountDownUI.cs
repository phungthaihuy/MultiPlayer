using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStartCountDownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownToStartUI;

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += Instance_OnStateChanged;

        Show();
    }

    private void Instance_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsCountDownToStartActive())
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
        countDownToStartUI.text = Mathf.Ceil(KitchenGameManager.Instance.GetCountDownToStartTimer()).ToString();
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
