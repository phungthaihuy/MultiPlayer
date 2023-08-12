using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ReadyUI : MonoBehaviour
{
    public static ReadyUI Instance { get; private set; }
    [SerializeField] private Button readyBtn;
    
    public event EventHandler OnReadybtnClick;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        readyBtn.onClick.AddListener(() =>
        {
            OnReadybtnClick?.Invoke(this, EventArgs.Empty);
        });

        KitchenGameManager.Instance.OnLocalPlayerReadyChanged += Instance_OnLocalPlayerReadyChanged;

        Show();
    }

    private void Instance_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if (KitchenGameManager.Instance.GetIsLocalPlayerReady())
        {
            Hide();
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
