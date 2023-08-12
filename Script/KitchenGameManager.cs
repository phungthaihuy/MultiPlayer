using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameManager : NetworkBehaviour
{
    public static KitchenGameManager Instance { get; private set; }
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalPlayerReadyChanged;

    private enum State
    {
        WaitingToStart,
        CountDownToStart,
        GamePlaying,
        GameOver
    }

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countDownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 90;
    private float gamePlayingTimerStaticMax = 90f;
    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        ReadyUI.Instance.OnReadybtnClick += ReadyUI_OnReadybtnClick;
    }

    private void ReadyUI_OnReadybtnClick(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();
        }
        //Debug.Log(isLocalPlayerReady);
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
    }
    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientIdsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                //this PLayer is Not ready
                allClientIdsReady = false;
                break;
            }
        }
        //Debug.Log(playerReadyDictionary[serverRpcParams.Receive.SenderClientId]);
        if (allClientIdsReady) // chuyen game state tu waiting to start thanh countdown to start
        {
            state.Value = State.CountDownToStart;
        }
    }

    void Update()
    {
        if (!IsServer)
        {
            return;
        }
        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountDownToStart:
                countDownToStartTimer.Value -= Time.deltaTime;
                if (countDownToStartTimer.Value < 0f)
                {
                    state.Value = State.GamePlaying;
                    //gamePlayingTimer = gamePlayingTimerMax;
                    //OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimerMax -= Time.deltaTime;
                gamePlayingTimer.Value = gamePlayingTimerMax;
                if (gamePlayingTimerMax < 0f)
                {
                    state.Value = State.GameOver;
                    //OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
            default:
                break;
        }
    }

    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }

    public bool IsCountDownToStartActive()
    {
        return state.Value == State.CountDownToStart;
    }

    public float GetCountDownToStartTimer()
    {
        return countDownToStartTimer.Value;
    }

    public float GetGamePlayingTimerNomalize()
    {
        return 1 - gamePlayingTimer.Value / gamePlayingTimerStaticMax;
    }

    public bool GetIsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }
}
