using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TesttingNetcodeUI : MonoBehaviour
{
    public static TesttingNetcodeUI Instance { get; private set; }
    public event EventHandler OnBtnClick;

    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        hostBtn.onClick.AddListener(() =>
        {
            Debug.Log("HOST!");
            NetworkManager.Singleton.StartHost();
            OnBtnClick?.Invoke(this, EventArgs.Empty);

            Hide();
        });

        clientBtn.onClick.AddListener(() =>
        {
            Debug.Log("Client!");
            NetworkManager.Singleton.StartClient();
            OnBtnClick?.Invoke(this, EventArgs.Empty);

            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
